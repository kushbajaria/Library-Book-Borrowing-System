Library Book Borrowing System
A RESTful backend API built with C#, ASP.NET Core 8, Entity Framework Core, and SQLite for managing library books, members, and borrowing activity.

Table of Contents

System Overview
API Endpoints
Architecture
DTOs
Validation
Error Handling
Concurrency
Caching
Team


System Overview
This is a backend only API with no frontend. It can be tested through Swagger UI at http://localhost:{PORT}/swagger after running the project.
The system manages three resources:

Books — library inventory with copy tracking
Members — registered library users
Borrow Records — tracks who borrowed what and when


API Endpoints
Books
MethodRouteDescriptionStatus CodesGET/api/booksGet all books (cached)200GET/api/books/{id}Get book by ID200, 404POST/api/booksCreate a book201, 400PUT/api/books/{id}Update a book200, 400, 404DELETE/api/books/{id}Delete a book204, 404
Members
MethodRouteDescriptionStatus CodesGET/api/membersGet all members200GET/api/members/{id}Get member by ID200, 404POST/api/membersCreate a member201, 400PUT/api/members/{id}Update a member200, 400, 404DELETE/api/members/{id}Delete a member204, 404
Borrowing
MethodRouteDescriptionStatus CodesPOST/api/borrowBorrow a book201, 400, 404, 409POST/api/return/{recordId}Return a book200, 400, 404GET/api/borrow recordsGet all borrow records200GET/api/members/{memberId}/borrow historyGet member borrow history200

Architecture
Three layer architecture with strict separation of concerns:
Client Request
     ↓
Controller   → validates input, handles HTTP
     ↓
Service      → enforces business rules
     ↓
Repository   → database queries only
     ↓
SQLite Database
Each layer only talks to the layer directly next to it. Controllers never touch the database. Repositories never make business decisions.

DTOs
DTOs separate what is stored in the database from what the API exposes. Clients never send an Id — the server always generates it.
DTODirectionPurposeCreateBookRequestIncomingTitle, Author, ISBN, TotalCopiesUpdateBookRequestIncomingFields allowed when editing a bookBookResponseOutgoingFull book data including AvailableCopiesCreateMemberRequestIncomingFullName and EmailMemberResponseOutgoingFull member data including MembershipDateBorrowRequestIncomingBookId and MemberIdBorrowRecordResponseOutgoingFull record with Status and ReturnDateErrorResponseOutgoing{ "error": "message" }

Validation
Controller level — required fields, basic format checks:

Title, Author, ISBN cannot be empty
TotalCopies must be greater than 0
Email must contain @

Service level — business rules:

Book and member must exist before borrowing
AvailableCopies must be greater than 0
Cannot borrow the same book twice without returning
Cannot return a book you did not borrow

Database level — foreign key constraints and cascade deletes via Entity Framework Core

Error Handling
All errors return a consistent format:
json{
  "error": "Message here"
}
ScenarioStatus CodeResource not found404 Not FoundInvalid input400 Bad RequestNo copies or duplicate borrow409 ConflictSuccessful creation201 CreatedSuccessful read or update200 OKSuccessful delete204 No Content
Stack traces are never exposed to the client.

Concurrency
Problem: Two members borrowing the last available copy simultaneously could both pass the availability check and drive AvailableCopies below zero.
Solution: Two layers of protection:

Service checks AvailableCopies > 0 before any write
EF Core wraps all writes in database transactions and SQLite serializes writes, so only one completes first

Result: AvailableCopies never goes below zero. One request gets 201 Created, the other gets 409 Conflict.

Caching
GET /api/books is cached using IMemoryCache because it is the most frequently read endpoint.
BehaviorDetailCache duration2 minutesFirst requestFetches from database, stores in memorySubsequent requestsServed from memory, no database hitInvalidationCache cleared on any create, update, or delete
Registered in Program.cs with builder.Services.AddMemoryCache() and injected into BookService.

Getting Started
bash# Install EF tools if not already installed
dotnet tool install   global dotnet ef

# Restore packages
dotnet restore

# Create the database
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the project
dotnet run
Open your browser at http://localhost:{PORT}/swagger to test all endpoints.

#Member Contributions:

Antonio Calderon: Repositories, Services, Caching, Design Document
Kush BajariaModels: Implementation design, Controllers, Database setup 
Evan:Controllers: DTOs, Program.cs, Testing