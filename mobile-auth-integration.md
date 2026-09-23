# Connecting a Flutter App: Authentication & Authorization Guide

This document explains exactly how auth works in this API today and what you need to
set up on both the backend and the Flutter side before the mobile app can talk to it.
It reflects the current state of the code (not the plan) — see `git log` if something
here looks stale later.

---

## 1. The short version

- This is a **real OAuth2/OIDC server** (OpenIddict), not a simple "POST email+password,
  get a JWT back" API. You cannot do a plain REST login call for existing accounts.
- Flow used: **Authorization Code + PKCE**, the standard, recommended flow for native
  mobile apps (no client secret, no app-embedded password form).
- Flutter should use the **`flutter_appauth`** package (wraps AppAuth on iOS/Android),
  which implements this exact flow via the system browser (Custom Tabs / ASWebAuthenticationSession).
- **Registration is a normal JSON REST call** (`POST /api/auth/register`) — only *login*
  goes through the browser-based OAuth dance.
- Every business endpoint requires a Bearer access token. Role (`Admin`/`Member`) and,
  for Members, a `member_id` claim drive both role checks and "you can only see your
  own records" ownership checks.
- **Before this can work at all, you must register a mobile OAuth client** — right now
  only Swagger's redirect URI is registered. See §4, this is the #1 blocker.

---

## 2. Identity model

Two separate entities — don't confuse them:

| Entity | Purpose | Key fields |
|---|---|---|
| `User` | Login/security identity | `Email`, `PasswordHash`, `Role` (`Admin`/`Member`), nullable `MemberId` |
| `Member` | Library-patron business record | Name, contact info, borrowing history |

