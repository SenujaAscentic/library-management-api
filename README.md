# Library Management System API

## Overview

A RESTful Library Management System built using ASP.NET Core Minimal APIs, Entity Framework Core, and PostgreSQL.

The system allows librarians to manage books, members, and borrowing operations while enforcing business rules such as borrowing limits, book availability, and unique member registrations.

---

## Features

### Book Management

- Create a book
- Get all books
- Get book by ID
- Update a book
- Delete a book

### Member Management

- Create a member
- Get all members
- Get member by ID
- Update a member
- Delete a member

### Borrowing Management

- Borrow a book
- Return a book
- View borrowing history
- View borrowing history by member

---

## Technologies Used

- .NET 9
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL
- FluentValidation
- Swagger / OpenAPI
- xUnit
- Moq
- FluentAssertions
- Docker Compose

---

## Architecture

The project follows a layered architecture:

```text
Client
   ↓
Endpoints
   ↓
Services
   ↓
Repositories
   ↓
DbContext
   ↓
PostgreSQL
```

### Project Structure

```text
Library.Api
│
├── Application
│   ├── Interfaces
│   └── Services
│
├── Common
│   ├── Exceptions
│   └── Validation
│
├── Contracts
│   ├── Books
│   ├── Members
│   ├── Borrowings
│   └── Common
│
├── Domain
│   ├── Entities
│   └── Enums
│
├── Endpoints
│
├── Infrastructure
│   ├── Data
│   └── Repositories
│
├── Middleware
│
├── Validators
│
└── Program.cs
```

---

## Business Rules

### Books

- ISBN must be unique
- Published year cannot be greater than the current year
- Total copies must be greater than zero

### Members

- Email must be unique
- Email format must be valid
- Members can be activated or deactivated

### Borrowing

- Member must exist
- Book must exist
- Member must be active
- Book must have available copies
- A member can borrow a maximum of 3 active books
- Due date is automatically set to 14 days after borrowing
- Returned books cannot be returned again
- Available copies decrease when a book is borrowed
- Available copies increase when a book is returned

---

## API Endpoints

### Books

| Method | Endpoint |
|----------|----------|
| POST | `/api/books` |
| GET | `/api/books` |
| GET | `/api/books/{id}` |
| PUT | `/api/books/{id}` |
| DELETE | `/api/books/{id}` |

### Members

| Method | Endpoint |
|----------|----------|
| POST | `/api/members` |
| GET | `/api/members` |
| GET | `/api/members/{id}` |
| PUT | `/api/members/{id}` |
| DELETE | `/api/members/{id}` |

### Borrowings

| Method | Endpoint |
|----------|----------|
| POST | `/api/borrowings` |
| GET | `/api/borrowings` |
| GET | `/api/members/{memberId}/borrowings` |
| POST | `/api/borrowings/{id}/return` |

---

## Validation

The API uses FluentValidation for request validation.

### Validation Rules

#### Books

- Title is required
- Author is required
- ISBN is required
- Published year cannot be in the future
- Total copies must be greater than zero

#### Members

- Full name is required
- Email is required
- Email must be valid
- Phone number is required

#### Borrowings

- Book ID is required
- Member ID is required

### Validation Error Response

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "field": "Email",
      "message": "Email is required"
    }
  ]
}
```

---

## Error Handling

The API uses custom exceptions and global exception middleware to provide consistent error responses.

### Example Error Response

```json
{
  "statusCode": 404,
  "message": "Book not found",
  "traceId": "00-abc123..."
}
```

### Handled Error Cases

- Book not found
- Member not found
- Borrowing record not found
- ISBN already exists
- Email already exists
- Book is unavailable
- Member is inactive
- Member borrowing limit exceeded
- Book has already been returned

---

## Unit Testing

Unit tests were implemented for Borrowing Service business rules.

### Covered Scenarios

- Book not found
- Member not found
- Member inactive
- Book unavailable
- Borrowing limit exceeded
- Returning an already returned book
- Available copies decrease after borrowing
- Available copies increase after returning

---

## Database Setup

### Prerequisites

- .NET 9 SDK
- Docker Desktop
- PostgreSQL (or Docker)

---

### Start PostgreSQL

```bash
docker compose up -d
```

Verify containers are running:

```bash
docker ps
```

---

### Apply Migrations

```bash
dotnet ef database update
```

---

## Running the Application

Restore packages:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run
```

---

## Swagger

After running the application, Swagger UI is available at:

```text
https://localhost:<port>/swagger
```

Swagger can be used to test all API endpoints directly from the browser.

---

## Sample Data

### Create Sample Book

**POST** `/api/books`

```json
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "9780132350884",
  "publishedYear": 2008,
  "totalCopies": 5
}
```

### Create Sample Member

**POST** `/api/members`

```json
{
  "fullName": "John Doe",
  "email": "john@test.com",
  "phoneNumber": "0771234567"
}
```

### Borrow a Book

**POST** `/api/borrowings`

```json
{
  "bookId": "<book-id>",
  "memberId": "<member-id>"
}
```

### Return a Book

**POST** `/api/borrowings/{id}/return`

---

## Testing

Run all tests:

```bash
dotnet test
```

Expected output:

```text
Passed! All tests passed.
```

---

## Future Improvements

- Pagination for list endpoints
- Authentication and authorization
- Logging
- Dockerized API deployment
- Integration tests
- CQRS pattern
- Caching

---

## Author

Library Management System API developed as part of a backend engineering assessment using ASP.NET Core, Entity Framework Core, PostgreSQL, FluentValidation, and Minimal APIs.
