# Mobile Spare Parts Management System - To-Do List

## ? = Complete | ?? = Pending | ?? = In Progress

---

## ?? Phase 1: Domain Layer

### Entity Classes
- ?? Create `src/MobileSparePartsManagement.Domain/Entities/User.cs`
  - Properties: Id (Guid), Email (string), PasswordHash (string), CreatedAt (DateTime)
- ?? Create `src/MobileSparePartsManagement.Domain/Entities/Supplier.cs`
  - Properties: Id (Guid), Name (string), ContactEmail (string?), Phone (string?), CreatedAt (DateTime)
- ?? Create `src/MobileSparePartsManagement.Domain/Entities/SparePart.cs`
  - Properties: Id (Guid), Name (string), Quantity (int), Price (decimal), SupplierId (Guid), Supplier (navigation), CreatedAt (DateTime)

### Cleanup
- ?? Remove `src/MobileSparePartsManagement.Domain/Class1.cs`

---

## ??? Phase 2: Infrastructure Layer

### Services
- ?? Create `src/MobileSparePartsManagement.Infrastructure/Services/PasswordService.cs`
  - Methods: HashPassword(string), VerifyPassword(string, string)
  - Use BCrypt.Net-Next
- ?? Create `src/MobileSparePartsManagement.Infrastructure/Services/TokenService.cs`
  - Method: GenerateToken(User) returns JWT string
  - Include claims: UserId, Email
  - 60-minute expiry

### Database Context
- ?? Create `src/MobileSparePartsManagement.Infrastructure/Data/AppDbContext.cs`
  - DbSets: Users, Suppliers, SpareParts
  - Override OnModelCreating
  - Apply configurations

### Entity Configurations
- ?? Create `src/MobileSparePartsManagement.Infrastructure/Data/Configurations/UserConfiguration.cs`
  - Unique index on Email
  - Required fields
  - Implements IEntityTypeConfiguration<User>
- ?? Create `src/MobileSparePartsManagement.Infrastructure/Data/Configurations/SupplierConfiguration.cs`
  - Required fields
  - Implements IEntityTypeConfiguration<Supplier>
- ?? Create `src/MobileSparePartsManagement.Infrastructure/Data/Configurations/SparePartConfiguration.cs`
  - Price decimal(10,2) precision
  - Foreign key: SupplierId ? Supplier.Id
  - Check constraints: Quantity >= 0, Price >= 0
  - Implements IEntityTypeConfiguration<SparePart>

### Cleanup
- ?? Remove `src/MobileSparePartsManagement.Infrastructure/Class1.cs`

### Migrations
- ?? Create initial migration
  ```bash
  dotnet ef migrations add InitialCreate --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
  ```

---

## ?? Phase 3: API Layer - DTOs

### Authentication DTOs
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Auth/LoginRequest.cs`
  - Properties: Email (required, email format), Password (required)
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Auth/LoginResponse.cs`
  - Properties: Token (string), User (UserDto)
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Auth/RegisterRequest.cs`
  - Properties: Email (required, email format), Password (required, min 6 chars)
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Auth/UserDto.cs`
  - Properties: Id (Guid), Email (string)

### Supplier DTOs
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Suppliers/SupplierDto.cs`
  - Properties: Id, Name, ContactEmail, Phone, CreatedAt
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Suppliers/CreateSupplierRequest.cs`
  - Properties: Name (required), ContactEmail (optional, email format), Phone (optional)
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Suppliers/UpdateSupplierRequest.cs`
  - Properties: Name (required), ContactEmail (optional, email format), Phone (optional)

### Spare Part DTOs
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/SpareParts/SparePartDto.cs`
  - Properties: Id, Name, Quantity, Price, SupplierId, SupplierName, CreatedAt
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/SpareParts/CreateSparePartRequest.cs`
  - Properties: Name (required), Quantity (>= 0), Price (>= 0), SupplierId (required)
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/SpareParts/UpdateSparePartRequest.cs`
  - Properties: Name (required), Quantity (>= 0), Price (>= 0), SupplierId (required)

