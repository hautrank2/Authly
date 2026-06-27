# Authly — Authentication & User Management API

REST API built on **ASP.NET Core 10**, using **MongoDB**, **Redis**, and **JWT** for authentication and user management.

---

## Table of Contents

- [Authentication](#authentication)
- [Response Format](#response-format)
- [API Reference](#api-reference)
  - [Auth](#auth-apiauth)
  - [User](#user-apiuser)
- [Enums](#enums)
- [Pagination](#pagination)
- [Error Codes](#error-codes)
- [Validation Rules](#validation-rules)
- [Usage Examples](#usage-examples)

---

## Authentication

The API uses **Bearer JWT Token**.

After a successful login, attach the token to every request that requires authentication:

```
Authorization: Bearer <accessToken>
```

> **Note:** A token is immediately invalidated upon calling the logout endpoint (the token is added to a Redis blacklist).

---

## Response Format

Every response is wrapped in a common structure:

```json
{
  "success": true,
  "message": "Description",
  "data": {},
  "errors": []
}
```

| Field | Type | Description |
|---|---|---|
| `success` | `boolean` | `true` if the request succeeded |
| `message` | `string` | Short description of the result |
| `data` | `object \| null` | Returned payload |
| `errors` | `string[]` | Validation error list (when `success = false`) |

---

## API Reference

### Auth `/api/auth`

---

#### `POST /api/auth/login`

Authenticate a user and receive a JWT token.

**Headers:** no Authorization required

**Request body:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Success response (`200`):**
```json
{
  "success": true,
  "message": null,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-06-27T10:00:00Z",
    "user": {
      "id": "683abc123def456",
      "name": "Nguyen Van A",
      "birthday": "2000-01-15",
      "avtUrl": "https://res.cloudinary.com/...",
      "username": "nguyenvana",
      "role": "Dev",
      "latestAccess": "2026-06-27T09:00:00Z",
      "createdAt": "2026-01-01T00:00:00Z",
      "updatedAt": "2026-06-27T09:00:00Z",
      "createdBy": null,
      "updatedBy": null
    }
  },
  "errors": null
}
```

**Error response (`400` — wrong credentials):**
```json
{
  "success": false,
  "message": "Invalid username or password.",
  "data": null,
  "errors": null
}
```

---

#### `POST /api/auth/logout`

Invalidate the current JWT token.

**Headers:**
```
Authorization: Bearer <accessToken>
```

**Request body:** none

**Success response (`200`):**
```json
{
  "success": true,
  "message": "Logged out successfully.",
  "data": null,
  "errors": null
}
```

---

### User `/api/user`

> All endpoints in this group **require an Authorization header**.

---

#### `GET /api/user`

Retrieve a paginated, filterable list of users.

**Required role:** `Admin`

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `pageIndex` | `number` | No | Current page (default: `1`) |
| `pageSize` | `number` | No | Records per page (default: `10`) |
| `name` | `string` | No | Filter by name (substring match) |
| `startAge` | `number` | No | Minimum age |
| `endAge` | `number` | No | Maximum age |
| `role` | `string` | No | Filter by role (see [Enums](#enums)) |
| `isDeleted` | `boolean` | No | Include soft-deleted users (default: `false`) |

**Example request:**
```
GET /api/user?pageIndex=1&pageSize=10&name=nguyen&role=Dev
```

**Success response (`200`):**
```json
{
  "success": true,
  "message": null,
  "data": {
    "pageIndex": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPage": 3,
    "items": [
      {
        "id": "683abc123def456",
        "name": "Nguyen Van A",
        "birthday": "2000-01-15",
        "avtUrl": "https://res.cloudinary.com/...",
        "username": "nguyenvana",
        "role": "Dev",
        "latestAccess": "2026-06-27T09:00:00Z",
        "createdAt": "2026-01-01T00:00:00Z",
        "updatedAt": "2026-06-27T09:00:00Z",
        "createdBy": "admin_id",
        "updatedBy": "admin_id"
      }
    ]
  },
  "errors": null
}
```

---

#### `POST /api/user`

Create a new user.

**Required role:** `Admin`

**Content-Type:** `multipart/form-data`

**Form fields:**

| Field | Type | Required | Description |
|---|---|---|---|
| `name` | `string` | **Yes** | Full name (1–100 characters) |
| `username` | `string` | **Yes** | Login name (3–30 characters, only `a-z A-Z 0-9 _ -`) |
| `password` | `string` | **Yes** | Password (≥8 characters, at least 1 digit + 1 special character) |
| `birthday` | `string` | **Yes** | Date of birth in `YYYY-MM-DD` format |
| `avatar` | `file` | No | Profile picture (JPG, PNG, ...) |
| `role` | `string` | No | Role (default: `Dev`, see [Enums](#enums)) |

**Success response (`200`):**
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "683abc123def456",
    "name": "Nguyen Van B",
    "birthday": "1999-05-20",
    "avtUrl": "https://res.cloudinary.com/...",
    "username": "nguyenvanb",
    "role": "Frontend",
    "latestAccess": null,
    "createdAt": "2026-06-27T09:00:00Z",
    "updatedAt": "2026-06-27T09:00:00Z",
    "createdBy": "admin_id",
    "updatedBy": "admin_id"
  },
  "errors": null
}
```

**Validation error (`400`):**
```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": [
    "Username must be between 3 and 30 characters.",
    "Password must be at least 8 characters and contain at least one digit and one special character."
  ]
}
```

---

#### `PUT /api/user/{id}`

Update a user's profile information.

**Required role:** The user themselves **or** `Admin`

**URL params:**

| Param | Type | Description |
|---|---|---|
| `id` | `string` | ID of the user to update |

**Content-Type:** `application/json`

**Request body** (all fields are optional):
```json
{
  "name": "Nguyen Van A Updated",
  "birthday": "2000-03-10",
  "role": "TeamLead"
}
```

> **Note:** Only `Admin` can change `role`. Regular users can only update `name` and `birthday`.

**Success response (`200`):**
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "683abc123def456",
    "name": "Nguyen Van A Updated",
    "birthday": "2000-03-10",
    "avtUrl": "https://res.cloudinary.com/...",
    "username": "nguyenvana",
    "role": "TeamLead",
    "latestAccess": "2026-06-27T09:00:00Z",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-06-27T10:00:00Z",
    "createdBy": "admin_id",
    "updatedBy": "current_user_id"
  },
  "errors": null
}
```

---

#### `PUT /api/user/{id}/image`

Update a user's profile picture.

**Required role:** The user themselves **or** `Admin`

**URL params:**

| Param | Type | Description |
|---|---|---|
| `id` | `string` | ID of the user |

**Content-Type:** `multipart/form-data`

**Form fields:**

| Field | Type | Required | Description |
|---|---|---|---|
| `file` | `file` | **Yes** | New image file (JPG, PNG, ...) |

**Success response (`200`):**
```json
{
  "success": true,
  "message": null,
  "data": {
    "id": "683abc123def456",
    "name": "Nguyen Van A",
    "birthday": "2000-01-15",
    "avtUrl": "https://res.cloudinary.com/.../new-avatar.jpg",
    "username": "nguyenvana",
    "role": "Dev",
    "latestAccess": "2026-06-27T09:00:00Z",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-06-27T10:30:00Z",
    "createdBy": "admin_id",
    "updatedBy": "683abc123def456"
  },
  "errors": null
}
```

---

#### `DELETE /api/user/{id}`

Soft-delete a user (marks as deleted, does not remove from the database).

**Required role:** `Admin`

> **Note:** Admin accounts cannot be deleted.

**URL params:**

| Param | Type | Description |
|---|---|---|
| `id` | `string` | ID of the user to delete |

**Success response (`200`):**
```json
{
  "success": true,
  "message": "User deleted successfully.",
  "data": null,
  "errors": null
}
```

**Error response (`400` — attempting to delete an Admin):**
```json
{
  "success": false,
  "message": "Cannot delete an admin user.",
  "data": null,
  "errors": null
}
```

---

#### `PUT /api/user/{id}/change-password`

Allows a user to change their own password.

**Required role:** The user themselves only (Admin cannot use this on behalf of others)

**URL params:**

| Param | Type | Description |
|---|---|---|
| `id` | `string` | ID of the user |

**Content-Type:** `application/json`

**Request body:**
```json
{
  "currentPassword": "OldPass@123",
  "newPassword": "NewPass@456"
}
```

| Field | Required | Constraint |
|---|---|---|
| `currentPassword` | **Yes** | The user's current password |
| `newPassword` | **Yes** | ≥8 characters, at least 1 digit + 1 special character |

**Success response (`200`):**
```json
{
  "success": true,
  "message": "Password changed successfully.",
  "data": null,
  "errors": null
}
```

**Error response (`400` — wrong current password):**
```json
{
  "success": false,
  "message": "Current password is incorrect.",
  "data": null,
  "errors": null
}
```

---

#### `PUT /api/user/{id}/reset-password`

Admin resets a user's password without needing to know the current one.

**Required role:** `Admin`

**URL params:**

| Param | Type | Description |
|---|---|---|
| `id` | `string` | ID of the user |

**Content-Type:** `application/json`

**Request body:**
```json
{
  "newPassword": "ResetPass@789"
}
```

| Field | Required | Constraint |
|---|---|---|
| `newPassword` | **Yes** | ≥8 characters, at least 1 digit + 1 special character |

**Success response (`200`):**
```json
{
  "success": true,
  "message": "Password reset successfully.",
  "data": null,
  "errors": null
}
```

---

## Enums

### UserRole

| Value | Description |
|---|---|
| `Backend` | Backend Developer |
| `Frontend` | Frontend Developer |
| `BA` | Business Analyst |
| `Dev` | Developer (default when creating a new user) |
| `TeamLead` | Team Lead |
| `Admin` | Administrator (full access) |

---

## Pagination

All list endpoints return a paginated structure:

```json
{
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPage": 3,
  "items": []
}
```

| Field | Type | Description |
|---|---|---|
| `pageIndex` | `number` | Current page |
| `pageSize` | `number` | Records per page |
| `totalCount` | `number` | Total number of records |
| `totalPage` | `number` | Total number of pages |
| `items` | `array` | Records for the current page |

---

## Error Codes

| HTTP Status | Meaning |
|---|---|
| `200` | Success |
| `400` | Invalid input or business logic error (see `errors[]` or `message`) |
| `401` | Not authenticated, token expired, or already logged out |
| `403` | Insufficient permissions for this endpoint |
| `404` | Resource not found |
| `500` | Internal server error |

---

## Validation Rules

| Field | Rule |
|---|---|
| `username` | 3–30 characters, only `a-z`, `A-Z`, `0-9`, `_`, `-` |
| `password` | Minimum 8 characters, must contain at least **1 digit** and **1 special character** (`!@#$%^&*...`) |
| `name` | 1–100 characters |
| `birthday` | `YYYY-MM-DD` format (e.g. `2000-01-15`) |

---

## Usage Examples

### Fetch API

```javascript
const BASE_URL = 'http://localhost:5088';

// Login
const loginRes = await fetch(`${BASE_URL}/api/auth/login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username: 'admin', password: 'Admin@123' }),
});
const { data } = await loginRes.json();
const token = data.accessToken;

// Fetch users (Admin only)
const usersRes = await fetch(`${BASE_URL}/api/user?pageIndex=1&pageSize=10`, {
  headers: { Authorization: `Bearer ${token}` },
});
const users = await usersRes.json();

// Upload profile picture
const formData = new FormData();
formData.append('file', imageFile);
await fetch(`${BASE_URL}/api/user/${userId}/image`, {
  method: 'PUT',
  headers: { Authorization: `Bearer ${token}` },
  body: formData,
});

// Logout
await fetch(`${BASE_URL}/api/auth/logout`, {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
});
```

### Axios

```javascript
import axios from 'axios';

const BASE_URL = 'http://localhost:5088';

// Create an axios instance with a base URL
const api = axios.create({ baseURL: BASE_URL });

// Attach token to every request after login
const setAuthToken = (token) => {
  api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
};

// Login
const loginRes = await api.post('/api/auth/login', {
  username: 'admin',
  password: 'Admin@123',
});
const token = loginRes.data.data.accessToken;
setAuthToken(token);

// Fetch users (Admin only)
const usersRes = await api.get('/api/user', {
  params: { pageIndex: 1, pageSize: 10 },
});
const users = usersRes.data.data;

// Upload profile picture
const formData = new FormData();
formData.append('file', imageFile);
await api.put(`/api/user/${userId}/image`, formData, {
  headers: { 'Content-Type': 'multipart/form-data' },
});

// Logout
await api.post('/api/auth/logout');
```
