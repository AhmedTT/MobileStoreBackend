# ?? Quick Start Guide - Mobile Spare Parts Management API

## ? Issue Fixed: Port Conflict Resolved

The application was failing because **port 5055 was already in use**. This has been fixed by changing to standard ports:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

---

## ?? Running the Application

### Step 1: Start the Application

Choose one of these methods:

**Method 1: Visual Studio**
- Press **F5** or click the green "Play" button
- Select the "https" profile

**Method 2: Command Line**
```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

**Method 3: Visual Studio Code**
- Open terminal (Ctrl + `)
- Run: `dotnet run --project src/MobileSparePartsManagement.Api`

### Step 2: Verify It's Running

You should see output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Step 3: Access the API

**OpenAPI Endpoint:**
- `https://localhost:5001/openapi/v1.json`

This will show you the OpenAPI specification (JSON format) of your API.

---

## ?? Before First Use: Create the Database

### Step 1: Ensure PostgreSQL is Running

Make sure PostgreSQL is installed and running on your machine.

### Step 2: Update Connection String (if needed)

Edit `src/MobileSparePartsManagement.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=MobileSpareParts;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
  }
}
```

### Step 3: Create Migration

```bash
dotnet ef migrations add InitialCreate --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

### Step 4: Apply Migration

```bash
dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

---

## ?? Testing the API

### Option 1: Using the HTTP File (Recommended)

1. Open `src/MobileSparePartsManagement.Api/MobileSparePartsManagement.Api.http`
2. Click "Send Request" above each endpoint (VS Code with REST Client extension, or Visual Studio)

**Test Sequence:**
1. Register a user
2. Login (copy the token from response)
3. Replace `YOUR_TOKEN_HERE` with your actual token
4. Test protected endpoints

### Option 2: Using cURL

**Register:**
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Password123!"}'
```

**Login:**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Password123!"}'
```

**Get Suppliers (with token):**
```bash
curl -X GET https://localhost:5001/api/suppliers \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Option 3: Using Postman

1. Import the OpenAPI spec from `https://localhost:5001/openapi/v1.json`
2. Or manually create requests as shown in the HTTP file

---

## ?? Authentication Flow

### 1. Register a User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "id": "guid-here",
  "email": "user@example.com"
}
```

### 2. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "guid-here",
    "email": "user@example.com"
  }
}
```

### 3. Use Token in Protected Endpoints

Add this header to all protected requests:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ?? Available Endpoints

### Authentication (Public)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token

### Suppliers (Protected - Requires JWT)
- `GET /api/suppliers` - List all suppliers
  - Query: `?name=search`
- `GET /api/suppliers/{id}` - Get supplier by ID
- `POST /api/suppliers` - Create new supplier
- `PUT /api/suppliers/{id}` - Update supplier
- `DELETE /api/suppliers/{id}` - Delete supplier

### Spare Parts (Protected - Requires JWT)
- `GET /api/spareparts` - List spare parts (with pagination)
  - Query: `?name=search&supplierId=guid&page=1&pageSize=10&sortBy=name&sortDir=asc`
- `GET /api/spareparts/{id}` - Get spare part by ID
- `POST /api/spareparts` - Create new spare part
- `PUT /api/spareparts/{id}` - Update spare part
- `DELETE /api/spareparts/{id}` - Delete spare part

---

## ?? Common Issues & Solutions

### Issue 1: Port Already in Use
**Error:** `Failed to bind to address http://127.0.0.1:5000`

**Solution:** 
- Change ports in `launchSettings.json`
- Or kill the process using that port:
  ```bash
  # Windows
  netstat -ano | findstr :5000
  taskkill /PID <process_id> /F
  
  # Linux/Mac
  lsof -ti:5000 | xargs kill -9
  ```

### Issue 2: Database Connection Failed
**Error:** `Npgsql.NpgsqlException: Failed to connect`

**Solution:**
1. Ensure PostgreSQL is running
2. Check connection string in `appsettings.Development.json`
3. Verify username and password

### Issue 3: Table Does Not Exist
**Error:** `relation "Users" does not exist`

**Solution:**
```bash
dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

### Issue 4: Unauthorized (401)
**Error:** HTTP 401 response

**Solution:**
1. Ensure you're logged in and have a valid token
2. Check token hasn't expired (60-minute lifetime)
3. Verify token is formatted as: `Bearer <token>`

---

## ?? Testing Workflow Example

### 1. Start the Application
```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

### 2. Register and Login
Use the HTTP file or cURL to register and login.

### 3. Create a Supplier
```http
POST https://localhost:5001/api/suppliers
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "name": "ABC Electronics",
  "contactEmail": "contact@abc.com",
  "phone": "+1234567890"
}
```

**Copy the `id` from the response.**

### 4. Create a Spare Part
```http
POST https://localhost:5001/api/spareparts
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "name": "iPhone 14 Screen",
  "quantity": 50,
  "price": 99.99,
  "supplierId": "PASTE_SUPPLIER_ID_HERE"
}
```

### 5. List Spare Parts with Filtering
```http
GET https://localhost:5001/api/spareparts?page=1&pageSize=10&sortBy=price&sortDir=desc
Authorization: Bearer YOUR_TOKEN
```

---

## ??? Development Commands

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

### Watch (auto-reload on changes)
```bash
dotnet watch --project src/MobileSparePartsManagement.Api
```

### Create Migration
```bash
dotnet ef migrations add MigrationName --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

### Apply Migrations
```bash
dotnet ef database update --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

### Remove Last Migration
```bash
dotnet ef migrations remove --project src/MobileSparePartsManagement.Infrastructure --startup-project src/MobileSparePartsManagement.Api
```

---

## ? Success Indicators

When everything is working correctly:

1. ? Application starts without errors
2. ? Can access `https://localhost:5001/openapi/v1.json`
3. ? Register endpoint returns 201 Created
4. ? Login endpoint returns 200 OK with token
5. ? Protected endpoints return 401 without token
6. ? Protected endpoints return 200 with valid token

---

## ?? Need Help?

If you encounter any issues:
1. Check the error message in the console
2. Verify PostgreSQL is running
3. Ensure migrations are applied
4. Check the connection string
5. Verify the JWT token is valid and not expired

---

**Your application is now ready to use! ??**
