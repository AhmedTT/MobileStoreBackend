# ?? Postman Testing Guide - Mobile Spare Parts API

## ?? Quick Start with Postman

---

## ?? Step 1: Install Postman

### Download Postman:
**Link:** https://www.postman.com/downloads/

### Choose Your Version:
- **Windows:** Download and install the .exe
- **Mac:** Download and install the .dmg
- **Linux:** Download and install the AppImage

**Alternative:** Use Postman Web (no installation needed)
- Go to: https://web.postman.co/

---

## ?? Step 2: Import Your API

### Option A: Import OpenAPI Specification (Easiest)

1. **Start your API first:**
   ```bash
   dotnet run --project src/MobileSparePartsManagement.Api
   ```
   Wait for: "Now listening on: https://localhost:5001"

2. **Open Postman**

3. **Click "Import"** (top left)

4. **Choose "Link"** tab

5. **Paste this URL:**
   ```
   https://localhost:5001/openapi/v1.json
   ```

6. **Click "Continue"** ? **"Import"**

7. **Done!** All your endpoints are now in Postman! ?

### Option B: Create Requests Manually (If import doesn't work)

See the manual setup section below.

---

## ?? Step 3: Set Up Authentication

### Create Environment Variables (Makes Testing Easier)

1. **Click "Environments"** (left sidebar, looks like an eye ???)

2. **Click "+"** to create new environment

3. **Name it:** `Mobile Spare Parts - Local`

4. **Add these variables:**

   | Variable | Initial Value | Current Value |
   |----------|---------------|---------------|
   | `baseUrl` | `https://localhost:5001` | `https://localhost:5001` |
   | `token` | (leave empty) | (leave empty) |

5. **Click "Save"**

6. **Select the environment** from the dropdown (top right)

---

## ?? Step 4: Test Authentication

### 4.1 Register a User

1. **Create New Request:**
   - Click "+" or "New" ? "HTTP Request"
   - Name it: `Register User`

2. **Configure Request:**
   - **Method:** `POST`
   - **URL:** `{{baseUrl}}/api/auth/register`
   - **Headers:** 
     - Key: `Content-Type`
     - Value: `application/json`
   - **Body:** Select `raw` ? `JSON`
     ```json
     {
       "email": "admin@example.com",
       "password": "Password123!"
     }
     ```

3. **Click "Send"**

4. **Expected Response:** `201 Created`
   ```json
   {
     "id": "some-guid-here",
     "email": "admin@example.com"
   }
   ```

### 4.2 Login to Get Token

1. **Create New Request:**
   - Name it: `Login`

2. **Configure Request:**
   - **Method:** `POST`
   - **URL:** `{{baseUrl}}/api/auth/login`
   - **Headers:**
     - Key: `Content-Type`
     - Value: `application/json`
   - **Body:** `raw` ? `JSON`
     ```json
     {
       "email": "admin@example.com",
       "password": "Password123!"
     }
     ```

3. **Click "Send"**

