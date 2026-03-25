# 📚 Library Management API

A RESTful API for managing a library system — handling books, members, borrowing, returns, and automatic fine calculation. Built with ASP.NET Core 8 and Entity Framework Core, following clean architecture principles.

---

## ✨ Features

- 📖 Books management — add, update, delete, and search the catalogue
- 👥 Member management — register and manage library members
- 🔄 Borrow & return tracking — full lifecycle management of loans

- 🧪 Unit tested services with xUnit

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Object mapping | AutoMapper |
| Testing | xUnit|
| API docs | Swagger / OpenAPI |

---

## 🏗️ Architecture

```
HTTP Request
      │
      ▼
Controllers        ← Thin — validates input, returns responses
      │
      ▼
Services           ← Business logic (fine calculation, availability checks)
      │
      ▼
Repositories       ← EF Core data access
      │
      ▼
SQL Server         ← Books, Members, Loan
```

**Key design decisions:**
- Repository pattern separates data access from business logic
- AutoMapper keeps DTOs clean — domain models never leak to API responses
- Fine calculation is handled entirely in the service layer, not the database
- In-memory database used in tests — no SQL Server dependency for CI

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-gb/sql-server/) (or SQL Server Express)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/library-management-api.git
cd library-management-api
```

### 2. Configure the database

Update `LibraryManagement.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Run database migrations

```bash
cd LibraryManagement.Api
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
# API runs at https://localhost:5000
# Swagger UI at https://localhost:5000/swagger
```

---

## 📡 API Endpoints

### Books

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/books` | Get all books |
| `GET` | `/api/books/{id}` | Get book by ID |
| `GET` | `/api/books/search?query={term}` | Search books by title or author |
| `GET` | `/api/books/available` | Get available books |
| `POST` | `/api/books` | Add a new book |
| `PUT` | `/api/books/{id}` | Update book details |
| `DELETE` | `/api/books/{id}` | Remove a book |

### Members

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/members` | Get all members |
| `GET` | `/api/members/{id}` | Get member by ID |
| `POST` | `/api/members` | Register a new member |
| `PUT` | `/api/members/{id}` | Update member details |
| `DELETE` | `/api/members/{id}` | Remove a member |

### Loan

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/loans` | Borrow a book |
| `POST` | `/api/loans/{Id}/return` | Return a borrowed book |
| `GET` | `/api/loans/active` | Get all active loans |
| `GET` | `/api/loans/overdue` | Get all overdue loans |
| `GET` | `/api/members/{memberId}/loans` | Get borrow history for a member |

### Example — Borrow a book

```json
POST /api/loans
{  
  "bookId": 5,
  "memberId": 1,
  "durationInDays": 14
}
```

---

## 🧪 Running Tests

```bash
cd LibraryManagement.Tests
dotnet test --verbosity normal
```

Tests cover:
- Book availability checks before borrowing
- Fine calculation for various overdue scenarios
- Member validation on registration
- Return processing logic

---

## 🗄️ Database Schema

```
Books
  Id, Title, Author, ISBN, PublishedYear, isAvailable,  CreatedAt

Members
  Id, Name, Email, Phone, JoinedDate

Loans
  Id, BookId (FK), MemberId (FK), LoanDate, DueDate, ReturnDate
```

---

## 🔮 Planned Improvements

- [ ] JWT authentication with Admin / Member roles
- [ ] Email notifications for overdue books
- [ ] Applying fine for overdue books
- [ ] CI/CD pipeline with GitHub Actions + Azure deployment
- [ ] Redis caching for frequently searched books
- [ ] Pagination on all list endpoints

---

## 📄 Licence

This project is open source under the [MIT Licence](LICENSE).
