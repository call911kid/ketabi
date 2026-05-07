# Ketabi

**Ketabi** is a peer-to-peer platform where users can list, exchange, and borrow books from each other. Designed initially for students and academic books, it aims to reduce financial strain and wasted resources by facilitating local book sharing and exchanges.

## Features (MVP)

- **Book Listing:** List books with title, description, condition, category, and transaction type (borrow/exchange).
- **Search & Filtering:** Browse available books by title, category, or location.
- **Borrow & Exchange Flows:** Request books for a specific duration or offer an exchange.
- **Request Management:** Book owners can review, accept, or reject incoming requests.
- **In-App Coordination:** Integrated chat (post-acceptance) to coordinate physical handoffs.
- **Transaction Management:** Confirm physical exchanges and track book availability statuses.

*(See [`docs/Ketabi_BRD.md`](docs/Ketabi_BRD.md) for full Business Requirements and Roadmap)*

## Tech Stack

- **Framework:** .NET 8.0 / ASP.NET Core MVC
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, Web)
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server (Configurable)
- **Libraries:** AutoMapper, FluentValidation, ASP.NET Core Identity
- **Frontend:** HTML5, CSS, Bootstrap, jQuery (with unobtrusive validation)

## Architecture Overview

The application follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability:

- **`Ketabi.Core` (Domain Layer):** Contains the core business domain entities (`BaseEntity`, etc.), repository interfaces (`IUnitOfWork`), and enums. It has no dependencies on other project layers.
- **`Ketabi.Application` (Application Layer):** Contains business logic, DTOs, AutoMapper profiles, and validation rules (FluentValidation). Depends only on the Core layer.
- **`Ketabi.Infrastructure` (Infrastructure Layer):** Implements data access using EF Core (`KetabiDbContext`), repository implementations, and external services. Depends on the Core layer.
- **`Ketabi.Web` (Presentation Layer):** The ASP.NET Core MVC project that serves as the application's entry point. It contains controllers, views, and view models, handling all user interactions. Depends on the Application and Infrastructure layers.

## Project Structure

```text
ketabi/
├── docs/
│   └── Ketabi_BRD.md             # Business Requirements Document
├── src/Ketabi/
│   ├── Ketabi.Core/              # Domain Entities and Interfaces
│   ├── Ketabi.Application/       # Application Services, DTOs, Mappings
│   ├── Ketabi.Infrastructure/    # EF Core Context, Repositories
│   └── Ketabi.Web/               # ASP.NET Core MVC Front-end
└── README.md                     # Project documentation
```

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- SQL Server (LocalDB, Express, or Developer edition)
- Visual Studio 2022, JetBrains Rider, or Visual Studio Code

### Setup & Run Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/call911kid/ketabi.git
   cd ketabi
   ```

2. **Configure the Database:**
   Update the `ConnectionStrings` in `src/Ketabi/Ketabi.Web/appsettings.Development.json` (or `appsettings.json`) to point to your local SQL Server instance.

3. **Apply Database Migrations:**
   If you are using **Visual Studio**, open the Package Manager Console and run:
   ```powershell
   Update-Database -Project Ketabi.Infrastructure -StartupProject Ketabi.Web
   ```
   
   If you are using the **.NET CLI** (e.g., in VS Code or terminal), run:
   ```bash
   cd src/Ketabi/Ketabi.Web
   dotnet ef database update --project ../Ketabi.Infrastructure --startup-project .
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```
   *Alternatively, open the solution folder or `src/Ketabi/Ketabi.slnx` in your IDE and run the `Ketabi.Web` project.*

5. Navigate to the local URL (e.g., `https://localhost:5001`) provided in your terminal or IDE console.
