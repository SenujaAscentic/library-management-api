# Clean Architecture & Entities - Study Notes

## Understanding Clean Architecture & Entities

Clean Architecture can feel like a lot of overhead at first, but it comes down to one core rule:

> **Organize your code so that changing your technology choices (like swapping databases or UI frameworks) won't break your core business logic.**

---

# 1. What is an Entity?

In software development, specifically in **Domain-Driven Design (DDD)**, an **Entity** represents a real-world object or concept in your business system.

## The Key Rule: Defined by Identity, Not Properties

An entity is unique because of its **Identity (ID)**, not its attributes.

### Analogy: Passport vs Coffee Cup

#### Value Object (Coffee Cup)

Two identical coffee cups from the same store with the same size and color are interchangeable.

If you swap them, nobody cares.

This is called a **Value Object**.

#### Entity (Person)

Two twin siblings might share the exact same:

- Birthday
- Last name
- Hair color
- Address

However, they have different:

- Passport Numbers
- Social Security Numbers

Therefore, they are two separate people.

This is called an **Entity**.

---

## Example Entity in C#

```csharp
public class Book : BaseEntity
{
    public string Title { get; set; }
    public string Author { get; set; }
    public decimal Price { get; set; }
}
```

Even if two `Book` objects have the same:

- Title
- Author
- Price

If their `Id` values are different, the system treats them as two separate entities.

---

# 2. Clean Architecture Explained Simply

Instead of placing everything inside one large project, Clean Architecture separates responsibilities into layers.

## The Golden Rule

> Dependencies only point inward.

The core business logic should never know about:

- Databases
- APIs
- Frameworks
- UI technologies

---

# The Four Main Layers

```text
┌───────────────────────────┐
│ Presentation / Web API    │
└─────────────▲─────────────┘
              │
┌─────────────┴─────────────┐
│ Application Layer         │
└─────────────▲─────────────┘
              │
┌─────────────┴─────────────┐
│ Domain Layer              │
└─────────────┬─────────────┘
              │
┌─────────────▼─────────────┐
│ Infrastructure Layer      │
└───────────────────────────┘
```

---

# 🟢 1. Domain Layer (The Core)

## What it is

The heart of the application.

## What lives here

- Entities
- Value Objects
- Domain Events
- Business Rules

### Example Business Rules

- A user cannot borrow more than 5 books.
- A member cannot reserve the same book twice.
- A book cannot be borrowed if it is already borrowed.

## Key Characteristic

The Domain Layer has:

- No database knowledge
- No API knowledge
- No framework dependencies

It is pure C# code.

### Example

```csharp
public class Book : BaseEntity
{
    public string Title { get; private set; }
    public bool IsBorrowed { get; private set; }

    public void Borrow()
    {
        if (IsBorrowed)
            throw new Exception("Book already borrowed");

        IsBorrowed = true;
    }
}
```

---

# 🔵 2. Application Layer (Business Logic / Use Cases)

## What it is

Defines what the system can do.

These are called **Use Cases**.

## What lives here

### Commands

Actions that change data.

Examples:

- CreateBookCommand
- RegisterUserCommand
- BorrowBookCommand

### Queries

Actions that retrieve data.

Examples:

- GetBookByIdQuery
- GetAllBooksQuery
- GetMemberByIdQuery

---

## Common Tools

### MediatR

MediatR routes requests to their handlers.

Without MediatR:

```text
Controller
   ↓
Service
   ↓
Repository
```

With MediatR:

```text
Controller
   ↓
MediatR
   ↓
Command/Query Handler
```

This keeps controllers clean and simple.

---

### FluentValidation

Validates incoming data before processing.

Example:

```csharp
public class CreateBookValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
```

---

# 🟡 3. Infrastructure Layer (The External World)

## What it is

Handles all external technologies and integrations.

## What lives here

### Database

- Entity Framework Core
- DbContext
- Migrations

### External Services

- Stripe
- Twilio
- Email Services

### File Storage

- Local storage
- Azure Blob Storage
- AWS S3

---

## Example

Application Layer defines an interface:

```csharp
public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body);
}
```

Infrastructure provides the implementation:

```csharp
public class EmailService : IEmailService
{
    public Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        // Send email using SMTP
    }
}
```

This allows you to change the email provider without modifying business logic.

---

# 🔴 4. Presentation & Web API Layer (Entry Point)

## What it is

The front door of the application.

It receives requests from:

- Web Applications
- Mobile Apps
- Third-party Systems

---

## What lives here

### Controllers

```csharp
[HttpPost]
public async Task<IActionResult> CreateBook(
    CreateBookCommand command)
{
    await _mediator.Send(command);

    return Ok();
}
```

### API Endpoints

- GET /books
- POST /books
- PUT /books/{id}
- DELETE /books/{id}

### Swagger / OpenAPI

Provides API documentation.

### Dependency Injection

Registers services and dependencies.

---

## Responsibility

1. Receive HTTP request.
2. Pass request to Application Layer.
3. Return HTTP response.

It should contain minimal business logic.

---

# Request Flow Example: Create a Book

Imagine a user wants to create a new book.

---

## Step 1: User Sends Request

```http
POST /books
```

Request Body:

```json
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "price": 45.99
}
```

---

## Step 2: Presentation Layer

Controller receives request.

```csharp
await _mediator.Send(command);
```

---

## Step 3: Application Layer

`CreateBookCommandHandler` executes.

```csharp
public async Task Handle(
    CreateBookCommand request,
    CancellationToken cancellationToken)
{
    var book = new Book(
        request.Title,
        request.Author,
        request.Price);

    await _repository.AddAsync(book);
}
```

Business logic is executed here.

---

## Step 4: Domain Layer

Creates the Book entity.

```csharp
var book = new Book(
    title,
    author,
    price);
```

Business rules are enforced.

---

## Step 5: Infrastructure Layer

Repository saves data to SQL Server.

```csharp
_context.Books.Add(book);
await _context.SaveChangesAsync();
```

---

## Step 6: Response Returned

```http
201 Created
```

The user receives confirmation that the book was created.

---

# Layer Dependency Diagram

```text
Presentation (Web API)
        │
        ▼
Application
        │
        ▼
Domain

Infrastructure
     ▲
     │
Application
```

### Important

- Domain does not know Infrastructure exists.
- Domain does not know SQL Server exists.
- Domain does not know ASP.NET exists.
- Infrastructure depends on Application contracts.
- Presentation depends on Application use cases.

---

# Summary

| Layer | Responsibility | Analogy |
|---------|----------------|----------|
| Domain | Core concepts and business rules | Rules of chess |
| Application | Use cases and flow control | Chess player deciding a move |
| Infrastructure | Databases, APIs, file systems | Physical chessboard and clock |
| Presentation | Web APIs, UI, HTTP endpoints | TV camera broadcasting the match |

---

# Key Takeaways

1. **Entities are defined by identity, not properties.**
2. **Domain Layer contains business rules and entities.**
3. **Application Layer contains use cases (Commands & Queries).**
4. **Infrastructure Layer handles databases and external services.**
5. **Presentation Layer handles HTTP requests and responses.**
6. **Dependencies always point inward.**
7. **The Domain Layer should remain independent of frameworks and technologies.**

Following these principles makes applications easier to:

- Maintain
- Test
- Scale
- Modify
- Replace technologies without affecting business logic