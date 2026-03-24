# Byway API — Complete Documentation

> **Base URL:** `http://bywayapi.runasp.net`  
> **OpenAPI Version:** 3.0.1  
> **API Version:** 1.0

---

## Table of Contents

1. [Overview](#overview)
2. [Authentication](#authentication)
3. [Standard Response Envelope](#standard-response-envelope)
4. [Error Handling](#error-handling)
5. [Enumerations](#enumerations)
6. [Controllers](#controllers)
   - [Auth](#auth-controller)
   - [Cart](#cart-controller)
   - [Category](#category-controller)
   - [Course](#course-controller)
   - [Instructor](#instructor-controller)
   - [Dashboard](#dashboard-controller)
   - [Home](#home-controller)
   - [Similar Courses](#similar-courses-controller)
   - [Order](#order-controller)

---

## Overview

The **Byway API** is a RESTful e-learning platform backend that supports user registration/login, course browsing, shopping cart management, checkout, and admin-level management of courses, categories, and instructors.

All responses follow a unified envelope format (see [Standard Response Envelope](#standard-response-envelope)).  
Admin-only endpoints require the `Admin` role; some user endpoints require any authenticated user. Public endpoints require no token.

---

## Authentication

The API uses **JWT Bearer tokens** for authentication.

### How it works

1. **Register** or **Login** to receive an `accessToken` and a `refreshToken`.
2. Include the access token in every protected request as an HTTP header:

```
Authorization: Bearer <your_access_token>
```

3. When the access token expires, call the **Refresh** endpoint with the refresh token to get a new access token without re-logging in.

### Token Lifetime

- **Access Token:** Short-lived (typically 15–60 minutes, configured server-side).
- **Refresh Token:** Long-lived (used once to obtain a new access token).

### Roles

| Role    | Description                                |
|---------|--------------------------------------------|
| `User`  | Registered end-user (can browse & shop)    |
| `Admin` | Platform administrator (full CRUD access)  |

---

## Standard Response Envelope

Every endpoint returns a JSON object with the following shape:

```json
{
  "success": true,
  "message": null,
  "data": { ... }
}
```

| Field     | Type      | Description                                              |
|-----------|-----------|----------------------------------------------------------|
| `success` | `boolean` | `true` on success, `false` on failure                    |
| `message` | `string?` | Human-readable message, usually `null` on success        |
| `data`    | `any`     | The actual payload; `null` for operations with no output |

---

## Error Handling

When a request fails, the API returns an appropriate HTTP status code and a response body like:

```json
{
  "success": false,
  "message": "Error description here",
  "data": null
}
```

### Common Status Codes

| Code  | Meaning                                                              |
|-------|----------------------------------------------------------------------|
| `200` | OK — Request succeeded                                               |
| `400` | Bad Request — Validation failed (e.g., missing fields, bad format)   |
| `401` | Unauthorized — Missing or invalid JWT token                          |
| `403` | Forbidden — Valid token but insufficient role (e.g., non-admin)      |
| `404` | Not Found — Requested resource does not exist                        |
| `500` | Internal Server Error — Unexpected server-side error                 |

---

## Enumerations

### `Level` (Course Difficulty)

| Integer Value | Name           |
|---------------|----------------|
| `1`           | `AllLevel`     |
| `2`           | `Beginner`     |
| `3`           | `Intermediate` |
| `4`           | `Expert`       |

### `JobTitle` (Instructor Role)

| String Value          | Description              |
|-----------------------|--------------------------|
| `FullStackDeveloper`  | Full Stack Developer     |
| `BackEndDeveloper`    | Back-End Developer       |
| `FrontEndDeveloper`   | Front-End Developer      |
| `UXUIDesigner`        | UX/UI Designer           |

> **Note:** When creating or updating an instructor, send the `jobTitle` as a **string** (the enum name), e.g. `"BackEndDeveloper"`.

### Paginated Response Shape

Endpoints that return lists support pagination and return:

```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [ ... ],
    "totalCount": 42,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

---

## Auth Controller

**Base path:** `/api/Auth`  
**Authentication required:** None (all endpoints are public)

---

### POST `/api/Auth/register`

Registers a new user account. The new user is assigned the `User` role by default.

**Authentication:** ❌ Not required

**Request Headers**

| Header         | Value              |
|----------------|--------------------|
| `Content-Type` | `application/json` |

**Request Body**

| Field       | Type     | Required | Validation              | Example              |
|-------------|----------|----------|-------------------------|----------------------|
| `firstName` | `string` | ✅ Yes   | Max 100 characters      | `"Ahmed"`            |
| `lastName`  | `string` | ✅ Yes   | Max 100 characters      | `"Hassan"`           |
| `email`     | `string` | ✅ Yes   | Valid email format      | `"ahmed@mail.com"`   |
| `password`  | `string` | ✅ Yes   | Minimum 8 characters    | `"P@ssword123"`      |

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Ahmed",
    "lastName": "Hassan",
    "email": "ahmed@mail.com",
    "password": "P@ssword123"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "email": "ahmed@mail.com"
  }
}
```

| Field          | Type     | Description                        |
|----------------|----------|------------------------------------|
| `token`        | `string` | JWT access token (use as Bearer)   |
| `refreshToken` | `string` | Refresh token for token renewal    |
| `email`        | `string` | Registered email address           |

**Possible Status Codes**

| Code  | Meaning                                      |
|-------|----------------------------------------------|
| `200` | Registration successful, tokens returned     |
| `400` | Validation error (missing/invalid fields)    |
| `400` | Email already registered                     |

---

### POST `/api/Auth/login`

Authenticates an existing user and returns JWT tokens.

**Authentication:** ❌ Not required

**Request Headers**

| Header         | Value              |
|----------------|--------------------|
| `Content-Type` | `application/json` |

**Request Body**

| Field      | Type     | Required | Example            |
|------------|----------|----------|--------------------|
| `email`    | `string` | ✅ Yes   | `"ahmed@mail.com"` |
| `password` | `string` | ✅ Yes   | `"P@ssword123"`    |

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "ahmed@mail.com",
    "password": "P@ssword123"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "email": "ahmed@mail.com"
  }
}
```

**Possible Status Codes**

| Code  | Meaning                               |
|-------|---------------------------------------|
| `200` | Login successful, tokens returned     |
| `400` | Invalid email or password             |
| `401` | Credentials do not match              |

---

### POST `/api/Auth/refresh`

Exchanges a valid refresh token for a new access token.

**Authentication:** ❌ Not required

**Request Headers**

| Header         | Value              |
|----------------|--------------------|
| `Content-Type` | `application/json` |

**Request Body**

| Field          | Type     | Required | Example                         |
|----------------|----------|----------|---------------------------------|
| `email`        | `string` | ✅ Yes   | `"ahmed@mail.com"`              |
| `refreshToken` | `string` | ✅ Yes   | `"dGhpcyBpcyBhIHJlZnJlc2gg..."` |

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Auth/refresh" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "ahmed@mail.com",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "bmV3UmVmcmVzaFRva2Vu...",
    "email": "ahmed@mail.com"
  }
}
```

**Possible Status Codes**

| Code  | Meaning                              |
|-------|--------------------------------------|
| `200` | New access token issued successfully |
| `400` | Invalid or expired refresh token     |

---

## Cart Controller

**Base path:** `/api/Cart`  
**Authentication required:** ✅ Yes — any authenticated user (JWT Bearer)

> The cart is user-specific. The user ID is automatically extracted from the JWT token — you never send it in the request body.

---

### GET `/api/Cart`

Returns the current user's shopping cart with all items and price summary.

**Authentication:** ✅ Required (User or Admin)

**Request Headers**

| Header          | Value                    |
|-----------------|--------------------------|
| `Authorization` | `Bearer <access_token>`  |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Cart" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "courses": [
      {
        "courseId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "description": "Complete React Developer Course",
        "price": 49.99,
        "imagePath": "https://example.com/images/react-course.jpg"
      }
    ],
    "subtotal": 49.99,
    "tax": 5.00,
    "total": 54.99
  }
}
```

**`data` fields**

| Field      | Type            | Description                    |
|------------|-----------------|--------------------------------|
| `courses`  | `CartItem[]`    | List of courses in the cart    |
| `subtotal` | `decimal`       | Sum of all course prices       |
| `tax`      | `decimal`       | Calculated tax amount          |
| `total`    | `decimal`       | Final amount (subtotal + tax)  |

**`CartItem` object**

| Field         | Type      | Description                |
|---------------|-----------|----------------------------|
| `courseId`    | `uuid`    | Unique course identifier   |
| `description` | `string`  | Course name/description    |
| `price`       | `decimal` | Course price               |
| `imagePath`   | `string`  | URL to the course image    |

**Possible Status Codes**

| Code  | Meaning                         |
|-------|---------------------------------|
| `200` | Cart retrieved successfully     |
| `401` | Missing or invalid token        |

---

### POST `/api/Cart/{courseId}`

Adds a course to the current user's cart.

**Authentication:** ✅ Required (User or Admin)

**Path Parameters**

| Parameter  | Type   | Required | Description              | Example                                  |
|------------|--------|----------|--------------------------|------------------------------------------|
| `courseId` | `uuid` | ✅ Yes   | ID of the course to add  | `a1b2c3d4-e5f6-7890-abcd-ef1234567890`  |

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <access_token>` |

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Cart/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

**Possible Status Codes**

| Code  | Meaning                                  |
|-------|------------------------------------------|
| `200` | Course added to cart successfully        |
| `401` | Missing or invalid token                 |
| `404` | Course not found                         |
| `400` | Course is already in the cart            |

---

### DELETE `/api/Cart/{courseId}`

Removes a course from the current user's cart.

**Authentication:** ✅ Required (User or Admin)

**Path Parameters**

| Parameter  | Type   | Required | Description                 | Example                                 |
|------------|--------|----------|-----------------------------|------------------------------------------|
| `courseId` | `uuid` | ✅ Yes   | ID of the course to remove  | `a1b2c3d4-e5f6-7890-abcd-ef1234567890` |

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <access_token>` |

**Example Request**

```bash
curl -X DELETE "http://bywayapi.runasp.net/api/Cart/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

**Possible Status Codes**

| Code  | Meaning                              |
|-------|--------------------------------------|
| `200` | Course removed from cart             |
| `401` | Missing or invalid token             |
| `404` | Course not found in cart             |

---

## Category Controller

**Base path:** `/api/Category`

| Method   | Endpoint               | Auth Required | Role    |
|----------|------------------------|---------------|---------|
| `GET`    | `/api/Category`        | ❌ No         | Public  |
| `GET`    | `/api/Category/{id}`   | ❌ No         | Public  |
| `POST`   | `/api/Category`        | ✅ Yes        | Admin   |
| `PUT`    | `/api/Category/{id}`   | ✅ Yes        | Admin   |
| `DELETE` | `/api/Category/{id}`   | ✅ Yes        | Admin   |

---

### GET `/api/Category`

Returns all available course categories.

**Authentication:** ❌ Not required

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Category"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "c1d2e3f4-a5b6-7890-cdef-012345678901",
      "name": "Web Development",
      "imagePath": "https://example.com/images/webdev.jpg"
    },
    {
      "id": "d2e3f4a5-b6c7-8901-defa-123456789012",
      "name": "Data Science",
      "imagePath": "https://example.com/images/datascience.jpg"
    }
  ]
}
```

**`CategoryDto` object**

| Field       | Type     | Description             |
|-------------|----------|-------------------------|
| `id`        | `uuid`   | Category identifier     |
| `name`      | `string` | Category display name   |
| `imagePath` | `string` | URL to category image   |

---

### GET `/api/Category/{id}`

Returns a single category by its ID.

**Authentication:** ❌ Not required

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `c1d2e3f4-a5b6-7890-cdef-012345678901`  |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Category/c1d2e3f4-a5b6-7890-cdef-012345678901"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "c1d2e3f4-a5b6-7890-cdef-012345678901",
    "name": "Web Development",
    "imagePath": "https://example.com/images/webdev.jpg"
  }
}
```

**Possible Status Codes**

| Code  | Meaning              |
|-------|----------------------|
| `200` | Category found       |
| `404` | Category not found   |

---

### POST `/api/Category`

Creates a new category. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |
| `Content-Type`  | `application/json`      |

**Request Body**

| Field       | Type     | Required | Description              | Example                                   |
|-------------|----------|----------|--------------------------|-------------------------------------------|
| `name`      | `string` | ✅ Yes   | Category name            | `"Mobile Development"`                   |
| `imagePath` | `string` | ⬜ No    | URL of the category image| `"https://example.com/images/mobile.jpg"` |

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Category" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mobile Development",
    "imagePath": "https://example.com/images/mobile.jpg"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "e3f4a5b6-c7d8-9012-efab-234567890123",
    "name": "Mobile Development",
    "imagePath": "https://example.com/images/mobile.jpg"
  }
}
```

**Possible Status Codes**

| Code  | Meaning                        |
|-------|--------------------------------|
| `200` | Category created successfully  |
| `401` | Missing or invalid token       |
| `403` | User is not an Admin           |
| `400` | Validation error               |

---

### PUT `/api/Category/{id}`

Updates an existing category by ID. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `c1d2e3f4-a5b6-7890-cdef-012345678901`  |

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |
| `Content-Type`  | `application/json`      |

**Request Body**

| Field       | Type     | Required | Description               | Example                                   |
|-------------|----------|----------|---------------------------|-------------------------------------------|
| `name`      | `string` | ⬜ No    | Updated category name     | `"Mobile & Cross-Platform"`              |
| `imagePath` | `string` | ⬜ No    | Updated image URL         | `"https://example.com/images/mobile2.jpg"`|

**Example Request**

```bash
curl -X PUT "http://bywayapi.runasp.net/api/Category/c1d2e3f4-a5b6-7890-cdef-012345678901" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mobile & Cross-Platform",
    "imagePath": "https://example.com/images/mobile2.jpg"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

**Possible Status Codes**

| Code  | Meaning                        |
|-------|--------------------------------|
| `200` | Category updated successfully  |
| `401` | Missing or invalid token       |
| `403` | User is not an Admin           |
| `404` | Category not found             |

---

### DELETE `/api/Category/{id}`

Deletes a category by ID. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `c1d2e3f4-a5b6-7890-cdef-012345678901`  |

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |

**Example Request**

```bash
curl -X DELETE "http://bywayapi.runasp.net/api/Category/c1d2e3f4-a5b6-7890-cdef-012345678901" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

**Possible Status Codes**

| Code  | Meaning                        |
|-------|--------------------------------|
| `200` | Category deleted successfully  |
| `401` | Missing or invalid token       |
| `403` | User is not an Admin           |
| `404` | Category not found             |

---

## Course Controller

**Base path:** `/api/Course`

| Method   | Endpoint             | Auth Required | Role    |
|----------|----------------------|---------------|---------|
| `GET`    | `/api/Course`        | ❌ No         | Public  |
| `GET`    | `/api/Course/{id}`   | ❌ No         | Public  |
| `POST`   | `/api/Course`        | ✅ Yes        | Admin   |
| `PUT`    | `/api/Course/{id}`   | ✅ Yes        | Admin   |
| `DELETE` | `/api/Course/{id}`   | ✅ Yes        | Admin   |

---

### GET `/api/Course`

Returns a paginated list of courses. Supports search by keyword.

**Authentication:** ❌ Not required

**Query Parameters**

| Parameter    | Type      | Required | Default | Description                        | Example      |
|--------------|-----------|----------|---------|------------------------------------|--------------|
| `pageNumber` | `integer` | ⬜ No    | `1`     | Page number (1-indexed)            | `1`          |
| `pageSize`   | `integer` | ⬜ No    | `10`    | Number of items per page           | `10`         |
| `search`     | `string`  | ⬜ No    | `null`  | Keyword to filter course names     | `"react"`    |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Course?pageNumber=1&pageSize=10&search=react"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "name": "Complete React Developer Course",
        "description": "Learn React from scratch to advanced concepts",
        "price": 49.99,
        "rating": 4.8,
        "createdAt": "2024-01-15T00:00:00Z",
        "level": 2,
        "imagePath": "https://example.com/images/react-course.jpg",
        "categoryId": "c1d2e3f4-a5b6-7890-cdef-012345678901",
        "instructorId": "f5a6b7c8-d9e0-1234-fabc-567890123456"
      }
    ],
    "totalCount": 25,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

---

### GET `/api/Course/{id}`

Returns the details of a single course by its ID.

**Authentication:** ❌ Not required

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `a1b2c3d4-e5f6-7890-abcd-ef1234567890`  |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Course/a1b2c3d4-e5f6-7890-abcd-ef1234567890"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "Complete React Developer Course",
    "description": "Learn React from scratch to advanced concepts",
    "price": 49.99,
    "rating": 4.8,
    "createdAt": "2024-01-15T00:00:00Z",
    "level": 2,
    "imagePath": "https://example.com/images/react-course.jpg",
    "categoryId": "c1d2e3f4-a5b6-7890-cdef-012345678901",
    "instructorId": "f5a6b7c8-d9e0-1234-fabc-567890123456"
  }
}
```

**`CourseDto` fields**

| Field          | Type       | Description                                    |
|----------------|------------|------------------------------------------------|
| `id`           | `uuid`     | Unique course identifier                       |
| `name`         | `string`   | Course title                                   |
| `description`  | `string`   | Detailed course description                    |
| `price`        | `decimal`  | Course price in USD                            |
| `rating`       | `decimal`  | Average rating (0.0 – 5.0)                     |
| `createdAt`    | `datetime` | ISO 8601 date when the course was created      |
| `level`        | `integer`  | Difficulty level (see [Enumerations](#enumerations)) |
| `imagePath`    | `string`   | URL to the course thumbnail image              |
| `categoryId`   | `uuid`     | ID of the associated category                  |
| `instructorId` | `uuid`     | ID of the associated instructor                |

**Possible Status Codes**

| Code  | Meaning           |
|-------|-------------------|
| `200` | Course found      |
| `404` | Course not found  |

---

### POST `/api/Course`

Creates a new course. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |
| `Content-Type`  | `application/json`      |

**Request Body**

| Field          | Type       | Required | Description                             | Example                                          |
|----------------|------------|----------|-----------------------------------------|--------------------------------------------------|
| `name`         | `string`   | ✅ Yes   | Course title                            | `"Complete React Developer Course"`              |
| `description`  | `string`   | ✅ Yes   | Course description                      | `"Learn React from scratch"`                     |
| `price`        | `number`   | ✅ Yes   | Price in USD (double)                   | `49.99`                                          |
| `rating`       | `number`   | ⬜ No    | Initial rating (double, 0-5)            | `0.0`                                            |
| `imagePath`    | `string`   | ⬜ No    | URL to course image                     | `"https://example.com/images/react.jpg"`        |
| `createdAt`    | `datetime` | ✅ Yes   | ISO 8601 creation date                  | `"2024-06-01T00:00:00Z"`                         |
| `level`        | `integer`  | ✅ Yes   | Difficulty level (1–4)                  | `2`                                              |
| `categoryId`   | `uuid`     | ✅ Yes   | ID of the category                      | `"c1d2e3f4-a5b6-7890-cdef-012345678901"`        |
| `instructorId` | `uuid`     | ✅ Yes   | ID of the instructor                    | `"f5a6b7c8-d9e0-1234-fabc-567890123456"`        |

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Course" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Complete React Developer Course",
    "description": "Learn React from scratch to advanced concepts",
    "price": 49.99,
    "rating": 0.0,
    "imagePath": "https://example.com/images/react.jpg",
    "createdAt": "2024-06-01T00:00:00Z",
    "level": 2,
    "categoryId": "c1d2e3f4-a5b6-7890-cdef-012345678901",
    "instructorId": "f5a6b7c8-d9e0-1234-fabc-567890123456"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

> `data` is the newly created course's `UUID`.

**Possible Status Codes**

| Code  | Meaning                       |
|-------|-------------------------------|
| `200` | Course created — ID returned  |
| `400` | Validation error              |
| `401` | Missing or invalid token      |
| `403` | User is not an Admin          |
| `404` | Category or instructor not found |

---

### PUT `/api/Course/{id}`

Updates an existing course. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `a1b2c3d4-e5f6-7890-abcd-ef1234567890`  |

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |
| `Content-Type`  | `application/json`      |

**Request Body** — Same fields as `POST /api/Course`

**Example Request**

```bash
curl -X PUT "http://bywayapi.runasp.net/api/Course/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Advanced React Developer Course",
    "description": "Master React with hooks and patterns",
    "price": 59.99,
    "rating": 4.8,
    "imagePath": "https://example.com/images/react-adv.jpg",
    "createdAt": "2024-06-01T00:00:00Z",
    "level": 3,
    "categoryId": "c1d2e3f4-a5b6-7890-cdef-012345678901",
    "instructorId": "f5a6b7c8-d9e0-1234-fabc-567890123456"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

**Possible Status Codes**

| Code  | Meaning                       |
|-------|-------------------------------|
| `200` | Course updated successfully   |
| `400` | Validation error              |
| `401` | Missing or invalid token      |
| `403` | User is not an Admin          |
| `404` | Course not found              |

---

### DELETE `/api/Course/{id}`

Deletes a course by ID. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `a1b2c3d4-e5f6-7890-abcd-ef1234567890`  |

**Example Request**

```bash
curl -X DELETE "http://bywayapi.runasp.net/api/Course/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

**Possible Status Codes**

| Code  | Meaning                       |
|-------|-------------------------------|
| `200` | Course deleted successfully   |
| `401` | Missing or invalid token      |
| `403` | User is not an Admin          |
| `404` | Course not found              |

---

## Instructor Controller

**Base path:** `/api/Instructor`  
**Authentication required:** ✅ Yes — All endpoints require the `Admin` role.

| Method   | Endpoint                 | Description              |
|----------|--------------------------|--------------------------|
| `GET`    | `/api/Instructor`        | Get all instructors      |
| `GET`    | `/api/Instructor/{id}`   | Get instructor by ID     |
| `POST`   | `/api/Instructor`        | Create new instructor    |
| `PUT`    | `/api/Instructor/{id}`   | Update instructor        |
| `DELETE` | `/api/Instructor/{id}`   | Delete instructor        |

---

### GET `/api/Instructor`

Returns a paginated list of instructors.

**Authentication:** ✅ Required — Role: `Admin`

**Query Parameters**

| Parameter    | Type      | Required | Default | Description                | Example      |
|--------------|-----------|----------|---------|----------------------------|--------------|
| `pageNumber` | `integer` | ⬜ No    | `1`     | Page number (1-indexed)    | `1`          |
| `pageSize`   | `integer` | ⬜ No    | `10`    | Items per page             | `10`         |
| `search`     | `string`  | ⬜ No    | `null`  | Keyword to search by name  | `"john"`     |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Instructor?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [
      {
        "id": "f5a6b7c8-d9e0-1234-fabc-567890123456",
        "name": "John Smith",
        "jobTitle": 1,
        "bio": "Seasoned full-stack developer with 10 years of experience.",
        "imagePath": "https://example.com/images/john.jpg"
      }
    ],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

**`InstructorDto` fields**

| Field       | Type     | Description                                          |
|-------------|----------|------------------------------------------------------|
| `id`        | `uuid`   | Unique instructor identifier                         |
| `name`      | `string` | Full name                                            |
| `jobTitle`  | `integer`| Job title enum value (see [Enumerations](#enumerations)) |
| `bio`       | `string` | Short biography                                      |
| `imagePath` | `string` | URL to the instructor's profile image                |

---

### GET `/api/Instructor/{id}`

Returns a single instructor by ID.

**Authentication:** ✅ Required — Role: `Admin`

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Instructor/f5a6b7c8-d9e0-1234-fabc-567890123456" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "f5a6b7c8-d9e0-1234-fabc-567890123456",
    "name": "John Smith",
    "jobTitle": 1,
    "bio": "Seasoned full-stack developer with 10 years of experience.",
    "imagePath": "https://example.com/images/john.jpg"
  }
}
```

---

### POST `/api/Instructor`

Creates a new instructor. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |
| `Content-Type`  | `application/json`      |

**Request Body**

| Field       | Type     | Required | Description                                     | Example                               |
|-------------|----------|----------|-------------------------------------------------|---------------------------------------|
| `name`      | `string` | ✅ Yes   | Instructor's full name                          | `"John Smith"`                        |
| `jobTitle`  | `string` | ✅ Yes   | One of the `JobTitle` enum **string names**     | `"FullStackDeveloper"`                |
| `bio`       | `string` | ⬜ No    | Short biography                                 | `"10+ years of web development"`      |
| `imagePath` | `string` | ⬜ No    | URL to profile image                            | `"https://example.com/images/john.jpg"` |

> ⚠️ **Important:** `jobTitle` must be the **string name** of the enum: `"FullStackDeveloper"`, `"BackEndDeveloper"`, `"FrontEndDeveloper"`, or `"UXUIDesigner"`.

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Instructor" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Smith",
    "jobTitle": "FullStackDeveloper",
    "bio": "Seasoned full-stack developer with 10 years of experience.",
    "imagePath": "https://example.com/images/john.jpg"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": "f5a6b7c8-d9e0-1234-fabc-567890123456"
}
```

> `data` is the newly created instructor's `UUID`.

**Possible Status Codes**

| Code  | Meaning                           |
|-------|-----------------------------------|
| `200` | Instructor created — ID returned  |
| `400` | Validation error or invalid enum  |
| `401` | Missing or invalid token          |
| `403` | User is not an Admin              |

---

### PUT `/api/Instructor/{id}`

Updates an existing instructor. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Path Parameters**

| Parameter | Type   | Required | Example                                  |
|-----------|--------|----------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | `f5a6b7c8-d9e0-1234-fabc-567890123456`  |

**Request Body** — Same fields as `POST /api/Instructor`

**Example Request**

```bash
curl -X PUT "http://bywayapi.runasp.net/api/Instructor/f5a6b7c8-d9e0-1234-fabc-567890123456" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Smith",
    "jobTitle": "BackEndDeveloper",
    "bio": "Backend specialist with expertise in .NET and microservices.",
    "imagePath": "https://example.com/images/john-updated.jpg"
  }'
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

---

### DELETE `/api/Instructor/{id}`

Deletes an instructor by ID. **Admin only.**

**Authentication:** ✅ Required — Role: `Admin`

**Example Request**

```bash
curl -X DELETE "http://bywayapi.runasp.net/api/Instructor/f5a6b7c8-d9e0-1234-fabc-567890123456" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": null
}
```

---

## Dashboard Controller

**Base path:** `/api/DashBoard`  
**Authentication required:** ✅ Yes — Role: `Admin`

---

### GET `/api/DashBoard`

Returns aggregate statistics for the admin dashboard.

**Authentication:** ✅ Required — Role: `Admin`

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <admin_token>`  |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/DashBoard" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "coursesCount": 42,
    "categoriesCount": 8,
    "instructorsCount": 15,
    "monthlySales": 12450.00
  }
}
```

**`DashboardDto` fields**

| Field              | Type      | Description                              |
|--------------------|-----------|------------------------------------------|
| `coursesCount`     | `integer` | Total number of courses in the platform  |
| `categoriesCount`  | `integer` | Total number of categories               |
| `instructorsCount` | `integer` | Total number of instructors              |
| `monthlySales`     | `decimal` | Revenue for the current calendar month   |

**Possible Status Codes**

| Code  | Meaning                       |
|-------|-------------------------------|
| `200` | Dashboard data returned       |
| `401` | Missing or invalid token      |
| `403` | User is not an Admin          |

---

## Home Controller

**Base path:** `/api/Home`  
**Authentication required:** ❌ None — All endpoints are public

Used to populate the homepage with highlighted content.

---

### GET `/api/Home/top-courses`

Returns the top-rated or most popular courses for homepage display.

**Authentication:** ❌ Not required

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Home/top-courses"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "name": "Complete React Developer Course",
      "description": "Learn React from scratch to advanced concepts",
      "price": 49.99,
      "rating": 4.8,
      "createdAt": "2024-01-15T00:00:00Z",
      "level": 2,
      "imagePath": "https://example.com/images/react-course.jpg",
      "categoryId": "c1d2e3f4-a5b6-7890-cdef-012345678901",
      "instructorId": "f5a6b7c8-d9e0-1234-fabc-567890123456"
    }
  ]
}
```

> Returns an array of `CourseDto` objects (see [Course Controller](#course-controller) for field definitions).

---

### GET `/api/Home/top-Categories`

Returns the top categories for homepage display.

**Authentication:** ❌ Not required

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Home/top-Categories"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "c1d2e3f4-a5b6-7890-cdef-012345678901",
      "name": "Web Development",
      "imagePath": "https://example.com/images/webdev.jpg"
    }
  ]
}
```

> Returns an array of `CategoryDto` objects.

---

### GET `/api/Home/top-Instructor`

Returns the highlighted instructors for homepage display.

**Authentication:** ❌ Not required

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Home/top-Instructor"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "f5a6b7c8-d9e0-1234-fabc-567890123456",
      "name": "John Smith",
      "jobTitle": 1,
      "bio": "Seasoned full-stack developer with 10 years of experience.",
      "imagePath": "https://example.com/images/john.jpg"
    }
  ]
}
```

> Returns an array of `InstructorDto` objects.

---

## Similar Courses Controller

**Base path:** `/api/GetSimilarCourses`  
**Authentication required:** ❌ Not required

---

### GET `/api/GetSimilarCourses/{id}/similar`

Returns a list of courses similar to the given course, based on category or other criteria.

**Authentication:** ❌ Not required

**Path Parameters**

| Parameter | Type   | Required | Description                             | Example                                  |
|-----------|--------|----------|-----------------------------------------|------------------------------------------|
| `id`      | `uuid` | ✅ Yes   | ID of the course to find similars for   | `a1b2c3d4-e5f6-7890-abcd-ef1234567890`  |

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/GetSimilarCourses/a1b2c3d4-e5f6-7890-abcd-ef1234567890/similar"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
      "name": "Vue.js Complete Guide",
      "description": "Master Vue.js from the ground up",
      "price": 39.99,
      "rating": 4.6,
      "createdAt": "2024-02-20T00:00:00Z",
      "level": 2,
      "imagePath": "https://example.com/images/vue.jpg",
      "categoryId": "c1d2e3f4-a5b6-7890-cdef-012345678901",
      "instructorId": "f5a6b7c8-d9e0-1234-fabc-567890123456"
    }
  ]
}
```

> Returns an array of `CourseDto` objects from the same category.

**Possible Status Codes**

| Code  | Meaning                      |
|-------|------------------------------|
| `200` | Similar courses returned     |
| `404` | Source course not found      |

---

## Order Controller

**Base path:** `/api/Order`

| Method | Endpoint               | Auth Required | Role      |
|--------|------------------------|---------------|-----------|
| `POST` | `/api/Order/checkout`  | ✅ Yes        | Any user  |
| `GET`  | `/api/Order/Success`   | ❌ No         | Public    |

---

### POST `/api/Order/checkout`

Initiates checkout for the current user's cart and creates an order. Processes everything currently in the cart.

**Authentication:** ✅ Required (any authenticated user)

**Request Headers**

| Header          | Value                   |
|-----------------|-------------------------|
| `Authorization` | `Bearer <access_token>` |

> No request body is needed. The user ID is automatically extracted from the JWT token.

**Example Request**

```bash
curl -X POST "http://bywayapi.runasp.net/api/Order/checkout" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": "9f8e7d6c-b5a4-3210-fedc-ba9876543210"
}
```

> `data` is the newly created **Order UUID**.

**Possible Status Codes**

| Code  | Meaning                               |
|-------|---------------------------------------|
| `200` | Order created — Order ID returned     |
| `401` | Missing or invalid token              |
| `400` | Cart is empty or checkout failed      |

---

### GET `/api/Order/Success`

A callback/confirmation endpoint that indicates a payment was completed successfully. Typically used as a redirect target after payment processing.

**Authentication:** ❌ Not required

**Example Request**

```bash
curl -X GET "http://bywayapi.runasp.net/api/Order/Success"
```

**Success Response — 200 OK**

```json
{
  "success": true,
  "message": null,
  "data": {
    "message": "Payment completed successfully"
  }
}
```

---

## Quick Reference

### Authentication Flow

```
1. POST /api/Auth/register     → Get accessToken + refreshToken
2. Use accessToken in header:  Authorization: Bearer <accessToken>
3. When expired:
   POST /api/Auth/refresh       → Get new accessToken + refreshToken
```

### Full Shopping Flow (User)

```
1. GET  /api/Course             → Browse courses
2. GET  /api/Course/{id}        → View course details
3. POST /api/Cart/{courseId}    → Add course to cart   [AUTH]
4. GET  /api/Cart               → Review cart          [AUTH]
5. POST /api/Order/checkout     → Place order/checkout [AUTH]
```

### Admin Management Flow

```
POST   /api/Auth/login          → Login as Admin
POST   /api/Instructor          → Create instructor    [ADMIN]
POST   /api/Category            → Create category      [ADMIN]
POST   /api/Course              → Create course        [ADMIN]
GET    /api/DashBoard           → View stats           [ADMIN]
```
