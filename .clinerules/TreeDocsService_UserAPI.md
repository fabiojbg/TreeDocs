# TreeDocs Services - Nodes API Documentation

This document provides an overview of the REST APIs exposed by the `TreeDocs.Service` to register, login users and manager users

---

## 1. UserController

**Base Route**: `/api/User`

**Responsibility**: Manages user accounts, authentication, and user data operations.

**Authentication**: Most endpoints require authentication via `[Authorize]` and `[AuthenticationRequired]` filters, except for `CreateUser` and `Login`.

**Endpoints**:

### `GET /api/User/Authenticated`
- **Description**: Returns the information for the currently authenticated user.
- **Authentication**: Required
- **Parameters**: None
- **Returns**: `200 OK` with `AuthenticatedUser` object.
- **Contracts**:
    - **Response**: `AuthenticatedUser`
        - `Id` (string): The unique identifier of the user.
        - `Username` (string): The user's display name.
        - `Email` (string): The user's email address.
        - `Roles` (string[]): Array of user roles.
        - `Token` (string): The JWT authentication token.
        - `TokenExpiration` (DateTime?): Token expiration date/time.

### `GET /api/User/{userEmail}`
- **Description**: Retrieves user details by email.
- **Authentication**: Required
- **Parameters**:
    - `userEmail` (`string`, Path Parameter): The email of the user to retrieve.
- **Returns**: `200 OK` with the requested user's details, or an error status code.
- **Contracts**:
    - **Request**: `GetUserRequest`
        - `Email` (string): The email of the user to retrieve.
    - **Response**: `GetUserResponse`
        - `Id` (string): The unique identifier of the user.
        - `Name` (string): The user's display name.
        - `Email` (string): The user's email address.
        - `Roles` (string[]): Array of user roles.

### `GET /api/User/test/{userEmail}`
- **Description**: A test endpoint demonstrating how background jobs can be executed asynchronously while the request finishes immediately.
- **Authentication**: Required
- **Parameters**:
    - `userEmail` (`string`, Path Parameter): The email of the user for the background job.
- **Returns**: `200 OK` with a success message.
- **Contracts**:
    - **Request**: `GetUserRequest`
        - `Email` (string): The email of the user for the background job.
    - **Response**: `string`
        - "Teste" message indicating successful request handling.

### `POST /api/User`
- **Description**: Creates a new user account. This endpoint allows anonymous access.
- **Authentication**: None (Anonymous access)
- **Returns**: `201 Created` with the new user's data on success, or an error status code if creation fails.
- **Contracts**:
    - **Request**: `CreateUserRequest`
        - `Name` (string): The user's display name.
        - `Email` (string): The user's email address.
        - `Password` (string): The user's password.
        - `Roles` (string[]): Array of initial roles (typically ["User"]).
    - **Response**: `CreateUserResponse`
        - `Id` (string): The unique identifier of the created user.
        - `Name` (string): The user's display name.
        - `Email` (string): The user's email address.
        - `Roles` (string[]): Array of user roles.

### `PUT /api/User`
- **Description**: Updates an existing user's profile information.
- **Authentication**: Required
- **Returns**: `200 OK` with the result of the update, or an error status code.
- **Contracts**:
    - **Request**: `UpdateUserDataRequest`
        - `UserId` (string): The unique identifier of the user to update.
        - `Name` (string): The new display name for the user.
        - `Email` (string): The new email address for the user.
        - `Roles` (List<string>): Array of roles (only modifiable by administrators).
    - **Response**: `UpdateUserDataResponse`
        - `Id` (string): The unique identifier of the updated user.
        - `Name` (string): The user's updated display name.
        - `Email` (string): The user's updated email address.
        - `Roles` (IEnumerable<string>): Array of user roles.

### `PUT /api/User/ChangePassword`
- **Description**: Changes a user's password.
- **Authentication**: Required
- **Returns**: `200 OK` with the result of the password change, or an error status code.
- **Contracts**:
    - **Request**: `UpdateUserPasswordRequest`
        - `UserId` (string): The unique identifier of the user (optional if UserEmail is provided).
        - `UserEmail` (string): The email of the user (optional if UserId is provided).
        - `OldPassword` (string): The user's current password.
        - `NewPassword` (string): The new password to set.
    - **Response**: `UpdateUserPasswordResponse`
        - (Empty response indicating successful password change)

### `POST /api/User/Login`
- **Description**: Authenticates a user and issues a JWT token. This endpoint allows anonymous access. The generated token has a 7-day expiration.
- **Authentication**: None (Anonymous access)
- **Returns**: `200 OK` with `AuthenticatedUser` object containing the generated JWT token and its expiration, or an error status code if authentication fails.
- **Contracts**:
    - **Request**: `AuthenticateUserRequest`
        - `UserEmail` (string): The user's email address.
        - `Password` (string): The user's password.
    - **Response**: `AuthenticatedUser`
        - `Id` (string): The unique identifier of the user.
        - `Username` (string): The user's display name.
        - `Email` (string): The user's email address.
        - `Roles` (string[]): Array of user roles.
        - `Token` (string): The generated JWT authentication token.
        - `TokenExpiration` (DateTime?): Token expiration date/time.

---
