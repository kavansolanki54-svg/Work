# DallyWorkReport API Documentation

Welcome to the **DallyWorkReport API** documentation. This API provides structured access to organization, member, and project tracking systems, including role-based access control and master data lookup.

## Table of Contents
1. [Authentication & Security](#1-authentication--security)
2. [Master Data](#2-master-data)
3. [Organizational Management](#3-organizational-management)
4. [Project & Operations](#4-project--operations)
5. [Access Control](#5-access-control)
6. [Testing & Observed Behavior](#6-testing--observed-behavior)
7. [Issues & Improvements](#7-issues--improvements)

---

## 1. Authentication & Security
The API uses **JWT (JSON Web Token)** for authorization. All protected endpoints require a Bearer token in the `Authorization` header.

### Login
- **Method**: `POST`
- **URL**: `{{host}}/api/Auth/login`
- **Description**: Authenticates a user and returns a JWT access token.

#### Request Body (LoginDTO)
```json
{
  "email": "user@example.com",
  "password": "yourpassword"
}
```
| Field | Type | Mandatory | Validation |
|---|---|---|---|
| email | string | Yes | Max 100 chars |
| password | string | Yes | Max 255 chars |

#### Example (cURL)
```bash
curl -X POST "http://localhost:5083/api/Auth/login" \
     -H "Content-Type: application/json" \
     -d '{"email": "admin@example.com", "password": "password123"}'
```

#### Success Response (`200 OK`)
```json
{
  "success": true,
  "message": "Token Generated",
  "data": {
    "accessToken": "eyJhbGci...",
    "refreshToken": "XyZ123...",
    "user": { "employeeID": 1, "userName": "Admin", "email": "admin@..." }
  }
}
```

---

### SignUp
- **Method**: `POST`
- **URL**: `{{host}}/api/Auth/SignUp`
- **Description**: Registers a new company and admin employee.

#### Request Body (SignUpViewModel)
| Field | Type | Mandatory | Constraints |
|---|---|---|---|
| companyName | string | Yes | Max 200 |
| email | string | Yes | Max 200, Email Format |
| password | string | Yes | Max 300 |
| confirmPassword | string | Yes | Must match `password` |

---

## 2. Master Data
Generic lookups for population filters.

### Get Countries
- **Method**: `GET`
- **URL**: `{{host}}/api/MasterData/countries`

### Get States
- **Method**: `GET`
- **URL**: `{{host}}/api/MasterData/states/{countryId}`

### Get Lookups
- **Method**: `GET`
- **URL**: `{{host}}/api/MasterData/lookups/{typeName}`
- **Common Types**: `Gender`, `EmployeeStatus`.

---

## 3. Organizational Management

### Company Master
- **Endpoints**:
    - `GET /api/CompanyMaster/{id}`: Retrieve company details.
    - `POST /api/CompanyMaster`: Create company (Multipart/form-data).
    - `PUT /api/CompanyMaster/{id}`: Update company (Multipart/form-data).

#### Request Body (CompanyDTO)
Supports `LogoFile` as `IFormFile`.

### Employee Master
- **Endpoints**:
    - `GET /api/EmployeeMaster/list/{companyId}`: List all employees.
    - `GET /api/EmployeeMaster/{id}`: Get employee by ID (Decrypts password for UI).
    - `POST /api/EmployeeMaster/save`: Create employee.
    - `PUT /api/EmployeeMaster/update`: Update employee.
    - `DELETE /api/EmployeeMaster/{id}`: Soft delete (sets ActiveStatus=0).

---

## 4. Project & Operations
Standard CRUD for Projects, Modules, and Statuses.

### Project Master
- **GET** `/api/ProjectMaster/List/{companyId}`
- **POST** `/api/ProjectMaster/Save`
- **PUT** `/api/ProjectMaster/Update`
- **DELETE** `/api/ProjectMaster/Delete/{id}` (Soft Delete)

| Schema (ProjectDTO) | Type | Description |
|---|---|---|
| projectId | int | ID (0 for new) |
| projectName | string | Required |
| projectColor | string | Hex/CSS color |
| companyId | int | Required |

---

## 5. Access Control

### Role Management
- **GET** `/api/RoleMaster?companyId=1`
- **POST** `/api/RoleMaster`: Create Role.

### Permission Hierarchy
- **GET** `/api/RoleMasterSoftwareModules/hierarchy/{roleId}`
- **Description**: Returns nested menu structure with granular permissions (View, Add, Edit, Delete).

### Save Permissions
- **POST** `/api/RoleMasterSoftwareModules/save`
```json
{
  "roleId": 1,
  "permissions": [
    { "moduleId": 10, "view": true, "add": false, "edit": true, "delete": false }
  ]
}
```

---

## 6. Testing & Observed Behavior

### Happy Path
1. **Auth**: Successful login returns JWT.
2. **Access Control**: Granular permissions are correctly stored in `RoleSoftwareModule` table.
3. **Data Integrity**: `EmployeeMaster` ensures unique `EmployeeCode` per company.

### Edge Cases
1. **Password Encryption**: All passwords stored as encrypted strings using `EncryptionHelper` (TripleDES/AES).
2. **Soft Deletion**: Most `DELETE` endpoints do not remove rows but set `ActiveStatus = 0`.
3. **Null Handling**: Repository patterns include null checks and return `NotFound(ApiResponse)` accordingly.

---

## 7. Issues & Improvements

### Security Concerns
- **TripleDES Obsolescence**: `EncryptionHelper` currently uses `TripleDESCryptoServiceProvider` which is marked as obsolete in .NET. Recommend migrating to **AES-256 (AesGcm)**.
- **Sensitive Data exposure**: `GET /api/EmployeeMaster/{id}` explicitly decrypts and returns the user's password to the frontend. This is a severe security risk. **Never return decrypted passwords to the client.**

### Design Inconsistencies
- **Response Codes**: `ExceptionMiddleware` overrides server errors (500) to return **HTTP 200 OK** with a JSON body indicating failure. This breaks standard HTTP status monitoring tools and should be corrected to return 500.
- **CamelCase Mismatch**: Some repositories use `SoftwareModulesMasterId` (camelCase) while the database might be expecting PascalCase or older aliases. Manual scaffolding cleanup was required.
- **Multipart Consistency**: `CompanyMaster` uses `[FromForm]` (Standard for file uploads), while others use `[FromBody]` (Default for JSON). Ensure frontend uses `FormData` correctly for Company calls.

### Suggested Features
- **Project Assignments**: Currently no mapping between Employees and Projects in the API.
- **Rate Limiting**: No rate limiting implemented on Login/SignUp. Recommend adding `AspNetCoreRateLimit`.

---

## Example Usage (JavaScript Fetch)
```javascript
const getToken = async (email, password) => {
  const response = await fetch('http://localhost:5083/api/Auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  const result = await response.json();
  return result.data.accessToken;
};
```

## Example Usage (Python)
```python
import requests

def get_employees(token, company_id):
    headers = {"Authorization": f"Bearer {token}"}
    url = f"http://localhost:5083/api/EmployeeMaster/list/{company_id}"
    r = requests.get(url, headers=headers)
    return r.json()
```
