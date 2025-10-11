# Attend - Event Registration System
## Project Development Plan

### Project Information
- **Project Location**: `C:\Users\Alisettar\source\repos\Attend\`
- **Version Control**: Git
- **Current Status**: Phase 5 Completed ✅
- **Database**: SQLite Multi-Tenant (Database per Tenant)

---

## ✅ Completed Features

### Phase 1: Core Foundation - COMPLETED
- ✅ Clean Architecture structure (Domain, Application, Infrastructure, API, Web)
- ✅ Domain entities with factory methods
- ✅ EF Core with SQLite + Configurations
- ✅ Repository Pattern implementation
- ✅ Database migrations
- ✅ Design-time DbContext factory

### Phase 2: Core API - COMPLETED
- ✅ Carter Minimal API endpoints
- ✅ CQRS with MediatR (Commands & Queries)
- ✅ FluentValidation pipeline
- ✅ Global exception handling
- ✅ OpenAPI/Swagger documentation

### Phase 3: Web Interface - COMPLETED
- ✅ ASP.NET Core MVC with dark theme
- ✅ Controllers: Home, Events, Users, Attendance, Language
- ✅ HttpClient service layer (EventService, UserService, AttendanceService)
- ✅ Responsive Bootstrap UI with gradient cards
- ✅ Multi-language support (TR/EN) with JSON localization
- ✅ Statistics dashboard
- ✅ Event cards with date badges

### Phase 4: QR Scanner - COMPLETED
- ✅ HTML5 QR code scanner (html5-qrcode library)
- ✅ Camera-based attendance check-in
- ✅ Real-time success/error feedback
- ✅ Mobile-responsive scanner interface
- ✅ Auto QRCode generation for users (USER-{GUID})

### Phase 5: Multi-Tenant Architecture - COMPLETED ✅
**Architecture Design:**
- ✅ Multiple SQLite databases (same schema, different data)
- ✅ Tenant identification via hard-coded username mapping
- ✅ Dynamic connection string resolution at runtime
- ✅ Tenant context service (ITenantService, TenantService)
- ✅ Scoped DbContext per request based on tenant

**Implementation:**
- ✅ ITenantService interface and TenantService implementation
- ✅ Tenant configuration in appsettings.json
- ✅ DbContext registration with dynamic connection string
- ✅ TenantMiddleware (cookie + header support)
- ✅ Separate migration/seeding for each tenant database
- ✅ API Auth endpoints (/api/auth/login, /api/auth/logout)
- ✅ Cookie-based authentication (API-managed)
- ✅ Web authentication middleware
- ✅ Clean Architecture enforcement (Web → API only, no Infrastructure reference)
- ✅ Cookie forwarding from Web to API

**Technical Implementation:**
- Web project: No Infrastructure/Application/Domain references
- API manages: Tenant resolution, cookie generation, authentication
- Web: HTTP client consumer only with cookie forwarding
- Data isolation: Complete separation per tenant database

---

## 🚧 Pending Features

### Phase 6: Admin Dashboard - NOT STARTED
**Priority: High**
- [ ] Admin role management
- [ ] Protected routes for admin actions
- [ ] Bulk operations (import/export users)
- [ ] Event management enhancements
- [ ] Attendance reports and analytics
- [ ] Admin-only pages

### Phase 7: User Management - NOT STARTED
**Priority: High**
- [ ] User list page with search/filter
- [ ] User details page with QR code display
- [ ] User edit functionality
- [ ] User profile pages
- [ ] QR code format enhancement (EVENT-{eventId}|USER-{userId})

### Phase 8: Event Enhancements - NOT STARTED
**Priority: Medium**
- [ ] Event capacity limits
- [ ] Event registration workflow
- [ ] Event details page with attendee list
- [ ] Event categories/tags
- [ ] Advanced event filtering

### Phase 9: Messaging Integration - NOT STARTED
**Priority: Low**
- [ ] QRCode image generation service
- [ ] WhatsApp integration (Twilio/Meta Business API)
- [ ] Telegram bot setup
- [ ] Send QR codes after registration
- [ ] Event reminder notifications

### Phase 10: Deployment - NOT STARTED
**Priority: High**
- [ ] Azure App Service configuration
- [ ] Environment variables setup for tenant configs
- [ ] HTTPS enforcement
- [ ] Production database migration (all tenants)
- [ ] CI/CD pipeline (.github/workflows)
- [ ] Tenant database backup strategy

---

## 📋 Technical Debt & Improvements

### High Priority
- [ ] Add proper QR code format for attendance (EVENT-{eventId}|USER-{userId})
- [ ] Implement event capacity limits
- [ ] Add attendee registration workflow
- [ ] Create user profile pages with QR code display
- [ ] Add event details page with attendee list
- [ ] Implement search and filtering on all list pages

### Medium Priority
- [ ] Add pagination controls to all lists
- [ ] Implement soft delete for entities
- [ ] Add audit logging (CreatedBy, UpdatedBy, DeletedAt)
- [ ] Create admin panel for bulk operations
- [ ] Add data export functionality (Excel/PDF)
- [ ] Implement email validation for optional email field

### Low Priority
- [ ] Add user avatars
- [ ] Event categories/tags
- [ ] Advanced reporting and analytics
- [ ] Export attendance reports
- [ ] Theme switcher (Dark/Light toggle)
- [ ] Add Turkish character support improvements

---

## 🐛 Known Issues

1. **Localization**: Turkish character display issues in JSON (i̇ instead of İ)
   - Consider using RESX files instead of JSON
   
2. **Navigation**: Some navigation links may not be implemented yet
   - Users list/details pages need completion
   
3. **Validation**: Client-side validation needs enhancement
   - Add JavaScript validation for forms

---

## 📊 Database Schema (Current)

**Schema applies to ALL tenant databases:**

```sql
Users
  - Id (Guid, PK)
  - Name (string, required, max: 200)
  - Email (string?, optional, max: 200)
  - Phone (string?, optional, max: 50)
  - QRCode (string, required, unique, max: 100)
  - CreatedAt (DateTime)

