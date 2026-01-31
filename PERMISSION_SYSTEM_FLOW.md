# Permission System Flow - Visual Guide

## 🔄 Request Flow Diagram

```
┌─────────────┐
│   Client    │
│  (Browser/  │
│   App)      │
└──────┬──────┘
       │
       │ 1. POST /api/auth/login
       │    { email, password }
       │
       ▼
┌─────────────────────┐
│   AuthController    │
│  - Validates user   │
│  - Generates JWT    │
└──────┬──────────────┘
       │
       │ 2. Returns JWT Token
       │    { token, email, roles }
       │
       ▼
┌─────────────┐
│   Client    │
│   Stores    │
│   Token     │
└──────┬──────┘
       │
       │ 3. GET /api/product
       │    Header: Authorization: Bearer {token}
       │
       ▼
┌─────────────────────┐
│  JWT Middleware     │
│  - Validates token  │
│  - Extracts claims  │
│  - Sets User        │
└──────┬──────────────┘
       │
       │ 4. User authenticated
       │
       ▼
┌─────────────────────┐
│ ProductController   │
│ [Authorize(Policy = │
│  "Permission:       │
│   Product.Read")]   │
└──────┬──────────────┘
       │
       │ 5. Authorization check
       │
       ▼
┌─────────────────────────────────┐
│ PermissionPolicyProvider        │
│ - Creates policy dynamically    │
│ - Policy name: "Permission:     │
│   Product.Read"                 │
└──────┬──────────────────────────┘
       │
       │ 6. Policy requirement
       │
       ▼
┌─────────────────────────────────┐
│ PermissionAuthorizationHandler  │
│ 1. Get user from token          │
│ 2. Get user's roles             │
│ 3. Query RolePermissions table  │
│    WHERE role IN (user roles)   │
│    AND permission = "Product.  │
│    Read"                        │
└──────┬───────────────────────────┘
       │
       │ 7. Check result
       │
       ├─── YES (has permission) ────┐
       │                              │
       ▼                              ▼
┌─────────────────┐         ┌─────────────────┐
│  Allow Access   │         │  Deny Access    │
│  Return 200 OK  │         │  Return 403     │
│  with data      │         │  Forbidden      │
└─────────────────┘         └─────────────────┘
```

---

## 🗂️ Database Structure

```
┌─────────────────────┐
│  AspNetUsers        │
│  - Id               │
│  - Email            │
│  - PasswordHash    │
└──────────┬──────────┘
           │
           │ (many-to-many)
           │
           ▼
┌─────────────────────┐      ┌──────────────────────┐
│  AspNetUserRoles    │      │  AspNetRoles         │
│  - UserId           │◄─────┤  - Id                │
│  - RoleId           │      │  - Name              │
└─────────────────────┘      └──────────┬───────────┘
                                         │
                                         │ (one-to-many)
                                         │
                                         ▼
                                ┌──────────────────────┐
                                │  RolePermissions     │
                                │  - Id                │
                                │  - RoleId            │
                                │  - PermissionId      │
                                │  - AssignedAt        │
                                └──────────┬───────────┘
                                           │
                                           │ (many-to-one)
                                           │
                                           ▼
                                  ┌──────────────────────┐
                                  │  Permissions         │
                                  │  - Id                │
                                  │  - Name              │
                                  │  - Resource          │
                                  │  - Action            │
                                  │  - IsActive          │
                                  └──────────────────────┘
```

---

## 🔐 Permission Check Logic

```
User Request
    │
    ├─► Extract JWT Token
    │       │
    │       ├─► Decode Token
    │       │       │
    │       │       ├─► Get UserId
    │       │       └─► Get Roles (from claims)
    │       │
    │       └─► Query Database:
    │           │
    │           ┌─────────────────────────────────────────┐
    │           │ SELECT rp.*                             │
    │           │ FROM RolePermissions rp                 │
    │           │ INNER JOIN Permissions p                │
    │           │   ON rp.PermissionId = p.Id             │
    │           │ INNER JOIN AspNetRoles r                 │
    │           │   ON rp.RoleId = r.Id                   │
    │           │ WHERE r.Name IN (@userRoles)            │
    │           │   AND p.Name = @requiredPermission     │
    │           │   AND p.IsActive = 1                   │
    │           └─────────────────────────────────────────┘
    │
    └─► Result:
            │
            ├─► Found? ──► YES ──► Allow Request (200 OK)
            │
            └─► Found? ──► NO  ──► Deny Request (403 Forbidden)
```