4. **Expected Response:** `200 OK`
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "user": {
       "id": "guid-here",
       "email": "admin@example.com"
     }
   }
   ```

5. **IMPORTANT: Save the Token Automatically**

   Add this script to the "Tests" tab (below the request body):
   ```javascript
   // Save token to environment variable
   if (pm.response.code === 200) {
       var jsonData = pm.response.json();
       pm.environment.set("token", "Bearer " + jsonData.token);
       console.log("Token saved!");
   }
   ```

6. **Click "Send" again**

7. **Check:** Go to environment variables - `token` should now have a value! ?

---

## ?? Step 5: Test Suppliers Endpoints

### 5.1 Get All Suppliers

1. **Create New Request:**
   - Name it: `Get All Suppliers`

2. **Configure Request:**
   - **Method:** `GET`
   - **URL:** `{{baseUrl}}/api/suppliers`
   - **Headers:** (This is automatic with Auth - see below)

3. **Add Authentication:**
   - Go to "Authorization" tab
   - **Type:** `Bearer Token`
   - **Token:** `{{token}}`
   
   *Note: The double curly braces tell Postman to use the environment variable*

4. **Click "Send"**

5. **Expected Response:** `200 OK`
   ```json
   [
     // Empty array if no suppliers yet
   ]
   ```

### 5.2 Create a Supplier

1. **Create New Request:**
   - Name it: `Create Supplier`

2. **Configure Request:**
   - **Method:** `POST`
   - **URL:** `{{baseUrl}}/api/suppliers`
   - **Authorization:** `Bearer Token` ? `{{token}}`
   - **Headers:**
     - Key: `Content-Type`
     - Value: `application/json`
   - **Body:** `raw` ? `JSON`
     ```json
     {
       "name": "ABC Electronics",
       "contactEmail": "contact@abc.com",
       "phone": "+1234567890"
     }
     ```

3. **Click "Send"**

4. **Expected Response:** `201 Created`
   ```json
   {
     "id": "supplier-guid-here",
     "name": "ABC Electronics",
     "contactEmail": "contact@abc.com",
     "phone": "+1234567890",
     "createdAt": "2024-01-15T10:30:00Z"
   }
   ```

5. **IMPORTANT: Save Supplier ID**

   Add this to "Tests" tab:
   ```javascript
   if (pm.response.code === 201) {
       var jsonData = pm.response.json();
       pm.environment.set("supplierId", jsonData.id);
       console.log("Supplier ID saved: " + jsonData.id);
   }
   ```

### 5.3 Get Single Supplier

1. **Create New Request:**
   - Name it: `Get Supplier by ID`
   - **Method:** `GET`
   - **URL:** `{{baseUrl}}/api/suppliers/{{supplierId}}`
   - **Authorization:** `Bearer Token` ? `{{token}}`

2. **Click "Send"**

3. **Expected Response:** `200 OK` with supplier details

### 5.4 Update Supplier

1. **Create New Request:**
   - Name it: `Update Supplier`
   - **Method:** `PUT`
   - **URL:** `{{baseUrl}}/api/suppliers/{{supplierId}}`
   - **Authorization:** `Bearer Token` ? `{{token}}`
   - **Body:** `raw` ? `JSON`
     ```json
     {
       "name": "ABC Electronics - Updated",
       "contactEmail": "newemail@abc.com",
       "phone": "+9876543210"
     }
     ```

2. **Click "Send"**

3. **Expected Response:** `200 OK` with updated supplier

### 5.5 Delete Supplier

1. **Create New Request:**
   - Name it: `Delete Supplier`
   - **Method:** `DELETE`
   - **URL:** `{{baseUrl}}/api/suppliers/{{supplierId}}`
   - **Authorization:** `Bearer Token` ? `{{token}}`

2. **Click "Send"**

3. **Expected Response:** `204 No Content`

---

## ?? Step 6: Test Spare Parts Endpoints

### 6.1 Get All Spare Parts (with Pagination)

1. **Create New Request:**
   - Name it: `Get All Spare Parts`
   - **Method:** `GET`
   - **URL:** `{{baseUrl}}/api/spareparts?page=1&pageSize=10`
   - **Authorization:** `Bearer Token` ? `{{token}}`

2. **Click "Send"**

3. **Expected Response:** `200 OK`
   ```json
   {
     "items": [],
     "totalCount": 0,
     "page": 1,
     "pageSize": 10,
     "totalPages": 0
   }
   ```

### 6.2 Create a Spare Part

1. **First, create a supplier** (if you deleted it in step 5.5)

2. **Create New Request:**
   - Name it: `Create Spare Part`
   - **Method:** `POST`
   - **URL:** `{{baseUrl}}/api/spareparts`
   - **Authorization:** `Bearer Token` ? `{{token}}`
   - **Body:** `raw` ? `JSON`
     ```json
     {
       "name": "iPhone 14 Pro Screen",
       "quantity": 50,
       "price": 129.99,
       "supplierId": "{{supplierId}}"
     }
     ```

3. **Click "Send"**

4. **Expected Response:** `201 Created`
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

5. **Save Spare Part ID:**

   Tests tab:
   ```javascript
   if (pm.response.code === 201) {
       var jsonData = pm.response.json();
       pm.environment.set("sparePartId", jsonData.id);
       console.log("Spare Part ID saved: " + jsonData.id);
   }
   ```

### 6.3 Get Spare Parts with Filters

**Filter by Name:**
```
{{baseUrl}}/api/spareparts?name=iPhone&page=1&pageSize=10
```

**Filter by Supplier:**
```
{{baseUrl}}/api/spareparts?supplierId={{supplierId}}&page=1&pageSize=10
```

**Sort by Price (Descending):**
```
{{baseUrl}}/api/spareparts?sortBy=price&sortDir=desc&page=1&pageSize=10
```

**Combined Filters:**
```
{{baseUrl}}/api/spareparts?name=iPhone&supplierId={{supplierId}}&sortBy=price&sortDir=asc&page=1&pageSize=10
```

### 6.4 Update Spare Part

1. **Create New Request:**
   - Name it: `Update Spare Part`
   - **Method:** `PUT`
   - **URL:** `{{baseUrl}}/api/spareparts/{{sparePartId}}`
   - **Authorization:** `Bearer Token` ? `{{token}}`
   - **Body:** `raw` ? `JSON`
     ```json
     {
       "name": "iPhone 14 Pro Screen - Updated",
       "quantity": 75,
       "price": 119.99,
       "supplierId": "{{supplierId}}"
     }
     ```

2. **Click "Send"**

### 6.5 Delete Spare Part

1. **Create New Request:**
   - Name it: `Delete Spare Part`
   - **Method:** `DELETE`
   - **URL:** `{{baseUrl}}/api/spareparts/{{sparePartId}}`
   - **Authorization:** `Bearer Token` ? `{{token}}`

2. **Click "Send"**

3. **Expected Response:** `204 No Content`

---

## ??? Step 7: Organize Your Requests

### Create a Collection

1. **Click "Collections"** (left sidebar)
2. **Click "+"** ? "Blank Collection"
3. **Name it:** `Mobile Spare Parts API`

### Create Folders

1. **Right-click collection** ? "Add folder"
2. Create these folders:
   - `Authentication`
   - `Suppliers`
   - `Spare Parts`

3. **Drag requests into folders:**
   - Register, Login ? Authentication
   - Supplier requests ? Suppliers
   - Spare part requests ? Spare Parts

---

## ?? Step 8: Create a Test Workflow

### Use Collection Runner

1. **Click your collection**
2. **Click "Run"** (top right)
3. **Select requests to run** in order:
   - Register User
   - Login
   - Create Supplier
   - Create Spare Part
   - Get All Spare Parts
   - Update Spare Part
   - Delete Spare Part
   - Delete Supplier

4. **Click "Run Mobile Spare Parts API"**

5. **Watch all tests run automatically!** ?

---

## ?? Pro Tips for Postman

### 1. Use Pre-request Scripts

Add this to Collection level (runs before every request):
```javascript
// Log what we're about to call
console.log("Calling: " + pm.request.url);
console.log("Method: " + pm.request.method);
```

### 2. Use Tests for Validation

Add to each request's "Tests" tab:
```javascript
// Check status code
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Check response time
pm.test("Response time is less than 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

// Check response has required fields
pm.test("Response has id field", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('id');
});
```

### 3. Share Collection with Team

1. Click collection ? "Share"
2. Export as JSON
3. Share file with team
4. They import: "Import" ? "File" ? Select JSON

---

## ?? Troubleshooting in Postman

### SSL Certificate Errors

If you get SSL errors:

1. **Go to Settings:** File ? Settings (or Postman ? Preferences on Mac)
2. **Turn off SSL verification:**
   - Find "SSL certificate verification"
   - Toggle it **OFF**
3. **Try request again**

### Token Expired (401 Error)

1. Go back to "Login" request
2. Click "Send" again
3. Token will be automatically saved
4. Retry your request

### Connection Refused

1. Check API is running: `dotnet run --project src/MobileSparePartsManagement.Api`
2. Verify URL: `https://localhost:5001`
3. Check firewall settings

