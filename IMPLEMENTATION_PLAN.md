# Mobile Spare Parts Management System - Implementation Plan

## Tech Stack
- **.NET 8 Web API** (Note: Current workspace is .NET 9, will adapt to .NET 8 as requested)
- **Entity Framework Core** (Npgsql provider for PostgreSQL)
- **JWT Bearer Authentication**
- **BCrypt password hashing** (`BCrypt.Net-Next`)
- **Swagger (Swashbuckle)** for API documentation
- **CORS** configured for Angular origin

---

## Entities

### User
- `Id` (Guid, Primary Key)
- `Email` (string, unique, required)
- `PasswordHash` (string, required)
- `CreatedAt` (DateTime, required)

### Supplier
- `Id` (Guid, Primary Key)
- `Name` (string, required)
- `ContactEmail` (string?, nullable)
- `Phone` (string?, nullable)
- `CreatedAt` (DateTime, required)

### SparePart
- `Id` (Guid, Primary Key)
- `Name` (string, required)
- `Quantity` (int, >= 0)
- `Price` (decimal(10,2), >= 0)
- `SupplierId` (Guid, Foreign Key)
- `CreatedAt` (DateTime, required)
- Navigation: `Supplier` (reference)

---

## Database Context

### AppDbContext
- **DbSets**: `Users`, `Suppliers`, `SpareParts`
- **Fluent API Configurations**:
  - User: Unique index on Email
  - SparePart: Price precision (10,2), check constraints for Quantity >= 0 and Price >= 0
  - Foreign key relationship: SparePart.SupplierId ? Supplier.Id
- **Provider**: PostgreSQL (Npgsql)
- **Migrations**: To be created and applied

---

## DTOs

### Authentication DTOs
- `LoginRequest`
  - `Email` (string, required, email format)
  - `Password` (string, required)
  
- `LoginResponse`
  - `Token` (string)
  - `User` (object with Id, Email)

- `RegisterRequest` (optional for seeding)
  - `Email` (string, required, email format)
  - `Password` (string, required, min length)

### Supplier DTOs
- `SupplierDto`
  - `Id` (Guid)
  - `Name` (string)
  - `ContactEmail` (string?)
  - `Phone` (string?)
  - `CreatedAt` (DateTime)

- `CreateSupplierRequest`
  - `Name` (string, required)
  - `ContactEmail` (string?, optional, email format if provided)
  - `Phone` (string?, optional)

- `UpdateSupplierRequest`
  - `Name` (string, required)
  - `ContactEmail` (string?, optional, email format if provided)
  - `Phone` (string?, optional)

### Spare Part DTOs
- `SparePartDto`
  - `Id` (Guid)
  - `Name` (string)
  - `Quantity` (int)
  - `Price` (decimal)
  - `SupplierId` (Guid)
  - `SupplierName` (string)
  - `CreatedAt` (DateTime)

- `CreateSparePartRequest`
  - `Name` (string, required)
  - `Quantity` (int, >= 0)
  - `Price` (decimal, >= 0)
  - `SupplierId` (Guid, required)

- `UpdateSparePartRequest`
  - `Name` (string, required)
  - `Quantity` (int, >= 0)
  - `Price` (decimal, >= 0)
  - `SupplierId` (Guid, required)

---

## Authentication

### Endpoints
- `POST /api/auth/login`
  - Validates credentials (email + password)
  - Returns JWT token + user info (id, email)
  - Returns 401 if invalid credentials

- `POST /api/auth/register` (optional, for seeding)
  - Hashes password with BCrypt
  - Creates new user
  - Returns created user info

### JWT Configuration
- **Settings** (from appsettings.json or environment):
  - `Jwt:Issuer`
  - `Jwt:Audience`
  - `Jwt:SecretKey` (minimum 32 characters for HS256)
- **Token Expiry**: 60 minutes
- **Claims**: UserId, Email
- **Protection**: Use `[Authorize]` attribute on protected controllers/actions

---

## Spare Parts API (Authorized)

All endpoints require JWT authentication (`[Authorize]`)

- `GET /api/spareparts`
  - Query parameters:
    - `name` (string?, optional filter)
    - `supplierId` (Guid?, optional filter)
    - `page` (int, default 1)
    - `pageSize` (int, default 10)
    - `sortBy` (string?, e.g., "name", "price", "quantity")
    - `sortDir` (string?, "asc" or "desc", default "asc")
  - Returns: Paginated list of SparePartDto

- `GET /api/spareparts/{id}`
  - Returns: Single SparePartDto or 404