---

## 📋 Permission Naming Convention

```
Permission Name Format: {Resource}.{Action}

Examples:
├── Product.Read      → Read products
├── Product.Create    → Create products
├── Product.Update    → Update products
├── Product.Delete    → Delete products
│
├── Order.Read        → Read orders
├── Order.Create      → Create orders
├── Order.Update      → Update orders
└── Order.Delete      → Delete orders

Resource: The entity/resource being accessed (Product, Order, User, etc.)
Action:   The operation being performed (Read, Create, Update, Delete, Manage)
```

---

## 🎯 Role-Based Access Control (RBAC) Example

```
┌────────────────────────────────────────────────────────┐
│                    Admin Role                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │ Permissions:                                     │  │
│  │  ✓ Product.Read                                  │  │
│  │  ✓ Product.Create                                │  │
│  │  ✓ Product.Update                                │  │
│  │  ✓ Product.Delete                                │  │
│  │  ✓ Permission.Manage                             │  │
│  │  ✓ Role.Manage                                   │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│                  Manager Role                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │ Permissions:                                     │  │
│  │  ✓ Product.Read                                  │  │
│  │  ✓ Product.Create                                │  │
│  │  ✓ Product.Update                                │  │
│  │  Product.Delete                                  │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                    User Role                             │
│  ┌──────────────────────────────────────────────────┐  │
│  │ Permissions:                                      │  │
│  │  ✓ Product.Read                                   │  │
│  │  ✗ Product.Create                                 │  │
│  │  ✗ Product.Update                                 │  │
│  │  ✗ Product.Delete                                 │  │
│  │  ✓ Order.Read                                      │  │
│  │  ✓ Order.Create                                    │  │
│  │  ✗ Order.Update                                    │  │
│  │  ✗ Order.Delete                                    │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

---

## 🔄 Complete Example: User Trying to Delete a Product

```
Step 1: User sends request
   POST /api/product/123
   Header: Authorization: Bearer {token}
   
Step 2: JWT Middleware validates token
   ✓ Token valid
   ✓ User authenticated
   ✓ User ID: "user-123"
   ✓ Roles: ["Manager"]
   
Step 3: ProductController receives request
   [HttpDelete("{id}")]
   [Authorize(Policy = "Permission:Product.Delete")]
   
Step 4: PermissionPolicyProvider
   Creates policy: "Permission:Product.Delete"
   Requirement: PermissionRequirement("Product.Delete")
   
Step 5: PermissionAuthorizationHandler
   Query: Does user "user-123" with role "Manager" 
          have permission "Product.Delete"?
   
   SQL Query:
   SELECT COUNT(*) 
   FROM RolePermissions rp
   INNER JOIN Permissions p ON rp.PermissionId = p.Id
   INNER JOIN AspNetRoles r ON rp.RoleId = r.Id
   WHERE r.Name = 'Manager'
     AND p.Name = 'Product.Delete'
     AND p.IsActive = 1
   
   Result: 0 rows found
   
Step 6: Authorization Result
   ✗ Permission NOT found
   → Return 403 Forbidden
   → Request blocked
   
Step 7: Response to Client
   Status: 403 Forbidden
   Body: { "message": "Access denied" }
```

---

## ✅ Best Practices

1. **Permission Naming**: Use consistent format `Resource.Action`
2. **Role Hierarchy**: Create roles that make sense for your business
3. **Least Privilege**: Give users only the permissions they need
4. **Token Security**: 
   - Use HTTPS in production
   - Set appropriate token expiration
   - Store tokens securely on client
5. **Permission Granularity**: 
   - Too broad: "Product.Manage" (allows everything)
   - Better: Separate "Product.Read", "Product.Create", etc.
6. **Testing**: Always test with different roles and permissions

---

## 🚨 Common Scenarios

### Scenario 1: New User Registration
```
1. User registers → Account created
2. User assigned to "User" role (default)
3. "User" role has: Product.Read, Order.Read
4. User can read but not create/update/delete
```

### Scenario 2: Promoting User to Manager
```
1. Admin assigns "Manager" role to user
2. "Manager" role has: Product.Read, Create, Update
3. User now has both "User" and "Manager" permissions
4. User can read, create, and update products
```

### Scenario 3: Revoking Permission
```
1. Admin removes "Product.Delete" from "Manager" role
2. All managers immediately lose delete permission
3. Next request with delete will return 403
```

---

This visual guide should help you understand how the permission system works end-to-end!