---

## ?? Complete Postman Request Collection

Here's a summary of all requests you should have:

### Authentication
- POST `{{baseUrl}}/api/auth/register`
- POST `{{baseUrl}}/api/auth/login`

### Suppliers
- GET `{{baseUrl}}/api/suppliers`
- GET `{{baseUrl}}/api/suppliers/{{supplierId}}`
- POST `{{baseUrl}}/api/suppliers`
- PUT `{{baseUrl}}/api/suppliers/{{supplierId}}`
- DELETE `{{baseUrl}}/api/suppliers/{{supplierId}}`

### Spare Parts
- GET `{{baseUrl}}/api/spareparts?page=1&pageSize=10`
- GET `{{baseUrl}}/api/spareparts/{{sparePartId}}`
- POST `{{baseUrl}}/api/spareparts`
- PUT `{{baseUrl}}/api/spareparts/{{sparePartId}}`
- DELETE `{{baseUrl}}/api/spareparts/{{sparePartId}}`

---

## ?? You're Ready!

**Your Postman setup is complete!** ??

### Quick Testing Workflow:

1. Start API (F5)
2. Open Postman
3. Run "Login" request
4. Create/test any endpoint
5. All tokens/IDs saved automatically via environment variables!

---

## ?? Quick Import (Alternative Method)

Instead of manual setup, you can import this collection:

1. Save this as `mobile-spare-parts.postman_collection.json`
2. Import in Postman
3. Set up environment variables
4. Start testing!

---

**Happy Testing with Postman! ???**
