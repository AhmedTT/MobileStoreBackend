# ? Testing Checklist - Mobile Spare Parts API

## ?? Before You Start

- [ ] API is running (`dotnet run` or F5)
- [ ] You see "Now listening on: https://localhost:5001"
- [ ] File `MobileSparePartsManagement.Api.http` is open

---

## ?? Authentication Tests

### Register User
- [ ] Click "Send Request" on `/api/auth/register`
- [ ] Response: `201 Created`
- [ ] Response contains: `id` and `email`

### Login
- [ ] Click "Send Request" on `/api/auth/login`
- [ ] Response: `200 OK`
- [ ] Response contains: `token` and `user` object
- [ ] Token copied to clipboard

### Set Token
- [ ] Token pasted at top: `@token = Bearer YOUR_TOKEN`
- [ ] Token includes "Bearer " prefix

---

## ?? Supplier Tests

### Create Supplier
- [ ] Click "Send Request" on `POST /api/suppliers`
- [ ] Response: `201 Created`
- [ ] Supplier ID copied from response
- [ ] Supplier details are correct

### Get All Suppliers
- [ ] Click "Send Request" on `GET /api/suppliers`
- [ ] Response: `200 OK`
- [ ] Response is array with your supplier

### Filter Suppliers by Name
- [ ] Click "Send Request" on `GET /api/suppliers?name=ABC`
- [ ] Response: `200 OK`
- [ ] Only matching suppliers returned

### Get Single Supplier
- [ ] Replace `{id}` with actual supplier ID
- [ ] Click "Send Request" on `GET /api/suppliers/{id}`
- [ ] Response: `200 OK`
- [ ] Correct supplier returned

### Update Supplier
- [ ] Replace `{id}` with actual supplier ID
- [ ] Modify name/email/phone in request body
- [ ] Click "Send Request" on `PUT /api/suppliers/{id}`
- [ ] Response: `200 OK`
- [ ] Changes reflected in response

### Get Suppliers Again (Verify Update)
- [ ] Click "Send Request" on `GET /api/suppliers`
- [ ] Updated supplier details visible

---

## ?? Spare Parts Tests

### Create Spare Part
- [ ] Supplier ID pasted in `supplierId` field
- [ ] Click "Send Request" on `POST /api/spareparts`
- [ ] Response: `201 Created`
- [ ] Spare part ID copied from response
- [ ] `supplierName` matches your supplier

### Get All Spare Parts (Basic)
- [ ] Click "Send Request" on `GET /api/spareparts`
- [ ] Response: `200 OK`
- [ ] Response contains `items`, `totalCount`, `page`, `pageSize`, `totalPages`
- [ ] Your spare part is in `items` array

### Get Spare Parts with Pagination
- [ ] Click "Send Request" on `GET /api/spareparts?page=1&pageSize=10`
- [ ] Response: `200 OK`
- [ ] Pagination fields are correct
- [ ] Items array size ? pageSize

### Filter by Name
- [ ] Click "Send Request" on `GET /api/spareparts?name=iPhone`
- [ ] Response: `200 OK`
- [ ] Only matching items returned

### Filter by Supplier
- [ ] Replace supplierId in URL
- [ ] Click "Send Request" on `GET /api/spareparts?supplierId=...`
- [ ] Response: `200 OK`
- [ ] Only parts from that supplier returned

### Sort by Price (Ascending)
- [ ] Click "Send Request" on `GET /api/spareparts?sortBy=price&sortDir=asc`
- [ ] Response: `200 OK`
- [ ] Items sorted by price (low to high)

### Sort by Price (Descending)
- [ ] Click "Send Request" on `GET /api/spareparts?sortBy=price&sortDir=desc`
- [ ] Response: `200 OK`
- [ ] Items sorted by price (high to low)

### Get Single Spare Part
- [ ] Replace `{id}` with actual spare part ID
- [ ] Click "Send Request" on `GET /api/spareparts/{id}`
- [ ] Response: `200 OK`
- [ ] Correct spare part returned

### Update Spare Part
- [ ] Replace `{id}` with actual spare part ID
- [ ] Modify name/quantity/price in request body
- [ ] Click "Send Request" on `PUT /api/spareparts/{id}`
- [ ] Response: `200 OK`
- [ ] Changes reflected in response

### Get Spare Part Again (Verify Update)
- [ ] Click "Send Request" on `GET /api/spareparts/{id}`
- [ ] Updated details visible

### Delete Spare Part
- [ ] Replace `{id}` with actual spare part ID
- [ ] Click "Send Request" on `DELETE /api/spareparts/{id}`
- [ ] Response: `204 No Content`