- `POST /api/spareparts`
  - Body: CreateSparePartRequest
  - Validation: Non-negative Quantity and Price
  - Returns: 201 Created with SparePartDto

- `PUT /api/spareparts/{id}`
  - Body: UpdateSparePartRequest
  - Validation: Non-negative Quantity and Price
  - Returns: 200 OK with updated SparePartDto or 404

- `DELETE /api/spareparts/{id}`
  - Returns: 204 No Content or 404

---

## Suppliers API (Authorized)

All endpoints require JWT authentication (`[Authorize]`)

- `GET /api/suppliers`
  - Query parameters:
    - `name` (string?, optional filter)
  - Returns: List of SupplierDto

- `GET /api/suppliers/{id}`
  - Returns: Single SupplierDto or 404

- `POST /api/suppliers`
  - Body: CreateSupplierRequest
  - Returns: 201 Created with SupplierDto

- `PUT /api/suppliers/{id}`
  - Body: UpdateSupplierRequest
  - Returns: 200 OK with updated SupplierDto or 404

- `DELETE /api/suppliers/{id}`
  - Returns: 204 No Content or 404
  - Note: Should check for related spare parts before deletion

---

## Validation & Error Handling

### Validation
- Use **DataAnnotations** on DTOs (Required, EmailAddress, Range, MinLength, etc.)
- Automatic 400 responses with validation details
- Custom validation for business rules (e.g., supplier exists when creating spare part)

### Global Exception Handling
- Middleware to catch unhandled exceptions
- Standardized error response format:
  ```json
  {
    "statusCode": 500,
    "message": "Error message",
    "details": "Additional details (only in Development)"
  }
  ```
- Prevent exposing sensitive data (e.g., PasswordHash) in responses

---

## CORS Configuration

- **Allowed Origins**: `http://localhost:4200` (Angular development server)
- **Allowed Methods**: GET, POST, PUT, DELETE
- **Allowed Headers**: `Content-Type`, `Authorization`
- Enable credentials if needed

---

## Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=MobileSpareParts;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Issuer": "MobileSparePartsAPI",
    "Audience": "MobileSparePartsClient",
    "SecretKey": "YourSuperSecretKeyMinimum32Characters!"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables (alternative)
