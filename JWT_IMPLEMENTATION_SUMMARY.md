# JWT Authentication System with Refresh Token - Implementation Summary

## Overview
A complete JWT Authentication system has been implemented for the ASP.NET Core Web API project following Clean/Onion Architecture principles. The implementation is intentionally straightforward, suitable for Junior Developers, without over-engineering.

## Architecture Overview

```
├── Coursera.Api (Web API Layer)
│   └── Controllers
│       └── AuthController.cs (Already exists - CQRS pattern)
│
├── Coursera.Application (Business Logic Layer)
│   ├── Features/Auth/
│   │   ├── Register/
│   │   │   ├── RegisterCommand.cs (NEW)
│   │   │   └── RegisterCommandHandler.cs (NEW)
│   │   ├── Login/
│   │   │   ├── LoginCommand.cs (UPDATED)
│   │   │   └── LoginCommandHandler.cs (NEW)
│   │   ├── Refresh/
│   │   │   ├── RefreshTokenCommand.cs (UPDATED)
│   │   │   └── RefreshTokenCommandHandler.cs (NEW)
│   │   └── AuthResponse.cs (Already exists)
│   │
│   ├── Common/
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs (Already exists)
│   │   │   └── IJwtService.cs (NEW)
│   │   ├── Models/
│   │   │   ├── JwtSettings.cs (Already exists)
│   │   │   └── ApiResponse.cs (Already exists)
│   │   └── DTOs/
│   │       ├── UserTokenDto.cs (Already exists)
│   │       └── RefreshTokenRequest.cs (UPDATED)
│   │
│   └── DependencyInjection.cs (Already configured for MediatR)
│
├── Coursera.Infrastructure (Data Access & Services Layer)
│   ├── Identity/
│   │   ├── ApplicationUser.cs (Already exists with RefreshToken fields)
│   │   └── JwtService.cs (NEW)
│   │
│   ├── Service/
│   │   └── AuthService.cs (Already exists - handles user creation & token storage)
│   │
│   └── DependencyInjection.cs (Already registered IJwtService & IAuthService)
│
└── Coursera.Domain (Entities & Business Rules)
```

## Key Components

### 1. **JwtService** (`Coursera.Infrastructure/Identity/JwtService.cs`)
Responsible for JWT token generation.

**Methods:**
- `GenerateAccessToken(ApplicationUser user)` - Creates JWT access token with claims (UserId, Email)
- `GenerateRefreshToken()` - Creates secure random refresh token using RandomNumberGenerator

**Configuration:** Uses `JwtSettings` from appsettings.json (Key, Issuer, Audience, DurationInHours)

### 2. **CQRS Commands & Handlers**

#### Register Command
- **File:** `Coursera.Application/Features/Auth/Register/RegisterCommand.cs`
- **Handler:** `RegisterCommandHandler.cs`
- **Flow:** Create user → Validate → Generate tokens → Store refresh token
- **Returns:** `AuthResponse` with Token and RefreshToken

#### Login Command
- **File:** `Coursera.Application/Features/Auth/Login/LoginCommand.cs`
- **Handler:** `LoginCommandHandler.cs`
- **Flow:** Find user → Validate password → Generate tokens → Store refresh token
- **Returns:** `AuthResponse` with Token and RefreshToken

#### Refresh Token Command
- **File:** `Coursera.Application/Features/Auth/Refresh/RefreshTokenCommand.cs`
- **Handler:** `RefreshTokenCommandHandler.cs`
- **Flow:** Validate refresh token → Generate new access token → Generate new refresh token
- **Returns:** `AuthResponse` with new Token and RefreshToken

### 3. **Application User**
Extended with JWT-specific fields (already in place):
```csharp
public string? RefreshToken { get; set; }
public DateTime RefreshTokenExpiryTime { get; set; }
```

### 4. **AuthService**
Already implemented with methods:
- `RegisterAsync()` - Creates user with "User" role
- `LoginAsync()` - Validates credentials
- `SetRefreshTokenAsync()` - Stores refresh token and expiry
- `RefreshTokenAsync()` - Validates and handles refresh token logic

## API Endpoints

All endpoints use CQRS pattern via MediatR:

### POST `/api/auth/register`
**Request:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "rnd0m+r3fr3sh0k3n...",
    "email": "john@example.com"
  }
}
```

### POST `/api/auth/login`
**Request:**
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

**Response:** Same as register

### POST `/api/auth/refresh`
**Request:**
```json
{
  "email": "john@example.com",
  "refreshToken": "rnd0m+r3fr3sh0k3n..."
}
```

**Response:** New AccessToken and RefreshToken

## JWT Token Structure

**Access Token Payload:**
```json
{
  "UserId": "guid",
  "email": "user@example.com",
  "nameid": "guid",
  "exp": 1234567890,
  "iss": "CourseraApi",
  "aud": "CourseraClient"
}
```

**Lifespan:** 2 hours (configurable in `DurationInHours`)

## Token Refresh Flow

1. Client uses access token until it expires
2. Client sends refresh token to `/api/auth/refresh`
3. Server validates:
   - User exists
   - Refresh token matches stored token
   - Refresh token hasn't expired (7 days)
4. Server generates:
   - New access token (2 hours)
   - New refresh token (7 days)
5. Client stores both tokens and continues

## Configuration

**appsettings.json:**
```json
{
  "JWT": {
    "Key": "YausPt63A+ndQcbSxkFxFC+F+fWEEqRXUpRwQ0tc9cM=",
    "Issuer": "CourseraApi",
    "Audience": "CourseraClient",
    "DurationInHours": 2,
    "RefreshTokenDurationInDays": 7
  }
}
```

**Program.cs Configuration:**
- JWT Bearer authentication scheme configured
- Token validation (Issuer, Audience, Signature, Lifetime)
- CORS policy enabled ("AllowAll")
- MediatR auto-registers all handlers from assembly

## Error Handling

All handlers use simple exception throwing (as per requirements):
- "User already exists" - Registration duplicate
- "Invalid email or password" - Login validation
- "User not found" - Refresh token validation
- "Invalid refresh token" - Token mismatch
- "Refresh token has expired" - Token expiry check

Exceptions are caught by `ExceotionMiddlewares` and returned as `ApiResponse` with `Success: false`

## Database Requirements

The `ApplicationDbContext` must be migrated to include:
- `AspNetUsers` table with columns:
  - `RefreshToken` (string, nullable)
  - `RefreshTokenExpiryTime` (datetime)

## Security Features Implemented

✅ JWT-based stateless authentication
✅ Secure refresh token generation (RandomNumberGenerator.Create())
✅ Token expiration validation
✅ Refresh token rotation
✅ CORS configured for cross-origin requests
✅ HTTPS enforced
✅ Password hashing via ASP.NET Identity

## How to Use

1. **Register a new user:**
   ```
   POST /api/auth/register
   ```

2. **Login user:**
   ```
   POST /api/auth/login
   ```

3. **Use access token:**
   Add to request header: `Authorization: Bearer {accessToken}`

4. **Refresh expired token:**
   ```
   POST /api/auth/refresh
   ```

## Dependencies

- `Microsoft.AspNetCore.Authentication.JwtBearer` v10.0.3
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` v10.0.3
- `Microsoft.EntityFrameworkCore` v10.0.3
- `MediatR` v14.0.0
- `FluentValidation` v12.1.1

## Notes

- Code is intentionally simple and straightforward for junior developers
- No advanced patterns like Repository Pattern or Specification Pattern
- Uses basic error handling with thrown exceptions
- MediatR CQRS pattern used for separation of concerns
- No custom validation rules yet (can be added to FluentValidation if needed)
