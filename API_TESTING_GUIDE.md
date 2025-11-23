# ?? API Testing Guide - All Methods

## ?? You Have 5 Easy Ways to Test Your API!

---

## ? METHOD 1: Visual Studio HTTP File (EASIEST - RECOMMENDED)

### How to Use:

1. **Open the file**: `src/MobileSparePartsManagement.Api/MobileSparePartsManagement.Api.http`

2. **Make sure your API is running** (F5 or `dotnet run`)

3. **Click "Send Request"** above each endpoint

   You'll see green "Send Request" links above each `###` section in Visual Studio.

### Step-by-Step Workflow:

#### Step 1: Register a User
```
Click "Send Request" above the "Register a New User" section
```
You should see: `201 Created` response

#### Step 2: Login
```
Click "Send Request" above the "Login to Get JWT Token" section
```
**Copy the token from the response!**

#### Step 3: Update Token Variable
At the top of the file, update:
```
@token = Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
Paste your actual token after `Bearer `

#### Step 4: Test Protected Endpoints
Now click "Send Request" on any protected endpoint (Suppliers, Spare Parts)

### Visual Guide:
```
???????????????????????????????????????
? ### Register User                   ?
? Send Request ? Click here          ?
? POST https://localhost:5001/...    ?
? {                                   ?
?   "email": "admin@example.com"     ?
? }                                   ?
???????????????????????????????????????
```

---

## ? METHOD 2: Postman (Popular Tool)

### Setup:
1. Download Postman: https://www.postman.com/downloads/
2. Import OpenAPI spec: `https://localhost:5001/openapi/v1.json`

### Quick Start:

#### 1. Register User
```
Method: POST
URL: https://localhost:5001/api/auth/register
Headers: Content-Type: application/json
Body (raw JSON):
{
  "email": "admin@example.com",
  "password": "Password123!"
}
```

#### 2. Login
```
Method: POST
URL: https://localhost:5001/api/auth/login
Headers: Content-Type: application/json
Body (raw JSON):
{
  "email": "admin@example.com",
  "password": "Password123!"
}
```
**Copy the token from response**

#### 3. Use Token for Protected Endpoints
```
Method: GET
URL: https://localhost:5001/api/suppliers
Headers: 
  - Authorization: Bearer YOUR_TOKEN_HERE
```

---

## ? METHOD 3: PowerShell (Built into Windows)

### Register User:
```powershell
$body = @{
    email = "admin@example.com"
    password = "Password123!"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/auth/register" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck
```

### Login:
```powershell
$body = @{
    email = "admin@example.com"
    password = "Password123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:5001/api/auth/login" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck

$token = $response.token
Write-Host "Token: $token"
```

### Get Suppliers (with token):
```powershell
$headers = @{
    Authorization = "Bearer $token"
}

Invoke-RestMethod -Uri "https://localhost:5001/api/suppliers" `
    -Method Get `
    -Headers $headers `
    -SkipCertificateCheck
```

---

## ? METHOD 4: cURL (Command Line)

### Register User:
```bash
curl -X POST "https://localhost:5001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@example.com\",\"password\":\"Password123!\"}" \
  -k
```

### Login:
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@example.com\",\"password\":\"Password123!\"}" \
  -k
```

### Get Suppliers (replace TOKEN):
```bash
curl -X GET "https://localhost:5001/api/suppliers" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -k
```

---

## ? METHOD 5: Thunder Client (VS Code Extension)

### Install:
1. Open VS Code
2. Go to Extensions (Ctrl+Shift+X)
3. Search "Thunder Client"
4. Install it

### Use:
1. Click Thunder Client icon in sidebar
2. Click "New Request"
3. Enter URL and method
4. Add headers and body as needed

---

## ?? COMPLETE TESTING WORKFLOW (Using HTTP File)