- `ConnectionStrings__Default`
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SecretKey`

---

## Seeding (Optional)

### Default Data
- **Admin User**:
  - Email: `admin@example.com`
  - Password: `P@ssw0rd!` (hashed with BCrypt)
  
- **Default Supplier**:
  - Name: "Default Supplier"
  - ContactEmail: "supplier@example.com"
  - Phone: "+1234567890"

### Implementation
- Seed data in `AppDbContext.OnModelCreating` or via a separate seeding service
- Check for existing data before seeding

---

## Project Structure

### MobileSparePartsManagement.Domain
**Path**: `src/MobileSparePartsManagement.Domain/`

**Files to Create**:
- `Entities/User.cs`
- `Entities/Supplier.cs`
- `Entities/SparePart.cs`

**Files to Remove**:
- `Class1.cs` (template file)

---

### MobileSparePartsManagement.Infrastructure
**Path**: `src/MobileSparePartsManagement.Infrastructure/`

**Files to Create**:
- `Data/AppDbContext.cs`
- `Data/Configurations/UserConfiguration.cs`
- `Data/Configurations/SupplierConfiguration.cs`
- `Data/Configurations/SparePartConfiguration.cs`
- `Services/TokenService.cs` (JWT generation)
- `Services/PasswordService.cs` (BCrypt hashing/verification)
- `Migrations/` (via EF Core CLI)

**Files to Remove**:
- `Class1.cs` (template file)

**NuGet Packages** (Already installed):
- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4)
- `BCrypt.Net-Next` (4.0.3)
- `Microsoft.EntityFrameworkCore.Design` (9.0.0)

---

### MobileSparePartsManagement.Api
**Path**: `src/MobileSparePartsManagement.Api/`

**Files to Create**:
- `Controllers/AuthController.cs`
- `Controllers/SuppliersController.cs`
- `Controllers/SparePartsController.cs`
- `DTOs/Auth/LoginRequest.cs`
- `DTOs/Auth/LoginResponse.cs`
- `DTOs/Auth/RegisterRequest.cs`
- `DTOs/Auth/UserDto.cs`
- `DTOs/Suppliers/SupplierDto.cs`
- `DTOs/Suppliers/CreateSupplierRequest.cs`
- `DTOs/Suppliers/UpdateSupplierRequest.cs`
- `DTOs/SpareParts/SparePartDto.cs`
- `DTOs/SpareParts/CreateSparePartRequest.cs`
- `DTOs/SpareParts/UpdateSparePartRequest.cs`
- `DTOs/Common/PaginatedResult.cs`
- `DTOs/Common/ErrorResponse.cs`
- `Middleware/GlobalExceptionHandlingMiddleware.cs`
- `Extensions/ServiceCollectionExtensions.cs` (optional, for organizing DI)
- `appsettings.Development.json` (with connection string)

**Files to Modify**:
- `Program.cs` (complete rewrite for services, middleware, JWT, CORS, Swagger)
- `appsettings.json` (add connection strings, JWT config, CORS config)

**NuGet Packages to Add**:
- `Swashbuckle.AspNetCore` (for Swagger/OpenAPI)

**NuGet Packages Already Installed**:
- `Microsoft.AspNetCore.Authentication.JwtBearer` (9.0.0)
- `Microsoft.AspNetCore.OpenApi` (9.0.8)

---

## Implementation Steps

### Phase 1: Domain Layer
1. **Create Entity Classes**
   - `User.cs` with Id, Email, PasswordHash, CreatedAt
   - `Supplier.cs` with Id, Name, ContactEmail, Phone, CreatedAt
   - `SparePart.cs` with Id, Name, Quantity, Price, SupplierId, Supplier navigation, CreatedAt
2. **Remove** `Class1.cs` from Domain project

### Phase 2: Infrastructure Layer
1. **Create Services**
   - `PasswordService.cs`: BCrypt hashing and verification methods
   - `TokenService.cs`: JWT token generation with claims

2. **Create AppDbContext**
   - `AppDbContext.cs` with DbSets and OnModelCreating

3. **Create EF Configurations**
   - `UserConfiguration.cs`: Unique email index, required fields
   - `SupplierConfiguration.cs`: Required fields
   - `SparePartConfiguration.cs`: Precision for Price, foreign key, check constraints

4. **Remove** `Class1.cs` from Infrastructure project

5. **Add Migration** (after all configurations are done)
   ```bash
   dotnet ef migrations add InitialCreate --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
   ```

### Phase 3: API Layer - DTOs
1. **Authentication DTOs**
   - `LoginRequest.cs`, `LoginResponse.cs`, `RegisterRequest.cs`, `UserDto.cs`

2. **Supplier DTOs**
   - `SupplierDto.cs`, `CreateSupplierRequest.cs`, `UpdateSupplierRequest.cs`

3. **Spare Part DTOs**
   - `SparePartDto.cs`, `CreateSparePartRequest.cs`, `UpdateSparePartRequest.cs`

4. **Common DTOs**
   - `PaginatedResult.cs` (generic for pagination)
   - `ErrorResponse.cs` (for standardized errors)

### Phase 4: API Layer - Middleware & Extensions
1. **Global Exception Handling Middleware**
   - `GlobalExceptionHandlingMiddleware.cs`
   - Catch exceptions, log them, return standardized JSON errors

2. **Service Extensions** (optional)
   - `ServiceCollectionExtensions.cs` for organizing dependency injection

### Phase 5: API Layer - Controllers
1. **AuthController**
   - `POST /api/auth/login`: Validate credentials, generate JWT
   - `POST /api/auth/register`: Create user with hashed password

2. **SuppliersController** (with `[Authorize]`)
   - CRUD endpoints for suppliers

3. **SparePartsController** (with `[Authorize]`)
   - CRUD + filtering/pagination/sorting endpoints

### Phase 6: API Configuration
1. **Update NuGet Packages**
   - Add `Swashbuckle.AspNetCore` for Swagger

2. **Update appsettings.json**
   - Add ConnectionStrings, Jwt, Cors sections

3. **Create appsettings.Development.json**
   - Development-specific connection string

4. **Rewrite Program.cs**
   - Add services:
     - DbContext (Npgsql)
     - JWT Authentication
     - CORS
     - Controllers
     - Swagger with JWT support
     - Scoped services (PasswordService, TokenService)
   - Configure middleware pipeline:
     - Exception handling
     - HTTPS redirection
     - CORS
     - Authentication/Authorization
     - Swagger (in Development)
     - MapControllers

### Phase 7: Database & Seeding
1. **Apply Migration**
   ```bash
   dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
   ```

2. **Optional: Implement Seeding**
   - Seed admin user and default supplier
   - Run seeding logic on application startup or via separate command

### Phase 8: Testing & Documentation
1. **Build Solution**
   ```bash
   dotnet build
   ```

2. **Run Application**
   ```bash
   dotnet run --project src/MobileSparePartsManagement.Api
   ```

3. **Test with Swagger**
   - Navigate to `https://localhost:<port>/swagger`
   - Test login, obtain JWT token
   - Use "Authorize" button to add Bearer token
   - Test protected endpoints