- An `Admin` user has **no** `MemberId` (admins don't borrow books).
- A `Member`-role user **must** have a `MemberId` linking it to their `Member` record.
- Registering via `/api/auth/register` creates **both** a `Member` and a `Member`-role
  `User` in one call.

---

## 3. Endpoints that matter to the mobile app

### `POST /api/auth/register` — plain JSON, no OAuth dance
```json
// Request
{ "fullName": "...", "email": "...", "phoneNumber": "...", "password": "..." } // password: min 8 chars

// 201 Created
{ "memberId": "guid" }
```
Call this directly from Flutter with `http`/`dio`. No browser involved.

### `GET /connect/authorize`, `POST /connect/login`, `POST /connect/token` — the login flow
These three only make sense together, driven by an OAuth client library, not called
individually by your own code:

1. App opens `/connect/authorize?...&code_challenge=...` in the **system browser** (not
   an in-app WebView — AppAuth insists on this for security, and OpenIddict doesn't care
   either way as long as cookies persist for the request).
2. No session cookie yet → server renders a **plain server-side HTML login form**
   (email/password). The user types credentials into this page, not into your Flutter
   UI — your app never sees the raw password. This is the deliberate reason ROPC
   (password grant) was rejected in favor of Authorization Code + PKCE.
3. Form posts to `/connect/login`, which validates against the `Users` table and signs
   a short-lived login cookie.
4. Browser redirects back to `/connect/authorize` → cookie present → OpenIddict issues
   an authorization code → redirects to **your app's registered redirect URI**.
5. AppAuth intercepts that redirect, extracts the code, and automatically calls
   `POST /connect/token` (with the PKCE `code_verifier`) to exchange it for tokens.
6. You get back:
   ```json
   { "access_token": "...", "refresh_token": "...", "token_type": "Bearer", "expires_in": ... }
   ```

Store `access_token` and `refresh_token` in **`flutter_secure_storage`** (Keychain /
Keystore-backed), never in plain `SharedPreferences`.

### Every other endpoint (`/api/books`, `/api/members`, `/api/borrowings`, ...)
Require `Authorization: Bearer <access_token>`. No token → **401**. Wrong role/not the
owner → **403**.

---

## 4. Blocker: no mobile OAuth client is registered yet

`WebApplicationExtensions.ApplyMigrationsAndSeedDataAsync` currently seeds exactly one
OpenIddict client, `library-client`, with a single hardcoded redirect URI:
```
https://localhost:7282/swagger/oauth2-redirect.html
```
That's Swagger's callback page. **A mobile app cannot use it.** Before Flutter can
authenticate, you (or the backend) need to register a client — or a second client —
whose `RedirectUris` includes a mobile-appropriate URI, e.g. a custom scheme:
```
com.yourorg.library://oauthredirect
```
or an Android App Link / iOS Universal Link if you want to avoid the custom-scheme
interception ambiguity (recommended for production; custom schemes can be squatted by
other apps on Android). `flutter_appauth` needs this exact string as its `redirectUrl`,
and it must also be registered as an intent-filter (Android) / associated domain or
`CFBundleURLTypes` entry (iOS) in the Flutter project.

`ClientType` must stay `Public` (no secret) and `Requirements.Features.ProofKeyForCodeExchange`
must stay required — both already true for `library-client`, keep them for the mobile
client too.

---

## 5. Reachability & TLS — will bite you immediately in local dev

- **`localhost` in the API's launch profile is the host machine's localhost, not the
  emulator's.** Android emulator: use `10.0.2.2` instead of `localhost`. iOS simulator:
  `localhost` works (shares the host network). A **physical device** needs your machine's
  LAN IP, and the redirect URI / API base URL must be reachable from the phone's network.
- The dev HTTPS certificate (`https://localhost:7282`) is a self-signed ASP.NET Core dev
  cert. **Mobile OS HTTP clients will not trust it by default**, unlike your desktop browser
  which you likely trusted manually. Options: run the app against plain `http://` during
  local dev (port 5173, also in `launchSettings.json`), export/trust the dev cert on the
  device/emulator, or tunnel through something like `ngrok`/Tailscale with a real cert.

---

## 6. Token lifetime & restart instability (dev-only, but will confuse testing)

- **Signing/encryption certificates are ephemeral** (`AddDevelopmentSigningCertificate`/
  `AddDevelopmentEncryptionCertificate`), regenerated **every time the API restarts**.
  Every access/refresh token issued before a restart becomes unverifiable afterward.
  During development, expect the app to need a fresh login after every `dotnet run`
  restart — that's not a bug in your Flutter code, it's the dev cert. This must be
  replaced with a persisted certificate before any real deployment.
- Access/refresh token **lifetimes are not explicitly configured** in
  `BuilderExtensions.AddAuthenticationAndAuthorization` — the code relies on OpenIddict's
  framework defaults. Don't hardcode an assumed expiry in the Flutter app; read
  `expires_in` from the token response, and always implement the refresh-token flow
  (`AllowRefreshTokenFlow()` is enabled server-side) rather than assuming a fixed TTL.
- There is **no logout/revocation endpoint** yet (no `/connect/logout`, no token
  revocation wired up). "Logout" on the mobile side today just means: discard the stored
  tokens locally. The previously issued tokens remain technically valid server-side until
  they naturally expire.

---

## 7. What's inside the access token (claims)

Set explicitly in `AuthorizeEndpoints.MapAuthorizeEndpoints` → `/connect/login`:

| Claim | Present for | Notes |
|---|---|---|
| `sub` | everyone | the `User.Id` (not `Member.Id`) |
| `email` | everyone | |
| `role` | everyone | `"Admin"` or `"Member"` — use this to show/hide admin UI |
| `member_id` | Member-role users only | the linked `Member.Id` — **this is what "my books"/"my borrowings" screens should filter by**, not `sub` |

Nothing else is in the token (no `full_name`, etc.) — claims are opt-in in OpenIddict
(`SetDestinations`), and only these four are marked to go into the access token. If you
need the member's name/phone in the UI, fetch it via `GET /api/members/{id}` after login,
don't expect it in the token.

---

## 8. Authorization rules to expect (per-endpoint, already enforced)

Implemented as plain `if` checks in each endpoint (`AuthorizationHelper.IsAdmin` /
`IsOwnerOrAdmin`), not `[Authorize(Roles=...)]` attributes — so the rules are explicit
in `BookEndpoints.cs` / `MemberEndpoints.cs` / `BorrowingEndpoints.cs` if you need to
double check exact behavior:

- **Books**: anyone authenticated can read; only `Admin` can create/update/delete.
- **Members**: `Admin` can list all / create / delete any; a `Member` can only
  GET/PUT **their own** record (`member_id` claim must match the route's `{id}`).
- **Borrowings**: a `Member` can only borrow/view/return **their own** borrowings
  (checked against `member_id`); `Admin` can see and act on everyone's.

A **role/ownership failure returns `403`** with a JSON `ProblemDetails` body
(`{ type, title, status, detail, code, traceId }`) — consistent and easy to parse in
Flutter.

⚠️ **A missing/invalid/expired token returns a bare `401`**, *not* the same
`ProblemDetails` JSON shape — it's produced by ASP.NET Core's authentication middleware
before your request ever reaches application code or `ExceptionMiddleware`, so don't
assume every error response is JSON-parseable. Treat any `401` as "redirect to login /
try refresh token," regardless of body content.

---

## 9. Recommended Flutter-side setup

1. `flutter_appauth` for the Authorization Code + PKCE flow (handles system-browser
   launch, redirect capture, PKCE generation, and the token exchange call for you).
2. `flutter_secure_storage` for persisting `access_token` / `refresh_token`.
3. An `http`/`dio` interceptor that:
   - attaches `Authorization: Bearer <access_token>` to every API call,
   - on `401`, attempts a silent refresh via the refresh token, and only if that also
     fails, clears storage and routes to the login screen.
4. Decode the `role`/`member_id` claims client-side (e.g. `jwt_decoder` — the access
   token is a normal JWT, OpenIddict's default format, not an opaque reference token
   here) to drive UI (show admin screens, scope "my borrowings" queries), but **never
   trust the client-decoded claims for security-sensitive decisions** — the server
   re-checks everything on every request regardless.

---

## 10. Everything above is dev-environment reality, not production-ready

Carried over from the backend implementation notes — relevant to you as the mobile
integrator because these will need to change before a real release, and the redirect
URI / cert / hostname changes will directly affect the Flutter app's config:

- Seeded admin account (`admin@library.local` / `ChangeMe123!`) exists only in
  `Development` — must be rotated/removed for real deployments.
- No account recovery / email verification / password reset flow exists yet.
- No login rate-limiting or lockout — not mobile-specific, but worth knowing while
  testing.
- CORS is not configured — irrelevant for a native Flutter app (mobile HTTP clients
  don't enforce CORS), but relevant if you ever add a Flutter **web** build target.