### Common DTOs
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Common/PaginatedResult.cs`
  - Generic class: PaginatedResult<T>
  - Properties: Items (List<T>), TotalCount (int), Page (int), PageSize (int), TotalPages (int)
- ?? Create `src/MobileSparePartsManagement.Api/DTOs/Common/ErrorResponse.cs`
  - Properties: StatusCode (int), Message (string), Details (string?)

---

## ??? Phase 4: API Layer - Middleware & Extensions

### Middleware
- ?? Create `src/MobileSparePartsManagement.Api/Middleware/GlobalExceptionHandlingMiddleware.cs`
  - Catch all exceptions
  - Return ErrorResponse JSON
  - Log exceptions
  - Hide sensitive details in production

### Extensions (Optional)
- ?? Create `src/MobileSparePartsManagement.Api/Extensions/ServiceCollectionExtensions.cs`
  - Extension methods for organizing DI registration
  - AddJwtAuthentication, AddApplicationServices, etc.

---

## ?? Phase 5: API Layer - Controllers

### AuthController
- ?? Create `src/MobileSparePartsManagement.Api/Controllers/AuthController.cs`
  - Route: `/api/auth`
  - `POST /login` - Validate credentials, return JWT + user info
  - `POST /register` - Hash password, create user, return user info

### SuppliersController
- ?? Create `src/MobileSparePartsManagement.Api/Controllers/SuppliersController.cs`
  - Route: `/api/suppliers`
  - Add `[Authorize]` attribute
  - `GET /` - List suppliers with optional name filter
  - `GET /{id}` - Get single supplier by ID
  - `POST /` - Create new supplier
  - `PUT /{id}` - Update supplier
  - `DELETE /{id}` - Delete supplier (check for related spare parts)

### SparePartsController
- ?? Create `src/MobileSparePartsManagement.Api/Controllers/SparePartsController.cs`
  - Route: `/api/spareparts`
  - Add `[Authorize]` attribute
  - `GET /` - List with filtering, pagination, sorting
    - Query params: name, supplierId, page, pageSize, sortBy, sortDir
  - `GET /{id}` - Get single spare part by ID
  - `POST /` - Create new spare part
  - `PUT /{id}` - Update spare part
  - `DELETE /{id}` - Delete spare part

---

## ?? Phase 6: API Configuration

### NuGet Packages
- ?? Add `Swashbuckle.AspNetCore` to MobileSparePartsManagement.Api
  ```bash
  dotnet add src/MobileSparePartsManagement.Api package Swashbuckle.AspNetCore
  ```

### Configuration Files
- ?? Update `src/MobileSparePartsManagement.Api/appsettings.json`
  - Add ConnectionStrings:Default
  - Add Jwt section (Issuer, Audience, SecretKey)
  - Add Cors section (AllowedOrigins)
- ?? Create `src/MobileSparePartsManagement.Api/appsettings.Development.json`
  - Development-specific connection string
  - Override any dev-specific settings

### Program.cs
- ?? Update `src/MobileSparePartsManagement.Api/Program.cs`
  - **Services Configuration:**
    - Add DbContext with Npgsql provider
    - Add JWT Authentication
    - Add CORS policy
    - Add Controllers
    - Add Swagger/OpenAPI with JWT support
    - Register PasswordService (Scoped)
    - Register TokenService (Scoped)
  - **Middleware Pipeline:**
    - Use GlobalExceptionHandlingMiddleware (first)
    - Use HTTPS redirection
    - Use CORS
    - Use Authentication
    - Use Authorization
    - Use Swagger (in Development only)
    - Map Controllers

---

## ??? Phase 7: Database & Seeding

### Database Setup
- ?? Apply migration to create database
  ```bash
  dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
  ```

### Data Seeding (Optional)
- ?? Create seeding logic in AppDbContext or separate service
  - Seed admin user (email: admin@example.com, password: P@ssw0rd! - hashed)
  - Seed default supplier (name: "Default Supplier", email: supplier@example.com, phone: +1234567890)
- ?? Execute seeding on application startup or via CLI command

---

## ?? Phase 8: Testing & Documentation

### Build & Run
- ?? Build the solution
  ```bash
  dotnet build
  ```
- ?? Fix any build errors

- ?? Run the application
  ```bash
  dotnet run --project src/MobileSparePartsManagement.Api
  ```

### Testing with Swagger
- ?? Navigate to `https://localhost:<port>/swagger`
- ?? Test POST /api/auth/register (create test user)
- ?? Test POST /api/auth/login (obtain JWT token)
- ?? Click "Authorize" button in Swagger, enter: `Bearer <token>`
- ?? Test GET /api/suppliers (should require auth)
- ?? Test POST /api/suppliers (create supplier)
- ?? Test POST /api/spareparts (create spare part)
- ?? Test GET /api/spareparts with filters, pagination
- ?? Test PUT /api/spareparts/{id}
- ?? Test DELETE /api/spareparts/{id}
- ?? Test DELETE /api/suppliers/{id}
- ?? Verify validation errors return 400 with details
- ?? Verify unauthorized requests return 401