4. **Create README.md**
   - Setup instructions
   - Database migration steps
   - Running the application
   - API documentation overview
   - Environment configuration

---

## Deliverables

### Solution Structure
```
Mobile/
??? src/
?   ??? MobileSparePartsManagement.Domain/
?   ?   ??? Entities/
?   ?   ?   ??? User.cs
?   ?   ?   ??? Supplier.cs
?   ?   ?   ??? SparePart.cs
?   ?   ??? MobileSparePartsManagement.Domain.csproj
?   ?
?   ??? MobileSparePartsManagement.Infrastructure/
?   ?   ??? Data/
?   ?   ?   ??? AppDbContext.cs
?   ?   ?   ??? Configurations/
?   ?   ?       ??? UserConfiguration.cs
?   ?   ?       ??? SupplierConfiguration.cs
?   ?   ?       ??? SparePartConfiguration.cs
?   ?   ??? Services/
?   ?   ?   ??? TokenService.cs
?   ?   ?   ??? PasswordService.cs
?   ?   ??? Migrations/
?   ?   ??? MobileSparePartsManagement.Infrastructure.csproj
?   ?
?   ??? MobileSparePartsManagement.Api/
?       ??? Controllers/
?       ?   ??? AuthController.cs
?       ?   ??? SuppliersController.cs
?       ?   ??? SparePartsController.cs
?       ??? DTOs/
?       ?   ??? Auth/
?       ?   ??? Suppliers/
?       ?   ??? SpareParts/
?       ?   ??? Common/
?       ??? Middleware/
?       ?   ??? GlobalExceptionHandlingMiddleware.cs
?       ??? Extensions/
?       ?   ??? ServiceCollectionExtensions.cs
?       ??? Program.cs
?       ??? appsettings.json
?       ??? appsettings.Development.json
?       ??? MobileSparePartsManagement.Api.csproj
?
??? IMPLEMENTATION_PLAN.md
??? README.md
```

### Key Files

#### Program.cs Setup
- EF Core with PostgreSQL (Npgsql)
- JWT Authentication with Bearer scheme
- CORS policy for Angular
- Swagger/OpenAPI with JWT authorization UI
- Global exception handling middleware
- HTTPS redirection
- Controllers with API endpoints

#### README.md Sections
1. **Overview**: Brief description of the system
2. **Prerequisites**: .NET 8 SDK, PostgreSQL
3. **Setup Instructions**:
   - Clone repository
   - Update connection string in appsettings.json
   - Run migrations: `dotnet ef database update`
   - Run application: `dotnet run`
4. **Configuration**: Environment variables and settings
5. **API Documentation**: Link to Swagger UI
6. **Authentication**: How to obtain and use JWT tokens
7. **Development**: Running in development mode, seeding data

---

## Notes

### .NET Version Adaptation
- Requirements specify .NET 8, but workspace is currently .NET 9
- **Options**:
  1. Downgrade all projects to .NET 8 (change `<TargetFramework>net9.0</TargetFramework>` to `net8.0`)
  2. Keep .NET 9 (compatible with .NET 8 patterns, just newer runtime)
- **Recommendation**: Keep .NET 9 unless there's a specific requirement for .NET 8 runtime

### Security Considerations
- Never expose `PasswordHash` in API responses
- Use strong JWT secret keys (minimum 32 characters)
- Validate all inputs with DataAnnotations
- Use parameterized queries (EF Core handles this)
- Implement rate limiting for authentication endpoints (optional enhancement)

### Performance Considerations
- Implement pagination for list endpoints
- Use async/await for all database operations
- Add database indexes on frequently queried fields (Email, SupplierId, Name)
- Consider caching for supplier list (optional enhancement)

### Future Enhancements
- Refresh tokens for JWT
- Role-based authorization (Admin, User)
- Audit logging (CreatedBy, UpdatedAt, UpdatedBy)
- Soft delete support
- Unit and integration tests
- Docker support
- CI/CD pipeline

---

## Ready for Implementation

This plan provides a comprehensive roadmap for building the Mobile Spare Parts Management system. Once approved, implementation will proceed in the phases outlined above.

**Estimated Time**: 4-6 hours for full implementation (excluding testing and refinements)

**Next Step**: Await approval to begin Phase 1 (Domain Layer)
