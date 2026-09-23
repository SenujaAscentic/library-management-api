# Authentication & Authorization — What We Built and Why

## The goal

Implement real OAuth2/OIDC authentication (not just hand-rolled JWT issuance) with
role- and ownership-based authorization matching `rbac.md`, specifically designed to
support a **future native mobile client** as the primary consumer — not just Swagger.

---

## Key decisions made, and the reasoning behind each

| Decision | Chosen | Why |
|---|---|---|
| Authorization Server | **OpenIddict** (not Keycloak, not Duende IdentityServer) | Free with no license conditions (unlike Duende); stays inside our own .NET solution as real, readable C# (unlike Keycloak, an external black-box service) — matches this project's goal of understanding every piece, not just integrating one. |
| Grant type | **Authorization Code + PKCE** (not Resource Owner Password Credentials) | PKCE exists specifically for public clients like native mobile apps, which can't hold a secret and shouldn't collect the user's raw password themselves. ROPC is deprecated in OAuth 2.1 and would need to be rebuilt correctly once the mobile app is real — building it right the first time avoids that rework. |
| Identity storage | **Separate `User` entity**, distinct from `Member` | `User` is a security/login concept (email, password hash, role); `Member` is a library-patron business concept. Conflating them would force every `Member` operation to think about credentials, and give Admins (who don't borrow books) an awkward pretend-`Member` identity. `User.MemberId` (nullable) links the two only where relevant. |
| Ownership checks (`rbac.md`'s "own id/own memberId only" rows) | **Explicit inline comparison inside each endpoint** (not a custom `IAuthorizationHandler`) | Matches this project's existing style of explicitness (Minimal APIs over Controllers, hand-rolled `Result<T>` over heavier libraries) — visible logic over framework ceremony. *(Not yet implemented — see "What's left" below.)* |
| First-Admin bootstrap | **Seeded automatically on startup**, `Development` only | Solves the chicken-and-egg problem (every admin-only endpoint needs an existing admin). Seeded credentials: `admin@library.local` / `ChangeMe123!` — must be rotated/removed before any real deployment. |

---

## What actually got built, by layer

### Domain
- `User` entity (`Id`, `Email`, `PasswordHash`, `Role`, nullable `MemberId`), `sealed`,
  private setters, `Create(...)` factory enforcing: valid email format, non-empty
  password hash, and the cross-field invariant that an `Admin` must have no
  `MemberId` while a `Member`-role user must have one.
- `UserRole` enum (`Admin`, `Member`).

### Application
- `IUserRepository` abstraction (`GetByEmailAsync`, `AddAsync` — intentionally
  minimal, no `Update`/`Delete` yet since nothing needs them).
- `Features/Auth/Login/LoginCommand` + `LoginCommandHandler` — checks credentials
  via `PasswordHasher<User>.VerifyHashedPassword`, returns the raw `User` entity
  (a deliberate, narrow exception to the "handlers return DTOs" rule, since nothing
  here is serialized directly to a client). Both "no such user" and "wrong password"
  throw the identical `invalid_credentials` error, closing off a user-enumeration
  vulnerability.

### Infrastructure
- `UserRepository` implementing `IUserRepository`.
- `LibraryDbContext`: `DbSet<User>`, unique index on `User.Email`,
  `modelBuilder.UseOpenIddict()` (adds OpenIddict's own internal tables — client
  applications, issued tokens, authorizations, scopes).
- A migration covering both the new `Users` table and OpenIddict's schema.

### Api
- **`BuilderExtensions.cs`** / **`WebApplicationExtensions.cs`** — `Program.cs` was
  split into named extension methods (`AddPersistence`, `AddApplicationLayer`,
  `AddAuthenticationAndAuthorization`, `AddApiDocumentation`,
  `ApplyMigrationsAndSeedDataAsync`, `MapHealthCheckEndpoints`) purely for
  readability, since auth configuration alone was pushing `Program.cs` past a
  reasonable length.
- **OpenIddict server configuration**: Authorization Code flow + mandatory PKCE +
  refresh token flow, scopes (`openid`, `profile`, `library_api`), development
  signing/encryption certificates (⚠️ dev-only, see limitations), passthrough
  enabled so we can write our own endpoint logic.
- **Cookie authentication** — a separate, short-lived mechanism from the OAuth2
  tokens, used only to remember "this browser is logged in" for the few requests
  it takes to complete the redirect dance.
- **Client + first-Admin seeding**, `Development`-only: registers a `Public` OAuth2
  client (`library-client`, PKCE required) representing both Swagger (now) and the
  future mobile app (later); seeds the first Admin user if none exists.
- **Swagger OAuth2 wiring** — `AddSecurityDefinition`/`AddSecurityRequirement`
  (using the newer `Microsoft.OpenApi` v2 / Swashbuckle v10 API shape — the
  `Microsoft.OpenApi.Models` namespace and `OpenApiSecurityScheme.Reference`
  property were both removed upstream during this build), `OAuthClientId`,
  `OAuthUsePkce()` — this is what makes Swagger's "Authorize" button perform the
  real flow.
- **`AuthorizeEndpoints.cs`**:
  - `GET /connect/authorize` — checks for the login cookie; if absent, renders a
    plain HTML login form; if present, hands off to OpenIddict to issue the
    authorization code.
  - `POST /connect/login` — calls `LoginCommand` via MediatR, builds claims
    (`sub`, `email`, `role`, and a custom `member_id` claim), explicitly marks
    each claim's destination as the access token (`SetDestinations` — claims are
    *not* included in tokens by default, a deliberate OpenIddict security choice),
    signs in via cookie, redirects back into the OAuth flow.
  - `POST /connect/token` — reads back the claims OpenIddict already associated
    with the authorization code (or refresh token) and re-signs-in through
    OpenIddict's own scheme, which is what actually triggers token issuance. This
    endpoint deliberately does *not* re-check credentials — that already happened
    in `/connect/login`; OpenIddict remembers the result.

---

## The full runtime flow (as it works right now, via Swagger)

1. Click **Authorize** in Swagger → Swagger redirects the browser to
   `/connect/authorize`, including a PKCE `code_challenge` it generated itself.
2. No login cookie yet → our endpoint renders the HTML login form.
3. You submit email/password → `POST /connect/login` → `LoginCommand` validates
   against the `Users` table → claims built and signed into a cookie.
4. Browser redirects back to `/connect/authorize` → cookie now present → OpenIddict
   issues an authorization code, redirects to Swagger's OAuth callback page.
5. Swagger automatically calls `POST /connect/token` with the code and its PKCE
   `code_verifier` → our handler reads back the claims OpenIddict already tied to
   that code → OpenIddict verifies PKCE, mints a real, signed access token (and
   refresh token) → returns them as JSON.
6. Swagger attaches `Authorization: Bearer <token>` to every subsequent request
   automatically.

For the future mobile app, steps 1–4 happen in a **secure system browser view**
(`ASWebAuthenticationSession`/Custom Tabs), not a native form — the app never sees
the password. Step 5 is a direct app-to-server call. See the mobile integration
notes discussed separately for the full breakdown.

---

## What's left (not yet built)

1. **Applying `[Authorize]`/role checks to the actual 14 business endpoints**, per
   `rbac.md`'s table — nothing is currently protected; every endpoint is still
   publicly accessible regardless of the auth work above.
2. **Ownership checks** (comparing the caller's `member_id` claim against route
   parameters) for the "own id/own memberId only" rows.
3. **A `POST /api/auth/register` endpoint** for new Members to create accounts
   (everything so far only covers logging in with an existing account — the seeded
   Admin is the only account that currently exists).
4. **Full end-to-end testing** of the login flow itself, before layering
   authorization on top.

---

## Known limitations / deliberate trade-offs

- **Development-only signing/encryption certificates** (`AddDevelopmentEncryptionCertificate`/`AddDevelopmentSigningCertificate`) — ephemeral, regenerated on
  restart, fine for local dev, **not appropriate for any real deployment**.
- **Hardcoded redirect URI** (`https://localhost:7282/swagger/oauth2-redirect.html`)
  — tied to a fixed port; only works when running `Library.Api` standalone (not via
  `Library.AppHost`, whose port is dynamic each run). Needs a more flexible solution
  before this matters for real use.
- **Seeded Admin credentials are a known, fixed value** (`ChangeMe123!`) — must be
  rotated before any non-local use.
- **No account-recovery, email-verification, or password-reset flow** — out of
  scope for now.
