# Coursera API (Backend)

This project is an ASP.NET Core Web API backend.

## Base URL

Local (Swagger):
- https://localhost:7040/swagger/index.html

API base:
- https://localhost:7040

## Authentication (JWT + Refresh Token)

The backend uses:
- ASP.NET Identity (users)
- JWT Bearer authentication (access token)
- Refresh token stored on the user record (`ApplicationUser.RefreshToken` + `ApplicationUser.RefreshTokenExpiryTime`)

### How the frontend should use auth

- **Access Token**
  - Used for calling protected endpoints.
  - Send it in the header:
    - `Authorization: Bearer <accessToken>`

- **Refresh Token**
  - Used to get a new access token when the access token expires.
  - Call the refresh endpoint with the refresh token.
  - The backend **rotates** the refresh token (returns a new one).

## Response Format

Most endpoints return an object like:

```json
{
  "success": true,
  "message": null,
  "data": { }
}
```

If an error happens, the API may return a 500 with:

```json
{
  "message": "Error message"
}
```

## Auth Endpoints

### 1) Register

`POST /api/auth/register`

Password rules:
- Minimum length: 8
- Must contain: 1 uppercase, 1 lowercase, 1 number, 1 special character

Body:
```json
{
  "firstName": "Test",
  "lastName": "User",
  "email": "test1@test.com",
  "password": "123456"
}
```

Response (`data`):
```json
{
  "token": "<accessToken>",
  "refreshToken": "<refreshToken>",
  "email": "test1@test.com"
}
```

### 2) Login

`POST /api/auth/login`

Body:
```json
{
  "email": "test1@test.com",
  "password": "123456"
}
```

Response (`data`):
```json
{
  "token": "<accessToken>",
  "refreshToken": "<refreshToken>",
  "email": "test1@test.com"
}
```

### 3) Refresh Token

`POST /api/auth/refresh`

Body:
```json
{
  "email": "test1@test.com",
  "refreshToken": "<refreshToken>"
}
```

Response (`data`):
```json
{
  "token": "<newAccessToken>",
  "refreshToken": "<newRefreshToken>",
  "email": "test1@test.com"
}
```

Notes:
- If you call refresh with an old refresh token, you should get an error.
- If the refresh token is expired, you should get an error.

## Calling a Protected Endpoint

1. Login and get `token`.
2. Call the endpoint with:

```
Authorization: Bearer <token>
```

Example:
- `POST /api/order/checkout` requires authentication.

## Configuration

JWT configuration is in:
- `Coursera.Api/appsettings.json` under `JWT`

Example:
- `DurationInHours`: access token lifetime
- `RefreshTokenDurationInDays`: refresh token lifetime
