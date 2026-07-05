# DallyWorkReport API Documentation

This document provides a comprehensive overview of all API endpoints available in the **DallyWorkReport** system. All endpoints return a standardized `ApiResponse<T>` wrapper.

---

## 1. Authentication API
Endpoints for user authentication and session management.

### Login
*   **Controller**: `AuthController`
*   **Method**: `Login`
*   **Endpoint**: `[POST] /api/Auth/login` (Public)
*   **Description**: Authenticates a user and returns a JWT access token and refresh token.

#### Request Payload
```json
{
  "email": "admin@starrvixen.com",
  "password": "password123"
}
```

#### Response (200 OK)
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Token Generated",
  "data": {
    "accessToken": "eyJhbGciOi...",
    "refreshToken": "Vw7GgoAX...",
    "user": {
      "employeeID": 1,
      "userName": "Kavan Solanki",
      "email": "solankikavan134@gmail.com"
    }
  },
  "errors": null
}
```

### SignUp
*   **Controller**: `AuthController`
*   **Method**: `SignUp`
*   **Endpoint**: `[POST] /api/Auth/SignUp` (Public)

#### Request Payload
```json
{
  "companyName": "New Company Ltd",
  "email": "admin@newco.com",
  "password": "SecurePassword123",
  "confirmPassword": "SecurePassword123"
}
```

### Refresh Token
*   **Controller**: `AuthController`
*   **Method**: `RefreshToken`
*   **Endpoint**: `[POST] /api/Auth/RefreshToken` (Public)

---

## 2. Master Data API
Generic lookups and location data.

### Get Countries
*   **Controller**: `MasterDataController`
*   **Method**: `GetCountries`
*   **Endpoint**: `[GET] /api/MasterData/countries` (Authorized)

### Get States
*   **Controller**: `MasterDataController`
*   **Method**: `GetStates`
*   **Endpoint**: `[GET] /api/MasterData/states/{countryId}` (Authorized)

### Get Lookups
*   **Controller**: `MasterDataController`
*   **Method**: `GetLookups`
*   **Endpoint**: `[GET] /api/MasterData/lookups/{typeName}` (Authorized)
*   **Common Types**: `Gender`, `Role Type`.

---

## 3. Company Master API
Organization-level profile management.

### Get Company
*   **Controller**: `CompanyMasterController`
*   **Method**: `GetById`
*   **Endpoint**: `[GET] /api/CompanyMaster/{id}` (Authorized)

### Save/Update Company
*   **Controller**: `CompanyMasterController`
*   **Method**: `Save/Update`
*   **Endpoint**: `[POST/PUT] /api/CompanyMaster` (Authorized)
*   **Content-Type**: `multipart/form-data`

---

## 4. Employee Master API
Staff and member management.

### List Employees
*   **Controller**: `EmployeeMasterController`
*   **Method**: `List`
*   **Endpoint**: `[GET] /api/EmployeeMaster/list/{companyId}` (Authorized)

### Save Employee
*   **Controller**: `EmployeeMasterController`
*   **Method**: `Save`
*   **Endpoint**: `[POST] /api/EmployeeMaster/save` (Authorized)

---

## 5. Project Operational API
Core business entities for project tracking.

### List Projects
*   **Controller**: `ProjectMasterController`
*   **Method**: `List`
*   **Endpoint**: `[GET] /api/ProjectMaster/List/{companyId}` (Authorized)

### Save Status
*   **Controller**: `StatusMasterController`
*   **Method**: `Save`
*   **Endpoint**: `[POST] /api/StatusMaster/Save` (Authorized)

### List Clients
*   **Controller**: `ClientMasterController`
*   **Method**: `List`
*   **Endpoint**: `[GET] /api/ClientMaster/List/{companyId}` (Authorized)

### Save Module
*   **Controller**: `ModuleMasterController`
*   **Method**: `Save`
*   **Endpoint**: `[POST] /api/ModuleMaster/Save` (Authorized)

---

## 6. Access Control & Security
Fine-grained permissions and navigation.

### Role Hierarchy
*   **Controller**: `RoleMasterSoftwareModulesController`
*   **Method**: `GetHierarchy`
*   **Endpoint**: `[GET] /api/RoleMasterSoftwareModules/hierarchy/{roleId}` (Authorized)

### Update Permissions
*   **Controller**: `RoleMasterSoftwareModulesController`
*   **Method**: `UpdatePermissions`
*   **Endpoint**: `[POST] /api/RoleMasterSoftwareModules/save` (Authorized)

### Get Navigation Menu
*   **Controller**: `MenuController`
*   **Method**: `Get`
*   **Endpoint**: `[GET] /api/Menu/{roleId}/{isTenant?}` (Authorized)

---

## 7. Role Master API
Role structure management.

### List Roles
*   **Controller**: `RoleMasterController`
*   **Method**: `GetAll`
*   **Endpoint**: `[GET] /api/RoleMaster?companyId=4` (Authorized)

### Create Role
*   **Controller**: `RoleMasterController`
*   **Method**: `Create`
*   **Endpoint**: `[POST] /api/RoleMaster` (Authorized)