### Verify Deletion
- [ ] Click "Send Request" on `GET /api/spareparts/{id}` (same ID)
- [ ] Response: `404 Not Found`

---

## ?? Error Handling Tests

### Invalid Email Format
- [ ] Send register request with email: "invalid-email"
- [ ] Response: `400 Bad Request`
- [ ] Error message mentions email format

### Short Password
- [ ] Send register request with password: "123"
- [ ] Response: `400 Bad Request`
- [ ] Error message mentions password length

### Negative Price
- [ ] Send create spare part with `price: -50`
- [ ] Response: `400 Bad Request`
- [ ] Error message mentions price validation

### Negative Quantity
- [ ] Send create spare part with `quantity: -10`
- [ ] Response: `400 Bad Request`
- [ ] Error message mentions quantity validation

### Invalid Supplier ID
- [ ] Send create spare part with random GUID in supplierId
- [ ] Response: `400 Bad Request`
- [ ] Error message: "Supplier not found"

### Unauthorized Access
- [ ] Remove or comment out `Authorization: {{token}}` header
- [ ] Try to access `GET /api/suppliers`
- [ ] Response: `401 Unauthorized`

### Wrong Token
- [ ] Set `@token = Bearer wrong-token-here`
- [ ] Try to access `GET /api/suppliers`
- [ ] Response: `401 Unauthorized`

### Expired Token
- [ ] Wait 61 minutes (or modify token expiry to 1 minute for testing)
- [ ] Try to access protected endpoint
- [ ] Response: `401 Unauthorized`

### Non-Existent Resource
- [ ] Try to get supplier with random GUID
- [ ] Response: `404 Not Found`

### Delete Supplier with Spare Parts
- [ ] Create supplier
- [ ] Create spare part for that supplier
- [ ] Try to delete supplier
- [ ] Response: `400 Bad Request`
- [ ] Error message: "Cannot delete supplier with existing spare parts"

---

## ?? Advanced Tests

### Pagination Edge Cases
- [ ] Try `page=0` (should use page 1)
- [ ] Try `pageSize=0` (should use default 10)
- [ ] Try `page=999` (empty results)
- [ ] Try `pageSize=100` (large page size)

### Sorting Edge Cases
- [ ] Try invalid sortBy value
- [ ] Try sorting by name (A-Z)
- [ ] Try sorting by quantity

### Multiple Filters Combined
- [ ] Filter by name AND supplierId
- [ ] Filter + Sort + Pagination all together

### Create Multiple Items
- [ ] Create 3 suppliers
- [ ] Create 5 spare parts across different suppliers
- [ ] Test filtering and pagination with multiple items

### Update All Fields
- [ ] Update supplier: name, email, phone
- [ ] Update spare part: name, quantity, price, supplierId

---

## ?? Performance Tests (Optional)

### Response Time
- [ ] Check response time in Visual Studio output
- [ ] Most requests should be < 200ms

### Large Dataset
- [ ] Create 50+ spare parts
- [ ] Test pagination performance
- [ ] Test filtering performance

---

## ?? Security Tests

### SQL Injection (Should Fail)
- [ ] Try supplier name: `' OR '1'='1`
- [ ] Should be treated as literal string (EF Core prevents injection)

### XSS (Should Fail)
- [ ] Try spare part name: `<script>alert('xss')</script>`
- [ ] Should be stored as-is (not executed)

### Token Manipulation
- [ ] Modify token slightly
- [ ] Should get 401 Unauthorized

---

## ? Final Verification

### All Endpoints Working
- [ ] All POST requests return 201
- [ ] All GET requests return 200
- [ ] All PUT requests return 200
- [ ] All DELETE requests return 204
- [ ] All validation errors return 400
- [ ] All auth errors return 401
- [ ] All not found errors return 404

### Data Integrity
- [ ] Created data matches request
- [ ] Updated data persists
- [ ] Deleted data no longer accessible
- [ ] Foreign keys work correctly (supplier ? spare parts)

### CORS (if testing from Angular)
- [ ] Requests from http://localhost:4200 work
- [ ] Preflight OPTIONS requests succeed

---

## ?? Notes Section

Use this space to note:
- Supplier IDs created:
- Spare Part IDs created:
- Test tokens:
- Issues found:

---

## ?? Completion Status

**Tests Passed:** _____ / _____

**Issues Found:** _____

**Ready for Production:** ? Yes ? No ? Needs Work

---

**Good job on thorough testing! ??**
