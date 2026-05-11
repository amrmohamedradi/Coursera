<div align="center">

# 🎓 Coursera API (Backend)

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=nuget)](https://docs.microsoft.com/en-us/ef/core/)
[![MediatR](https://img.shields.io/badge/MediatR-14.0-blue?style=for-the-badge)](https://github.com/jbogard/MediatR)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server/)
[![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://xunit.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

_A robust, scalable, and secure ASP.NET Core Web API backend for a learning management platform._

[Explore the API](#-api-reference) · [Live Demo](http://bywayapi.runasp.net/docs) · [Full API Docs](API_DOCUMENTATION.md) · [Report Bug](#-contact) · [Request Feature](#-contact)

</div>

---

## 📖 Overview

This repository contains the backend infrastructure for a Coursera-like online learning platform. Built with **.NET 8** following **Clean Architecture** principles, it provides a solid foundation for managing courses, instructors, categories, user carts, and a secure checkout process. It leverages **CQRS** with **MediatR** to keep the application logic organized and maintainable.

> 🌐 **Live API:** [`http://bywayapi.runasp.net`](http://bywayapi.runasp.net) — Swagger docs available at [`/docs`](http://bywayapi.runasp.net/docs)

### ✨ Key Features

- **🔐 Robust Authentication & Authorization**: ASP.NET Core Identity paired with JWT (Access & Refresh tokens) and Role-Based Access Control (`Admin` vs `User`).
- **🏛️ Clean Architecture**: Strict separation of concerns (`Api`, `Application`, `Domain`, `Infrastructure`).
- **📨 CQRS Pattern**: Implementation using `MediatR` for predictable and decoupled request handling.
- **✅ Input Validation**: `FluentValidation` integrated into the request pipeline for clean, declarative validation rules.
- **📚 Comprehensive Course Management**: Full CRUD capabilities for Courses, Categories, and Instructors.
- **🛒 E-Commerce Capabilities**: Shopping cart management and secure checkout operations.
- **🤖 Content-Based Recommendations**: ML-powered similar course suggestions based on content similarity.
- **📊 Admin Dashboard**: Aggregated statistics endpoint for platform oversight.
- **📄 Interactive API Documentation**: Swagger (OpenAPI) UI integrated out-of-the-box at `/docs`.
- **🛡️ Resilience**: Global exception handling middleware ensuring consistent JSON error responses.
- **🌐 CORS Support**: Fully configured Cross-Origin Resource Sharing for frontend integration.
- **🧪 Comprehensive Tests**: Unit and integration tests using xUnit, Moq, and FluentAssertions.

---

## 🏗️ Architecture & Project Structure

The solution follows **Clean Architecture** and is divided into distinct layers:

```
Coursera/
├── Coursera.Api/                    # 🎯 Presentation Layer
│   ├── Controllers/
│   │   ├── AuthController.cs        # Registration, Login, Token Refresh
│   │   ├── CartController.cs        # Shopping Cart (Get, Add, Remove)
│   │   ├── CategoryController.cs    # Category CRUD
│   │   ├── CourseController.cs      # Course CRUD + Pagination & Search
│   │   ├── DashboardController.cs   # Admin statistics
│   │   ├── GetSimilarCoursesController.cs  # ML Recommendations
│   │   ├── HomeController.cs        # Top Courses, Categories, Instructors
│   │   ├── InstructorController.cs  # Instructor CRUD
│   │   └── OrderController.cs       # Checkout & Payment
│   ├── Middlewares/
│   │   └── ExceptionMiddleware.cs   # Global exception handler
│   ├── Program.cs                   # Composition root & startup
│   └── appsettings.json
│
├── Coursera.Application/            # ⚙️ Business Logic Layer
│   ├── Common/
│   │   ├── Constans/Roles.cs        # Role constants (Admin, User)
│   │   ├── DTOs/                    # Data Transfer Objects
│   │   ├── Exceptions/              # Custom exceptions (NotFound, Validation, Unauthorized)
│   │   ├── Interfaces/              # IApplicationDbContext, IAuthService
│   │   └── Models/                  # ApiResponse<T>, PaginatedList, JwtSettings, Request models
│   ├── Features/                    # CQRS organized by feature
│   │   ├── Auth/                    # Register, Login, Refresh, ExternalLogin
│   │   ├── Carts/                   # Commands & Queries
│   │   ├── Categories/              # Commands & Queries
│   │   ├── Courses/                 # Commands & Queries + GetSimilarCourses
│   │   ├── Dashboard/               # Queries
│   │   ├── Home/                    # GetTopCourses, GetTopCategories, GetTopInstructors
│   │   ├── Instructors/             # Commands & Queries
│   │   └── Orders/                  # Checkout Command
│   ├── Interfaces/IJwtService.cs
│   └── DependencyInjection.cs       # MediatR & FluentValidation registration
│
├── Coursera.Domain/                 # 🌍 Enterprise Logic Layer
│   ├── Common/BaseEntity.cs         # Base entity with shared properties
│   ├── Entities/
│   │   ├── Cart.cs, CartItem.cs
│   │   ├── Category.cs
│   │   ├── Course.cs
│   │   ├── Instructor.cs
│   │   ├── Order.cs, OrderItem.cs
│   │   └── RefreshToken.cs
│   └── Enums/
│       ├── Level.cs                 # AllLevel, Beginner, Intermediate, Expert
│       └── JobTitle.cs              # FullStackDeveloper, BackEndDeveloper, etc.
│
├── Coursera.Infrastructure/         # 🧱 External Concerns Layer
│   ├── Data/
│   │   ├── ApplicationDbContext.cs  # EF Core DbContext (IdentityDbContext)
│   │   └── Configurations/          # Fluent API entity configurations (8 configs)
│   ├── Identity/
│   │   ├── ApplicationUser.cs       # Extended IdentityUser<Guid>
│   │   └── RoleSeeder.cs            # Auto-seeds Admin & User roles + default admin
│   ├── Migrations/                  # EF Core migrations
│   ├── Service/
│   │   ├── AuthService.cs           # Authentication logic
│   │   └── JwtService.cs            # JWT token generation
│   └── DependencyInjection.cs       # DbContext, Identity, Services registration
│
├── Coursera.Tests/                  # 🧪 Test Layer
│   ├── Application/
│   │   ├── Auth/                    # LoginHandlerTests, RegisterHandlerTests
│   │   ├── Courses/                 # GetCourseQueryHandlerTests
│   │   └── Orders/                  # CheckoutHandlerTests
│   ├── Domain/
│   │   ├── CartTests.cs
│   │   └── OrderTests.cs
│   └── Infrastructure/
│       ├── AuthServiceTests.cs
│       └── JwtServiceTests.cs
│
├── Coursera.slnx                    # Solution file
├── API_DOCUMENTATION.md             # Complete API reference
└── README.md
```

---

## 🚀 Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or Developer edition)
- Your favorite IDE (Visual Studio 2022, Rider, or VS Code)

### 1. Clone the repository

```bash
git clone https://github.com/amrmohamedradi/Coursera.git
cd Coursera
```

### 2. Configuration Setup

Configure your appsettings. Navigate to `Coursera.Api/appsettings.json` and update the database connection string and JWT configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CourseraDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"
  },
  "JWT": {
    "Key": "YourSuperSecretKeyThatMustBeLongEnoughForHmacSha256!",
    "Issuer": "CourseraApi",
    "Audience": "CourseraClient",
    "DurationInHours": 2,
    "RefreshTokenDurationInDays": 7
  }
}
```

### 3. Database Migration & Seeding

Entity Framework Core is used for data access. Apply the migrations to create your database schema.

From the repository root, run:

```bash
dotnet ef database update --project Coursera.Infrastructure --startup-project Coursera.Api
```

> **💡 Note:** Roles (`Admin`, `User`) and a default admin user (`admin@coursera.com` / `Admin@Admin0`) are seeded automatically during application startup via `RoleSeeder.cs`.

### 4. Run the API

```bash
cd Coursera.Api
dotnet run
```

_Your API is now running! Navigate to `https://localhost:<port>/docs` to explore the endpoints via Swagger UI._

### 5. Run Tests

```bash
dotnet test
```

---

## 📡 API Reference

Explore the robust endpoints available in the system. All endpoints return a standardized `ApiResponse<T>` wrapper.

```json
{
  "success": true,
  "message": null,
  "data": { ... }
}
```

> 📘 For complete API documentation with request/response examples, see **[API_DOCUMENTATION.md](API_DOCUMENTATION.md)**

### 🔑 Authentication (`/api/auth`)

| Method | Endpoint    | Description                                     | Auth |
| ------ | ----------- | ----------------------------------------------- | ---- |
| `POST` | `/register` | Register a new user                             | ❌   |
| `POST` | `/login`    | Authenticate user & receive JWT                 | ❌   |
| `POST` | `/refresh`  | Obtain a new access token using a refresh token | ❌   |

### 📚 Courses (`/api/course`)

| Method   | Endpoint | Description                                  | Auth       |
| -------- | -------- | -------------------------------------------- | ---------- |
| `GET`    | `/`      | Retrieve paginated courses (search, paging)  | ❌         |
| `GET`    | `/{id}`  | Retrieve specific course details             | ❌         |
| `POST`   | `/`      | Create a new course                          | 🛡️ `Admin` |
| `PUT`    | `/{id}`  | Update an existing course                    | 🛡️ `Admin` |
| `DELETE` | `/{id}`  | Remove a course                              | 🛡️ `Admin` |

### 🏷️ Categories (`/api/category`)

| Method   | Endpoint | Description               | Auth       |
| -------- | -------- | ------------------------- | ---------- |
| `GET`    | `/`      | Retrieve all categories   | ❌         |
| `GET`    | `/{id}`  | Retrieve category details | ❌         |
| `POST`   | `/`      | Create a new category     | 🛡️ `Admin` |
| `PUT`    | `/{id}`  | Update category           | 🛡️ `Admin` |
| `DELETE` | `/{id}`  | Remove a category         | 🛡️ `Admin` |

### 🧑‍🏫 Instructors (`/api/instructor`)

| Method   | Endpoint | Description                         | Auth       |
| -------- | -------- | ----------------------------------- | ---------- |
| `GET`    | `/`      | Retrieve paginated instructors list | 🛡️ `Admin` |
| `GET`    | `/{id}`  | Retrieve instructor profile         | 🛡️ `Admin` |
| `POST`   | `/`      | Add a new instructor                | 🛡️ `Admin` |
| `PUT`    | `/{id}`  | Update instructor profile           | 🛡️ `Admin` |
| `DELETE` | `/{id}`  | Remove an instructor                | 🛡️ `Admin` |

### 🛒 Cart (`/api/cart`)

| Method   | Endpoint       | Description              | Auth       |
| -------- | -------------- | ------------------------ | ---------- |
| `GET`    | `/`            | Get current user's cart  | 🔒 `User`  |
| `POST`   | `/{courseId}`  | Add a course to cart     | 🔒 `User`  |
| `DELETE` | `/{courseId}`  | Remove item from cart    | 🔒 `User`  |

### 📦 Orders (`/api/order`)

| Method | Endpoint     | Description              | Auth       |
| ------ | ------------ | ------------------------ | ---------- |
| `POST` | `/checkout`  | Process checkout logic   | 🔒 `User`  |
| `GET`  | `/Success`   | Payment success callback | ❌         |

### 🏠 Home (`/api/home`)

| Method | Endpoint            | Description               | Auth |
| ------ | ------------------- | ------------------------- | ---- |
| `GET`  | `/top-courses`      | List popular courses      | ❌   |
| `GET`  | `/top-Categories`   | List top categories       | ❌   |
| `GET`  | `/top-Instructor`   | List top instructors      | ❌   |

### 📊 Dashboard & ML

| Group      | Method | Endpoint                              | Description                   | Auth       |
| ---------- | ------ | ------------------------------------- | ----------------------------- | ---------- |
| **Admin**  | `GET`  | `/api/dashBoard`                      | System statistics overview    | 🛡️ `Admin` |
| **ML/Rec** | `GET`  | `/api/getSimilarCourses/{id}/similar` | Content-based recommendations | ❌         |

---

## ⚡ API Usage Examples

Here are some real-world payload examples for common flows.

### 1. User Registration (`POST /api/auth/register`)

**Request Body:**

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "johndoe@example.com",
  "password": "StrongPassword123!"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "d72d6a54-3e91...",
    "email": "johndoe@example.com"
  }
}
```

### 2. Get User Cart (`GET /api/cart`)

_Requires: `Authorization: Bearer <token>` Header_

**Response (200 OK):**

```json
{
  "success": true,
  "message": null,
  "data": {
    "courses": [
      {
        "courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "description": "Complete .NET 8 Web API Guide",
        "price": 49.99,
        "imagePath": "/images/dotnet-course.png"
      }
    ],
    "subtotal": 49.99,
    "tax": 7.5,
    "total": 57.49
  }
}
```

### 3. Create a New Course (`POST /api/course`)

_Requires: `Admin` Role (`Authorization: Bearer <token>`)_

**Request Body:**

```json
{
  "name": "Advanced React Patterns",
  "description": "Master advanced patterns in React and Next.js.",
  "price": 89.99,
  "rating": 4.8,
  "imagePath": "/images/react-adv.png",
  "createdAt": "2026-03-14T00:00:00Z",
  "level": "Advanced",
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "instructorId": "0b15b6d9-3665-4f40-8b06-056bd5652514"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": null,
  "data": "a3b91c78-1f1c-43f1-b92c-561b34a17932"
}
```

---

## 🧪 Testing

The project includes a comprehensive test suite covering all layers of the architecture.

| Layer             | Test Files                                      | Coverage Area                  |
| ----------------- | ----------------------------------------------- | ------------------------------ |
| **Application**   | `LoginHandlerTests`, `RegisterHandlerTests`     | Auth CQRS handlers             |
| **Application**   | `GetCourseQueryHandlerTests`                    | Course query logic             |
| **Application**   | `CheckoutHandlerTests`                          | Order/checkout flow            |
| **Domain**        | `CartTests`, `OrderTests`                       | Entity business rules          |
| **Infrastructure**| `AuthServiceTests`, `JwtServiceTests`           | Service implementations        |

**Testing Stack:**

- **Framework:** [xUnit](https://xunit.net/) `2.9.3`
- **Mocking:** [Moq](https://github.com/moq/moq4) `4.20.72` + [MockQueryable](https://github.com/nicecandies/MockQueryable) for EF Core `DbSet` mocking
- **Assertions:** [FluentAssertions](https://fluentassertions.com/) `8.8.0`
- **Coverage:** [Coverlet](https://github.com/coverlet-coverage/coverlet) `6.0.0`
- **In-Memory DB:** `Microsoft.EntityFrameworkCore.InMemory` `8.0.0`

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🛠️ Tech Stack

| Category           | Technology                                       |
| ------------------ | ------------------------------------------------ |
| **Runtime**        | .NET 8.0                                         |
| **Framework**      | ASP.NET Core Web API                             |
| **ORM**            | Entity Framework Core 8.0                        |
| **Database**       | SQL Server                                       |
| **Authentication** | ASP.NET Core Identity + JWT Bearer               |
| **Mediator**       | MediatR 14.0                                     |
| **Validation**     | FluentValidation 12.1.1                          |
| **API Docs**       | Swashbuckle (Swagger / OpenAPI)                  |
| **Testing**        | xUnit · Moq · FluentAssertions · Coverlet        |
| **Hosting**        | IIS / Kestrel (deployed on runasp.net)           |

---

## 🧪 Testing Swagger Endpoints

To interact with protected routes visually:

1. Navigate to [`http://bywayapi.runasp.net/docs`](http://bywayapi.runasp.net/docs) (or `https://localhost:<port>/docs` locally).
2. Hit the `/api/auth/login` endpoint to obtain your `token`.
3. Click **Authorize** 🔒 at the top right of the Swagger interface.
4. Enter `Bearer <your_token>` and click **Authorize**.
5. You are now authenticated for subsequent requests!

> **💡 Default Admin Credentials:** `admin@coursera.com` / `Admin@Admin0`

---

## 🤝 Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📫 Contact

**Author:** Amr Mohamed Radi

**Repository:** [https://github.com/amrmohamedradi/Coursera](https://github.com/amrmohamedradi/Coursera)