### 1. Start Your API
```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

### 2. Open HTTP File
Open: `src/MobileSparePartsManagement.Api/MobileSparePartsManagement.Api.http`

### 3. Register & Login (Follow These Steps)

#### ? Register
Click "Send Request" above:
```http
POST {{baseUrl}}/api/auth/register
```
? Expected: `201 Created`

#### ? Login
Click "Send Request" above:
```http
POST {{baseUrl}}/api/auth/login
```
? Expected: `200 OK` with token

#### ? Copy Token
From response, copy the `token` value (the long string)

#### ? Update Token Variable
At top of file, replace:
```http
@token = Bearer YOUR_COPIED_TOKEN_HERE
```

### 4. Test Suppliers

#### Create Supplier
Click "Send Request" above:
```http
POST {{baseUrl}}/api/suppliers
```
? Expected: `201 Created`
?? **Copy the `id` from response!**

#### Get All Suppliers
Click "Send Request" above:
```http
GET {{baseUrl}}/api/suppliers
```
? Expected: `200 OK` with list of suppliers

### 5. Test Spare Parts

#### Create Spare Part
Replace `PASTE_SUPPLIER_ID_HERE` with the supplier ID you copied, then click "Send Request":
```http
POST {{baseUrl}}/api/spareparts
```
? Expected: `201 Created`

#### Get Spare Parts with Pagination
Click "Send Request" above:
```http
GET {{baseUrl}}/api/spareparts?page=1&pageSize=10
```
? Expected: `200 OK` with paginated results

---

## ?? Understanding Responses

### Success Responses:
- `200 OK` - Request succeeded
- `201 Created` - Resource created successfully
- `204 No Content` - Deletion succeeded

### Error Responses:
- `400 Bad Request` - Validation error (check your input)
- `401 Unauthorized` - Missing or invalid token
- `404 Not Found` - Resource doesn't exist

---

## ?? Tips for Testing

### 1. Keep Track of IDs
When you create a supplier or spare part, save the `id` from the response to use in update/delete operations.

### 2. Token Expiry
JWT tokens expire after 60 minutes. If you get `401 Unauthorized`, login again to get a fresh token.

### 3. Use Variables in HTTP File
The HTTP file uses variables like `{{baseUrl}}` and `{{token}}` - this makes testing easier!

### 4. Test Validation
Try sending invalid data (negative prices, invalid emails) to see validation in action.

### 5. Test Pagination
Try different page sizes and page numbers:
```
GET /api/spareparts?page=1&pageSize=5
GET /api/spareparts?page=2&pageSize=5
```

---

## ?? Example Test Sequence

Here's a complete test flow you can follow:

```
1. Register user ?
2. Login ?
3. Copy token ?
4. Update @token variable ?
5. Create supplier "ABC Electronics" ?
6. Copy supplier ID ?
7. Create spare part "iPhone Screen" (use supplier ID) ?
8. Get all spare parts (should see 1 item) ?
9. Update spare part (change price) ?
10. Get spare parts again (verify price changed) ?
11. Delete spare part ?
12. Delete supplier ?
```

---

## ?? Visual Studio Features

When using the HTTP file in Visual Studio:

### Response Window Shows:
- ? Status Code (200, 201, etc.)
- ? Response Headers
- ? Response Body (JSON)
- ? Response Time

### Features:
- ? Syntax highlighting
- ? One-click execution
- ? Variables support
- ? Response history
- ? Copy as cURL

---

## ?? Troubleshooting

### "Send Request" link not visible?
- Make sure you have Visual Studio 2022 or later
- Or use VS Code with "REST Client" extension

### 401 Unauthorized?
- Check if token is set correctly: `@token = Bearer YOUR_TOKEN`
- Make sure you include "Bearer " before the token
- Token might be expired (login again)

### Connection refused?
- Make sure API is running (`dotnet run`)
- Check the port in `@baseUrl` matches your running app

### SSL Certificate errors?
- Use `-k` flag in cURL
- Use `-SkipCertificateCheck` in PowerShell
- This is normal for local development

---

## ?? You're Ready to Test!

The **EASIEST** way is to:
1. Open `MobileSparePartsManagement.Api.http`
2. Click "Send Request" above each section
3. Copy token from login response
4. Update `@token` variable
5. Test all endpoints!

**No additional tools needed - everything is already set up! ??**
