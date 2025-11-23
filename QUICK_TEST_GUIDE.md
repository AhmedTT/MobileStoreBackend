# ?? Quick Start - Test Your API in 3 Minutes!

## ? The Fastest Way (Using Visual Studio)

### Step 1: Start Your API (5 seconds)
Press **F5** in Visual Studio

OR run this command:
```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

You should see:
```
Now listening on: https://localhost:5001
Application started.
```

---

### Step 2: Open the Test File (5 seconds)
In Visual Studio, open:
```
src/MobileSparePartsManagement.Api/MobileSparePartsManagement.Api.http
```

---

### Step 3: Register a User (10 seconds)

**Find this section in the file:**
```http
### STEP 1: Register a New User
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Password123!"
}
```

**Click "Send Request"** (green link above the POST line)

**? You should see:**
```json
{
  "id": "some-guid-here",
  "email": "admin@example.com"
}
```

---

### Step 4: Login to Get Token (10 seconds)

**Find this section:**
```http
### STEP 2: Login to Get JWT Token
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Password123!"
}
```

**Click "Send Request"**

**? You should see:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI...",
  "user": {
    "id": "some-guid",
    "email": "admin@example.com"
  }
}
```

**?? COPY THE TOKEN** (the long string after "token":)

---

### Step 5: Set Your Token (15 seconds)

**Go to the top of the file**

Find this line:
```http
@token = 
```

**Replace it with:**
```http
@token = Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...YOUR_COPIED_TOKEN_HERE
```

?? **IMPORTANT:** Keep the word "Bearer " before your token!

---

### Step 6: Test Creating a Supplier (10 seconds)

**Find this section:**
```http
### Create a Supplier
POST {{baseUrl}}/api/suppliers
Authorization: {{token}}
Content-Type: application/json

{
  "name": "ABC Electronics",
  "contactEmail": "contact@abc.com",
  "phone": "+1234567890"
}
```

**Click "Send Request"**

**? You should see:**
```json
{
  "id": "supplier-guid-here",
  "name": "ABC Electronics",
  "contactEmail": "contact@abc.com",
  "phone": "+1234567890",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**?? COPY THE SUPPLIER ID** (you'll need it for spare parts!)

---

### Step 7: Get All Suppliers (5 seconds)

**Find this section:**
```http
### Get All Suppliers
GET {{baseUrl}}/api/suppliers
Authorization: {{token}}
```

**Click "Send Request"**

**? You should see a list:**
```json
[
  {
    "id": "supplier-guid",
    "name": "ABC Electronics",
    ...
  }
]
```

---

### Step 8: Create a Spare Part (15 seconds)

**Find this section:**
```http
### Create a Spare Part
POST {{baseUrl}}/api/spareparts
Authorization: {{token}}
Content-Type: application/json

{
  "name": "iPhone 14 Pro Screen",
  "quantity": 50,
  "price": 129.99,
  "supplierId": "PASTE_SUPPLIER_ID_HERE"
}
```

**Replace `PASTE_SUPPLIER_ID_HERE`** with the supplier ID you copied

**Click "Send Request"**

**? You should see:**
```json
{
  "id": "sparepart-guid",
  "name": "iPhone 14 Pro Screen",
  "quantity": 50,
  "price": 129.99,
  "supplierId": "supplier-guid",
  "supplierName": "ABC Electronics",
  "createdAt": "2024-01-15T10:35:00Z"
}
```

---

### Step 9: Get Spare Parts with Pagination (5 seconds)

**Find this section:**
```http
### Get All Spare Parts (with pagination)
GET {{baseUrl}}/api/spareparts?page=1&pageSize=10
Authorization: {{token}}
```

**Click "Send Request"**

**? You should see:**
```json
{
  "items": [
    {
      "id": "sparepart-guid",
      "name": "iPhone 14 Pro Screen",
      ...
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

## ?? Congratulations!

You just tested:
- ? User Registration
- ? User Login (JWT Authentication)
- ? Create Supplier
- ? List Suppliers
- ? Create Spare Part
- ? List Spare Parts with Pagination

---

## ?? What's Next?

### Test More Features:

1. **Search by Name:**
```http
GET {{baseUrl}}/api/spareparts?name=iPhone&page=1&pageSize=10
Authorization: {{token}}
```

2. **Sort by Price:**
```http
GET {{baseUrl}}/api/spareparts?sortBy=price&sortDir=desc
Authorization: {{token}}
```

3. **Filter by Supplier:**
```http
GET {{baseUrl}}/api/spareparts?supplierId=YOUR_SUPPLIER_ID
Authorization: {{token}}
```

4. **Update a Spare Part:**
```http
PUT {{baseUrl}}/api/spareparts/YOUR_SPAREPART_ID
Authorization: {{token}}
Content-Type: application/json

{
  "name": "iPhone 14 Pro Screen - Updated",
  "quantity": 75,
  "price": 119.99,
  "supplierId": "YOUR_SUPPLIER_ID"
}
```

5. **Delete a Spare Part:**
```http
DELETE {{baseUrl}}/api/spareparts/YOUR_SPAREPART_ID
Authorization: {{token}}
```

---

## ?? Pro Tips

### Tip 1: Token Expires After 60 Minutes
If you get `401 Unauthorized`, just login again (Step 4) and update your token (Step 5).

### Tip 2: Save IDs
Keep track of supplier IDs and spare part IDs in a notepad for quick testing.

### Tip 3: Use Response History
Visual Studio saves your response history - you can review previous responses.

### Tip 4: Test Validation
Try creating a spare part with negative price to see validation errors:
```json
{
  "name": "Test",
  "quantity": 10,
  "price": -50,  // This will fail!
  "supplierId": "your-id"
}
```

---

## ?? Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| "Send Request" not showing | Make sure you're using Visual Studio 2022+ |
| 401 Unauthorized | Check token is set correctly with "Bearer " prefix |
| Connection refused | Make sure API is running (F5 or dotnet run) |
| Invalid supplier ID | Use the actual GUID from create supplier response |

---

## ?? Alternative: Use Postman

If you prefer a GUI tool:

1. Download Postman: https://www.postman.com/downloads/
2. Create new request
3. Import collection from: `https://localhost:5001/openapi/v1.json`
4. Follow same steps as above

---

## ? Summary

**Total Time: ~3 minutes**

1. ?? 5 sec - Start API
2. ?? 5 sec - Open HTTP file
3. ?? 10 sec - Register user
4. ?? 10 sec - Login
5. ?? 15 sec - Copy and set token
6. ?? 10 sec - Create supplier
7. ?? 5 sec - List suppliers
8. ?? 15 sec - Create spare part
9. ?? 5 sec - List spare parts

**Your API is fully functional! ??**

---

**Need more help? Check `API_TESTING_GUIDE.md` for detailed documentation!**
