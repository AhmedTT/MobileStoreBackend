# Mobile Spare Parts Management System

A comprehensive .NET 9 Web API for managing mobile spare parts inventory with suppliers, featuring JWT authentication and PostgreSQL persistence.

## ?? Overview

This system provides a RESTful API to manage mobile spare parts, suppliers, and user authentication. It includes features like filtering, pagination, sorting, and secure JWT-based authentication.

## ?? Features

- **JWT Authentication** - Secure token-based authentication with 60-minute expiry
- **User Management** - Registration and login with BCrypt password hashing
- **Supplier Management** - Full CRUD operations for suppliers
- **Spare Parts Management** - Complete inventory management with filtering, pagination, and sorting
- **PostgreSQL Database** - Robust relational database with Entity Framework Core
- **Swagger Documentation** - Interactive API documentation
- **CORS Support** - Configured for Angular frontend integration
- **Global Exception Handling** - Standardized error responses

## ??? Prerequisites

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL** - [Download](https://www.postgresql.org/download/)
- **Git** (optional) - For version control

## ?? Tech Stack

- **Framework**: .NET 9 Web API
- **Database**: PostgreSQL 
- **ORM**: Entity Framework Core 9 with Npgsql provider
- **Authentication**: JWT Bearer Authentication
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swashbuckle (Swagger/OpenAPI)

## ??? Project Structure

```
Mobile/
??? src/
?   ??? MobileSparePartsManagement.Domain/       # Entity models
?   ?   ??? Entities/
?   ?       ??? User.cs
?   ?       ??? Supplier.cs
?   ?       ??? SparePart.cs
?   ?
?   ??? MobileSparePartsManagement.Infrastructure/   # Data access & services
?   ?   ??? Data/
?   ?   ?   ??? AppDbContext.cs
?   ?   ?   ??? Configurations/          # EF Core configurations
?   ?   ??? Services/
?   ?       ??? PasswordService.cs       # BCrypt hashing
?   ?       ??? TokenService.cs          # JWT generation
?   ?
?   ??? MobileSparePartsManagement.Api/          # Web API layer
?       ??? Controllers/                 # API endpoints
?       ??? DTOs/                        # Data transfer objects
?       ??? Middleware/                  # Exception handling
?       ??? Program.cs                   # Application startup
?
??? IMPLEMENTATION_PLAN.md
??? TODO.md
??? README.md
```

## ?? Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Mobile
```

### 2. Configure Database Connection

Update the connection string in `src/MobileSparePartsManagement.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=MobileSpareParts;Username=postgres;Password=yourpassword"
  }
}
```

Or for development, update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=MobileSpareParts;Username=postgres;Password=postgres"
  }
}
```

### 3. Configure JWT Settings

The JWT configuration is already set in `appsettings.json`. **Important**: Change the `SecretKey` in production!

```json
{
  "Jwt": {
    "Issuer": "MobileSparePartsAPI",
    "Audience": "MobileSparePartsClient",
    "SecretKey": "YourSuperSecretKeyMinimum32CharactersLong!!"
  }
}
```

### 4. Install Entity Framework Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

### 5. Create and Apply Database Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api

# Apply migration to create database
dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

### 6. Run the Application

```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger**: `https://localhost:5001/swagger`

## ?? API Documentation

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "guid",
    "email": "user@example.com"
  }
}
```

### Suppliers Endpoints (Authorized)

All supplier endpoints require JWT authentication. Include the token in the `Authorization` header:
```
Authorization: Bearer <your-jwt-token>
```

- `GET /api/suppliers` - List all suppliers (optional query: `?name=search`)
- `GET /api/suppliers/{id}` - Get supplier by ID
- `POST /api/suppliers` - Create new supplier
- `PUT /api/suppliers/{id}` - Update supplier
- `DELETE /api/suppliers/{id}` - Delete supplier

### Spare Parts Endpoints (Authorized)

- `GET /api/spareparts` - List spare parts with filtering and pagination
  - Query parameters:
    - `name` - Filter by part name
    - `supplierId` - Filter by supplier
    - `page` - Page number (default: 1)
    - `pageSize` - Items per page (default: 10)
    - `sortBy` - Sort field (name, price, quantity)
    - `sortDir` - Sort direction (asc, desc)
- `GET /api/spareparts/{id}` - Get spare part by ID
- `POST /api/spareparts` - Create new spare part
- `PUT /api/spareparts/{id}` - Update spare part
- `DELETE /api/spareparts/{id}` - Delete spare part

## ?? Using the API with Swagger

1. Navigate to `https://localhost:5001/swagger`
2. Register a new user via `POST /api/auth/register`
3. Login via `POST /api/auth/login` and copy the token from the response
4. Click the **"Authorize"** button at the top of Swagger UI
5. Enter: `Bearer <your-token-here>` (include the word "Bearer")
6. Click "Authorize" and then "Close"
7. Now you can test all protected endpoints

## ??? Database Schema

### Users Table
- `Id` (UUID, PK)
- `Email` (Unique, Required)
- `PasswordHash` (Required)
- `CreatedAt` (DateTime)

### Suppliers Table
- `Id` (UUID, PK)
- `Name` (Required)
- `ContactEmail` (Optional)
- `Phone` (Optional)
- `CreatedAt` (DateTime)

### SpareParts Table
- `Id` (UUID, PK)
- `Name` (Required)
- `Quantity` (Integer, >= 0)
- `Price` (Decimal(10,2), >= 0)
- `SupplierId` (UUID, FK ? Suppliers)
- `CreatedAt` (DateTime)

## ?? Development

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Creating New Migrations

```bash
dotnet ef migrations add <MigrationName> --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

### Reverting Migrations

```bash
dotnet ef database update <PreviousMigrationName> --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

## ?? CORS Configuration

The API is configured to accept requests from Angular development server:
- Default origin: `http://localhost:4200`

To modify allowed origins, update `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200", "https://your-production-url.com"]
  }
}
```

## ?? Security Considerations

- **Password Security**: Passwords are hashed using BCrypt before storage
- **JWT Tokens**: 60-minute expiry, configurable in `TokenService.cs`
- **HTTPS**: Enforced in production
- **CORS**: Restricted to specific origins
- **Sensitive Data**: PasswordHash is never exposed in API responses

## ?? Environment Variables

You can override configuration using environment variables:

```bash
# Connection String
ConnectionStrings__Default="Host=localhost;Database=MobileSpareParts;Username=postgres;Password=yourpassword"

# JWT Settings
Jwt__SecretKey="YourProductionSecretKey"
Jwt__Issuer="MobileSparePartsAPI"
Jwt__Audience="MobileSparePartsClient"
```

## ?? Troubleshooting

### Database Connection Issues
- Ensure PostgreSQL is running
- Verify connection string credentials
- Check if database exists: `dotnet ef database update`

### Migration Errors
- Delete `Migrations` folder and recreate: `dotnet ef migrations add InitialCreate`
- Ensure Infrastructure project builds successfully

### JWT Token Issues
- Verify token hasn't expired (60-minute lifetime)
- Ensure token is formatted as: `Bearer <token>`
- Check JWT secret key is at least 32 characters

## ?? License

This project is licensed under the MIT License.

## ?? Contributors

- Development Team

## ?? Support

For issues and questions, please open an issue in the repository.

---

**Happy Coding! ??**