Events
  - Id (Guid, PK)
  - Title (string, required, max: 200)
  - Description (string?, optional, max: 1000)
  - Date (DateTime)
  - CreatedAt (DateTime)

Attendances
  - Id (Guid, PK)
  - UserId (Guid, FK → Users)
  - EventId (Guid, FK → Events)
  - CheckedIn (bool)
  - CheckedInAt (DateTime?)
  - Status (int: 0=Registered, 1=CheckedIn, 2=Cancelled)
  - Unique constraint on (UserId, EventId)
```

**Multi-Tenant Databases:**
- AttendDb_Erkekler.db (Tenant1)
- AttendDb_Kadinlar.db (Tenant2)
- Same structure, isolated data

---

## 🔧 Tech Stack

### Backend
- ASP.NET Core 9.0
- Carter (Minimal API)
- MediatR (CQRS)
- FluentValidation
- Entity Framework Core 9.0
- SQLite (Multi-Tenant)

### Frontend
- ASP.NET Core MVC
- Bootstrap 5.3.2 (Dark Theme)
- Bootstrap Icons 1.11.1
- html5-qrcode 2.3.8
- JSON-based Localization (TR/EN)

### Patterns & Architecture
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS Pattern
- Repository Pattern
- Factory Pattern
- Dependency Injection
- **Multi-Tenant (Database-per-Tenant)**

---

## 📝 Next Steps

### Immediate (This Week) - Phase 6
1. Admin dashboard implementation
2. User management pages (List, Details, Edit)
3. Enhanced event management
4. Attendance reports

### Short-term (Next 2 Weeks)
1. QR code format improvements
2. Event registration workflow
3. Search and filtering
4. Data export features
5. Fix localization character issues

### Long-term (Next Month)
1. Messaging integration (WhatsApp/Telegram)
2. Azure deployment setup
3. CI/CD pipeline
4. Production testing

---

## 🚀 Deployment Strategy

### Development
- Local: Multiple SQLite databases (one per tenant)
- API: http://localhost:5025
- Web: http://localhost:5xxx
- Tenant databases: AttendDb_Erkekler.db, AttendDb_Kadinlar.db

### Production (Planned)
- Azure App Service (Free Tier) × 2
  - API: attend-api.azurewebsites.net
  - Web: attend-web.azurewebsites.net
- Multiple SQLite file storage on Azure (one per tenant)
- Environment-based configuration with tenant settings
- Automated backup for all tenant databases

---

## 🏗️ Multi-Tenant Architecture

**Database-per-Tenant Strategy:**
- Complete data isolation
- Easy backup/restore per tenant
- No cross-tenant data leak risk
- Independent scaling per tenant

**Authentication Flow:**
1. User enters username on Web login page
2. Web sends POST to API `/api/auth/login`
3. API resolves tenant by username
4. API sets `TenantId` and `Username` cookies
5. API returns success
6. All subsequent Web → API requests include cookies
7. API TenantMiddleware reads cookie and sets tenant context
8. DbContext connects to correct tenant database

**Clean Architecture Enforcement:**
- Web: Only references API via HTTP
- API: Manages tenant resolution and authentication
- Infrastructure: Contains tenant service
- No cross-cutting concerns between Web and Infrastructure

---

*Last Updated: January 11, 2025*
*Status: Phase 1-5 Complete ✅, Phase 6-10 Pending*
