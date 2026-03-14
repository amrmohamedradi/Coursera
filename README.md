<div align="center">

# 🎓 Coursera API (Backend)

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-10.0-512BD4?style=for-the-badge&logo=nuget)](https://docs.microsoft.com/en-us/ef/core/)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

_A robust, scalable, and secure ASP.NET Core Web API backend for a learning management platform._

[Explore the API](#-api-reference) · [Report Bug](#-contact) · [Request Feature](#-contact)

</div>

---

## 📖 Overview

This repository contains the backend infrastructure for a Coursera-like online learning platform. Built with **.NET 10** following **Clean Architecture** principles, it provides a solid foundation for managing courses, instructors, categories, user carts, and a secure checkout process. It leverages **CQRS** with **MediatR** to keep the application logic organized and maintainable.

### ✨ Key Features

- **🔐 Robust Authentication & Authorization**: ASP.NET Core Identity paired with JWT (Access & Refresh tokens) and Role-Based Access Control (Admin vs User).
- **🏛️ Clean Architecture**: Strict separation of concerns (`Api`, `Application`, `Domain`, `Infrastructure`).
- **📨 CQRS Pattern**: Implementation using `MediatR` for predictable and decoupled request handling.
- **📚 Comprehensive Course Management**: Full CRUD capabilities for Courses, Categories, and Instructors.
- **🛒 E-Commerce Capabilities**: Shopping cart management and secure checkout operations.
- **📄 Interactive API Documentation**: Swagger (OpenAPI) UI integrated out-of-the-box.
- **🛡️ Resilience**: Global exception handling middleware ensuring consistent JSON error responses.

---

## 🏗️ Architecture Setup

The solution is divided into distinct layers:

- 🎯 **`Coursera.Api`** — Web API, Controllers, Global Exception Handling, Swagger configuration, and Dependency Injection composition root.
- ⚙️ **`Coursera.Application`** — Business logic layer containing DTOs, CQRS Commands/Queries, MediatR handlers, and abstraction interfaces.
- 🌍 **`Coursera.Domain`** — Enterprise logic, core entities, enumerations, and domain exceptions.
- 🧱 **`Coursera.Infrastructure`** — External concerns: EF Core `ApplicationDbContext`, Identity (`ApplicationUser`), JWT token generation, Database Migrations, and persistence implementations.
- 🧪 **`Coursera.Tests`** — Unit and integration tests to ensure system reliability.

---

## 🚀 Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
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
    "Key": "SuperSecretKeyThatMustBeLongEnoughForHmacSha256!",
    "Issuer": "CourseraApiIssuer",
    "Audience": "CourseraApiAudience",
    "DurationInHours": 1,
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

> **💡 Note:** Roles (`Admin`, `User`) and optionally the default admin user are seeded automatically during application startup located in `Program.cs`.

### 4. Run the API

```bash
cd Coursera.Api
dotnet run
```

_Your API is now running! Navigate to `https://localhost:<port>/swagger` to explore the endpoints._

---

## 📡 API Reference

Explore the robust endpoints available in the system. Most endpoints return a standardized `ApiResponse<T>` wrapper.

```json
{
  "success": true,
  "message": null,
  "data": { ... }
}
```

### 🔑 Authentication (`/api/auth`)

| Method | Endpoint    | Description                                     | Auth |
| ------ | ----------- | ----------------------------------------------- | ---- |
| `POST` | `/register` | Register a new user                             | ❌   |
| `POST` | `/login`    | Authenticate user & receive JWT                 | ❌   |
| `POST` | `/refresh`  | Obtain a new access token using a refresh token | ❌   |

### 📚 Courses (`/api/course`)

| Method   | Endpoint | Description                      | Auth       |
| -------- | -------- | -------------------------------- | ---------- |
| `GET`    | `/{id}`  | Retrieve specific course details | ❌         |
| `GET`    | `/`      | Retrieve paginated courses       | ❌         |
| `POST`   | `/`      | Create a new course              | 🛡️ `Admin` |
| `PUT`    | `/{id}`  | Update an existing course        | 🛡️ `Admin` |
| `DELETE` | `/{id}`  | Remove a course                  | 🛡️ `Admin` |

### 🏷️ Categories (`/api/category`)

| Method   | Endpoint | Description               | Auth       |
| -------- | -------- | ------------------------- | ---------- |
| `GET`    | `/{id}`  | Retrieve category details | ❌         |
| `GET`    | `/`      | Retrieve all categories   | ❌         |
| `POST`   | `/`      | Create a new category     | 🛡️ `Admin` |
| `PUT`    | `/{id}`  | Update category           | 🛡️ `Admin` |
| `DELETE` | `/{id}`  | Remove a category         | 🛡️ `Admin` |

### 🧑‍🏫 Instructors (`/api/instructor`)

| Method   | Endpoint | Description                         | Auth       |
| -------- | -------- | ----------------------------------- | ---------- |
| `GET`    | `/{id}`  | Retrieve instructor profile         | 🛡️ `Admin` |
| `GET`    | `/`      | Retrieve paginated instructors list | 🛡️ `Admin` |
| `POST`   | `/`      | Add a new instructor                | 🛡️ `Admin` |
| `PUT`    | `/{id}`  | Update instructor profile           | 🛡️ `Admin` |
| `DELETE` | `/{id}`  | Remove an instructor                | 🛡️ `Admin` |

### 🛒 E-Commerce & Extras

| Group      | Method   | Endpoint                              | Description                   | Auth       |
| ---------- | -------- | ------------------------------------- | ----------------------------- | ---------- |
| **Cart**   | `GET`    | `/api/cart`                           | Get current user's cart       | 🔒 `User`  |
| **Cart**   | `DELETE` | `/api/cart/{courseId}`                | Remove item from cart         | 🔒 `User`  |
| **Orders** | `POST`   | `/api/order/checkout`                 | Process checkout logic        | 🔒 `User`  |
| **Orders** | `GET`    | `/api/order/Success`                  | Payment success callback      | ❌         |
| **Home**   | `GET`    | `/api/home/top-courses`               | List popular courses          | ❌         |
| **Admin**  | `GET`    | `/api/dashBoard`                      | System statistics overview    | 🛡️ `Admin` |
| **ML/Rec** | `GET`    | `/api/getSimilarCourses/{id}/similar` | Content-based recommendations | ❌         |

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
        "description": "Complete .NET 10 Web API Guide",
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

## 🧪 Testing Swagger Endpoints

To interact with protected routes visually:

1. Hit the `/api/auth/login` endpoint to obtain your `token`.
2. Click **Authorize** at the top right of the Swagger interface.
3. Enter `Bearer <your_token>` and click Authorize.
4. You are now authenticated for subsequent requests!

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

**Repository:** [https://github.com/amrmohamedradi/Coursera](https://github.com/amrmohamedradi/Coursera)

---

<div align="center">
  <i>Built with ❤️ for passionate developers and lifelong learners.</i>
</div>
