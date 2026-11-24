
# Library Management Engine

A C# console-based Library Management System designed to demonstrate Object-Oriented Programming (OOP), clean code organisation, interfaces, dependency injection, and separation of concerns.

## Project Overview

The Library Management Engine simulates the core operations of a library. It allows library staff to manage books, physical book copies, members, borrowing, returns, reservations, and financial transactions.

The application uses in-memory repositories to manage data during execution.

## Features

### Book Management
- Register new books.
- Prevent duplicate ISBN registration.
- View all registered books.
- Manage multiple physical copies of a book.
- Track copy statuses: Available, Borrowed, Reserved, Lost, and Damaged.

### Member Management
- Register library members.
- Support Standard and Student membership categories.
- Prevent duplicate membership numbers.
- View registered members.
- Suspend and reinstate members through the domain model.

### Borrowing Management
- Borrow available book copies.
- Enforce borrowing limits based on membership category.
- Calculate loan due dates.
- Return borrowed books.
- Calculate overdue fines.
- Report lost books and apply replacement charges.

### Reservation Management
- Reserve books when no copies are available.
- Prevent duplicate active reservations by the same member.
- Cancel reservations.
- View reservation queues.

### Financial Management
- Record fine payments.
- Support partial and full payments.
- Prevent overpayments.
- Track outstanding balances.

## Technologies Used

- C#
- .NET 9
- .NET Console Application
- xUnit
- Git and GitHub

## Object-Oriented Programming Concepts

This project demonstrates:

- **Encapsulation:** Domain entities protect their state through private setters and controlled methods.
- **Abstraction:** Interfaces define contracts for repositories and services.
- **Inheritance and polymorphism:** Borrowing policies provide different rules for Standard and Student members.
- **Composition:** Services collaborate through injected dependencies.
- **Separation of Concerns:** Domain models, repositories, services, and user interface responsibilities are separated.

## Project Structure

```text
LibraryManagementEngine/
│
├── src/
│   └── LibraryManagement/
│       ├── Domain/
│       │   ├── Entities/
│       │   └── Enums/
│       │
│       ├── Policies/
│       │
│       ├── Repositories/
│       │   ├── Interfaces/
│       │   └── Implementations/
│       │
│       ├── Services/
│       │
│       ├── UserInterface/
│       │
│       └── Program.cs
│
├── tests/
│   └── LibraryManagement.Tests/
│
├── LibraryManagement.sln
└── README.md
```

## Prerequisites

- .NET 9 SDK
- Visual Studio Code or another C# compatible IDE
- Git

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/jacobOpintan/LibraryManagementEngine.git
```

### 2. Navigate into the project

```bash
cd LibraryManagementEngine
```

### 3. Build the solution

```bash
dotnet build
```

### 4. Run the application

```bash
dotnet run --project src/LibraryManagement
```

### 5. Run automated tests

```bash
dotnet test
```

## Data Storage

The application currently uses in-memory repositories.

This means:
- Data is stored in application memory.
- Data is available while the application is running.
- Data is lost when the application exits.
- No external database is required.

## Limitations and Future Improvements

Potential future improvements include:

- Persistent database storage using SQL Server and Entity Framework Core.
- A graphical user interface or web-based interface.
- Improved reporting and administrative dashboards.
- More comprehensive reservation fulfilment and notifications.
- User authentication and role-based access control.

## Testing

The project includes automated unit tests using xUnit.

Tests cover core domain behaviour, borrowing policies, repository operations, borrowing and returns, financial transactions, lost-book handling, and reservations.

Run all tests using:

```bash
dotnet test
```

## Author

Developed as a C# learning and portfolio project to demonstrate practical software development, object-oriented design, and application architecture.
