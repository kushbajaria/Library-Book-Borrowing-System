<div align="center">

# Library Book Borrowing System
### Libaverse Design Document
### AP, Kush, Evan

</div>
---

## Project Demo

https://youtu.be/c4OQpPhrhR4?si=EhYR1buzeeZleQbh 

---

## 1. System Overview

The Library Book Borrowing System is a RESTful backend API built with C#, ASP.NET Core 8, Entity Framework Core, and SQLite. It manages three core resources: Books, Members, and Borrow Records.

The system supports:
- Full CRUD operations for books and members
- Borrowing and returning books with availability tracking
- Viewing all borrow records and a member's borrow history

---

## 2. API Design — Endpoint List

**Books**
- `GET /api/books` — Get all books (cached) → 200
- `GET /api/books/{id}` — Get book by ID → 200, 404
- `POST /api/books` — Create a book → 201, 400
- `PUT /api/books/{id}` — Update a book → 200, 400, 404
- `DELETE /api/books/{id}` — Delete a book → 204, 404

**Members**
- `GET /api/members` — Get all members → 200
- `GET /api/members/{id}` — Get member by ID → 200, 404
- `POST /api/members` — Create a member → 201, 400
- `PUT /api/members/{id}` — Update a member → 200, 400, 404
- `DELETE /api/members/{id}` — Delete a member → 204, 404

**Borrowing**
- `POST /api/borrow` — Borrow a book → 201, 400, 404, 409
- `POST /api/return/{recordId}` — Return a book → 200, 400, 404
- `GET /api/borrow-records` — Get all borrow records → 200
- `GET /api/members/{memberId}/borrow-history` — Get member borrow history → 200

---

## 3. Architecture Explanation

The project uses a strict three-layer architecture:

**Controller Layer** — handles HTTP requests and basic input validation. Never contains business logic. Returns appropriate HTTP status codes and JSON responses.

**Service Layer** — contains all business rules and logic. Decides whether an operation is allowed. Converts database models to DTOs before returning data.

**Repository Layer** — the only layer that touches the database. Handles all queries and writes through Entity Framework Core. No business logic here.

A request flows: `Client → Controller → Service → Repository → SQLite Database`, and the result travels back up the same chain.

---

## 4. DTO Design

**Incoming (what the client sends):**
- `CreateBookRequest` — Title, Author, ISBN, TotalCopies (no Id, server generates it)
- `UpdateBookRequest` — Fields allowed when editing a book
- `CreateMemberRequest` — FullName and Email
- `UpdateMemberRequest` — FullName and Email
- `BorrowRequest` — BookId and MemberId

**Outgoing (what the server sends back):**
- `BookResponse` — Full book data including Id and AvailableCopies
- `MemberResponse` — Full member data including Id and MembershipDate
- `BorrowRecordResponse` — Full record including Status and ReturnDate
- `ErrorResponse` — Consistent error format: `{ "error": "message" }`
- `PagedResult<T>` — Generic wrapper for paginated list responses

---

## 5. Validation Strategy

Validation is applied at three levels:

**Controller** — required fields, data types, and basic format checks. Title cannot be empty, TotalCopies must be greater than zero, Email must contain @.

**Service** — business rules such as: book and member must exist before borrowing, AvailableCopies must be greater than zero, a member cannot borrow the same book twice, and a member cannot return a book they did not borrow.

**Database** — foreign key constraints and cascade deletes configured through Entity Framework Core ensure data integrity at the database level.

---

## 6. Error Handling Approach

All errors return: `{ "error": "message" }`

**Status codes used:**
- `200 OK` — Successful read or update
- `201 Created` — Successful creation, returns new resource
- `204 No Content` — Successful delete, empty body
- `400 Bad Request` — Invalid input or business rule violation
- `404 Not Found` — Resource does not exist
- `409 Conflict` — No available copies or duplicate borrow attempt

---

## 7. Concurrency Handling

**Problem:** Two members borrowing the last available copy simultaneously could both pass the availability check and result in AvailableCopies going below zero.

**Solution:** Two layers of protection are used. First, the service checks that AvailableCopies is greater than zero before allowing any write. Second, Entity Framework Core wraps all writes in database transactions and SQLite processes writes one at a time, so even if two requests pass the first check simultaneously, only one write completes first and the other fails safely.

**Result:** AvailableCopies never goes below zero. One request gets `201 Created`, the other gets `409 Conflict`.

---

## 8. Caching Strategy

**Why:** Without caching, every call to `GET /api/books` hits the database. Caching stores the result in memory so repeated requests are served instantly without a database query.

**What:** The `GET /api/books` endpoint is cached using `IMemoryCache` because it is the most frequently called read endpoint. Results are cached for 2 minutes.

**Invalidation:** The cache is cleared immediately whenever a book is created, updated, or deleted, ensuring clients never receive stale data.

---

## Member Contributions

**Antonio Calderon** — Repositories, Services, Caching, Design Document

**Kush Bajaria** — Models, Implementation design, Controllers, Database setup

**Evan** — DTOs, Program.cs, Project Demo Video, Testing
