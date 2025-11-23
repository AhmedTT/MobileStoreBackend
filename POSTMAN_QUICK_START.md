# ?? Postman Quick Start Card

## ? Test Your API in Postman (5 Minutes)

---

## ?? Step 1: Install Postman (2 minutes)
**Download:** https://www.postman.com/downloads/

---

## ?? Step 2: Import Your API (30 seconds)

### Easy Auto-Import:

1. **Start your API:**
   ```bash
   dotnet run --project src/MobileSparePartsManagement.Api
   ```

2. **In Postman, click "Import"**

3. **Paste this URL:**
   ```
   https://localhost:5001/openapi/v1.json
   ```

4. **Click "Continue" ? "Import"**

? **Done! All endpoints imported automatically!**

---

## ?? Step 3: Set Up Environment (1 minute)

1. **Click "Environments"** (left sidebar, eye icon ???)

2. **Click "+" ? Name it:** `Local`

3. **Add variables:**
   ```
   baseUrl = https://localhost:5001
   token = (leave empty)
   ```

4. **Save** and **select environment** from dropdown (top right)

---

## ?? Step 4: Login & Get Token (1 minute)

### Find "Login" Request
(If imported, it's in your collection)

**OR Create it manually:**

**Method:** `POST`  
**URL:** `{{baseUrl}}/api/auth/login`  
**Body (JSON):**
```json
{
  "email": "admin@example.com",
  "password": "Password123!"
}
```

### Auto-Save Token

**Add this to "Tests" tab:**
```javascript
if (pm.response.code === 200) {
    var data = pm.response.json();
    pm.environment.set("token", "Bearer " + data.token);
}
```

**Click "Send"** ? Token saved automatically! ?

---

## ?? Step 5: Test Protected Endpoints (30 seconds)

### For ANY protected request:

**Authorization tab:**
- Type: `Bearer Token`
- Token: `{{token}}`

**Example - Get Suppliers:**

**Method:** `GET`  
**URL:** `{{baseUrl}}/api/suppliers`  
**Authorization:** Bearer Token ? `{{token}}`

**Click "Send"** ?

---

## ?? Complete Test Flow

### 1. Register (if needed)
```
POST {{baseUrl}}/api/auth/register
Body: {"email":"admin@example.com","password":"Password123!"}
```

### 2. Login
```
POST {{baseUrl}}/api/auth/login
Body: {"email":"admin@example.com","password":"Password123!"}
```
*Token auto-saved via Tests script*

### 3. Create Supplier
```
POST {{baseUrl}}/api/suppliers
Auth: Bearer {{token}}
Body: {"name":"ABC Electronics","contactEmail":"test@abc.com"}
```

### 4. Get Suppliers
```
GET {{baseUrl}}/api/suppliers
Auth: Bearer {{token}}
```

### 5. Create Spare Part
```
POST {{baseUrl}}/api/spareparts
Auth: Bearer {{token}}
Body: {
  "name":"iPhone Screen",
  "quantity":50,
  "price":99.99,
  "supplierId":"PASTE_ID_FROM_STEP_3"
}
```

### 6. Get Spare Parts (Paginated)
```
GET {{baseUrl}}/api/spareparts?page=1&pageSize=10
Auth: Bearer {{token}}
```

---

## ?? Pro Tips

### Save IDs Automatically

**Add to Tests tab of Create requests:**

**For Supplier:**
```javascript
if (pm.response.code === 201) {
    pm.environment.set("supplierId", pm.response.json().id);
}
```

**For Spare Part:**
```javascript
if (pm.response.code === 201) {
    pm.environment.set("sparePartId", pm.response.json().id);
}
```

### Then Use IDs in URLs:
```
GET {{baseUrl}}/api/suppliers/{{supplierId}}
DELETE {{baseUrl}}/api/spareparts/{{sparePartId}}
```

---

## ?? Troubleshooting

### SSL Error?
**Settings** ? Turn off "SSL certificate verification"

### 401 Unauthorized?
- Run "Login" request again
- Check token starts with "Bearer "

### Connection Refused?
- Make sure API is running (F5 or `dotnet run`)
- Check URL: `https://localhost:5001`

---

## ?? Environment Variables Cheat Sheet

| Variable | Value | Auto-set by |
|----------|-------|-------------|
| `baseUrl` | `https://localhost:5001` | Manual |
| `token` | `Bearer eyJ...` | Login request |
| `supplierId` | `guid-here` | Create Supplier request |
| `sparePartId` | `guid-here` | Create Spare Part request |

---

## ??? Collection Organization

```
?? Mobile Spare Parts API
  ?? Authentication
    ?? POST Register
    ?? POST Login
  ?? Suppliers
    ?? GET All Suppliers
    ?? POST Create Supplier
    ?? GET Supplier by ID
    ?? PUT Update Supplier
    ?? DELETE Supplier
  ?? Spare Parts
    ?? GET All Spare Parts
    ?? POST Create Spare Part
    ?? GET Spare Part by ID
    ?? PUT Update Spare Part
    ?? DELETE Spare Part
```

---

## ? Verification Checklist

- [ ] Postman installed
- [ ] API running (port 5001)
- [ ] Collection imported OR requests created
- [ ] Environment created with `baseUrl` and `token`
- [ ] Environment selected (top right dropdown)
- [ ] Login request sends successfully
- [ ] Token auto-saved to environment
- [ ] Protected endpoints work with `{{token}}`
- [ ] SSL verification turned off (if needed)

---

## ?? You're Ready!

**Test your entire API in minutes with Postman!** ??

Need more details? Check **POSTMAN_GUIDE.md** for complete documentation.

---

## ?? Quick Commands Reference

### Start API:
```bash
dotnet run --project src/MobileSparePartsManagement.Api
```

### Import URL:
```
https://localhost:5001/openapi/v1.json
```

### Test Script (Login):
```javascript
if (pm.response.code === 200) {
    pm.environment.set("token", "Bearer " + pm.response.json().token);
}
```

---

**Happy Testing! ??**