### Documentation
- ?? Create `README.md` at solution root
  - **Overview**: System description
  - **Prerequisites**: .NET 9 SDK, PostgreSQL
  - **Setup Instructions**:
    - Clone repository
    - Configure connection string
    - Run migrations
    - Run application
  - **Configuration**: Environment variables and settings
  - **API Documentation**: Swagger URL and usage
  - **Authentication**: How to obtain and use JWT tokens
  - **Project Structure**: Brief description of layers
  - **Development**: Seeding data, running in dev mode

---

## ?? Final Checklist

### Code Quality
- ?? All entities have proper navigation properties
- ?? All DTOs have proper validation attributes
- ?? No PasswordHash exposed in any DTO/response
- ?? All controllers use async/await
- ?? All database operations use async methods
- ?? Proper error handling in all controllers
- ?? Global exception middleware properly configured

### Security
- ?? JWT secret key is at least 32 characters
- ?? Passwords are hashed with BCrypt
- ?? All protected endpoints have [Authorize] attribute
- ?? CORS is properly configured
- ?? No sensitive data in error responses (production)

### Database
- ?? All migrations applied successfully
- ?? Foreign keys configured correctly
- ?? Indexes created (Email on User)
- ?? Check constraints working (Quantity >= 0, Price >= 0)
- ?? Decimal precision correct for Price (10,2)

### API Functionality
- ?? Authentication works (login returns valid JWT)
- ?? Registration creates users with hashed passwords
- ?? JWT tokens are validated on protected endpoints
- ?? CRUD operations work for Suppliers
- ?? CRUD operations work for SpareParts
- ?? Filtering works (by name, supplierId)
- ?? Pagination works correctly
- ?? Sorting works (by name, price, quantity)
- ?? Validation errors return proper 400 responses
- ?? 404 returned for non-existent resources
- ?? 401 returned for unauthorized requests

### Documentation
- ?? Swagger UI accessible and working
- ?? All endpoints documented in Swagger
- ?? JWT authorization in Swagger works
- ?? README.md complete with setup instructions
- ?? Connection string documented
- ?? Environment variables documented

---

## ?? Progress Tracker

**Total Tasks:** 79
**Completed:** 0
**In Progress:** 0
**Remaining:** 79

---

## ?? Quick Start Commands

```bash
# Build solution
dotnet build

# Add migration
dotnet ef migrations add InitialCreate --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api

# Update database
dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api

# Run application
dotnet run --project src/MobileSparePartsManagement.Api

# Add NuGet package
dotnet add src/MobileSparePartsManagement.Api package Swashbuckle.AspNetCore
```

---

## ?? Notes

- Keep .NET 9 (workspace is already on .NET 9)
- PostgreSQL must be running before applying migrations
- Update connection string in appsettings.json before running
- Test each phase before moving to the next
- Use Swagger for API testing during development

---

## ?? Estimated Time per Phase

- Phase 1 (Domain): 30 minutes
- Phase 2 (Infrastructure): 1 hour
- Phase 3 (DTOs): 45 minutes
- Phase 4 (Middleware): 30 minutes
- Phase 5 (Controllers): 1.5 hours
- Phase 6 (Configuration): 30 minutes
- Phase 7 (Database): 20 minutes
- Phase 8 (Testing & Docs): 1 hour

**Total Estimated Time:** 6 hours
