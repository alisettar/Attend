# Attend - Event Registration System
## Project Development Plan

### Project Information
- **Project Location**: `C:\Users\Alisettar\source\repos\Attend\`
- **Version Control**: Git
- **Current Status**: Phase 6 In Progress 🚧
- **Database**: SQLite Multi-Tenant (Database per Tenant)

---

## ✅ Completed Features

### Phase 1: Core Foundation - COMPLETED
- ✅ Clean Architecture structure (Domain, Application, Infrastructure, API, Web)
- ✅ Domain entities with factory methods
- ✅ EF Core with SQLite + Configurations
- ✅ Repository Pattern implementation
- ✅ Database migrations
- ✅ Design-time DbContext factory (removed in Phase 5.1)

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

**Phase 5.1: Database Seeding & Search Improvements - COMPLETED ✅**
- ✅ Tenant-specific seed files (participants_men.json / participants_women.json, events_men.json / events_women.json)
- ✅ DatabaseSeeder reads tenant from connection string
- ✅ Removed DefaultConnection fallback (tenant required)
- ✅ Removed AttendDbContextFactory (no longer needed)
- ✅ Fixed user seeding with JsonSerializerOptions (PropertyNameCaseInsensitive)
- ✅ Case-insensitive search (ToUpper for EF Core/SQLite compatibility)
- ✅ Smart pagination (max 5 pages with ellipsis)
- ✅ Complete localization for all UI pages

### Phase 6: User Management & QR Generation - IN PROGRESS 🚧
**Completed:**
- ✅ User Details page with full information display
- ✅ Backend QR code image generation (QRCoder library)
- ✅ User.QRCodeImage field (base64 PNG storage)
- ✅ QRCodeService (IQRCodeService interface + implementation)
- ✅ Automatic QR generation on user creation
- ✅ QR generation during database seeding
- ✅ QR code display and download functionality
- ✅ User list with Details/Edit/Delete actions
- ✅ Full localization support

**Pending:**
- [ ] Admin role management
- [ ] Protected routes for admin actions
- [ ] Bulk operations (import/export users)
- [ ] User bulk import from Excel/CSV
- [ ] Event management enhancements
- [ ] Attendance reports and analytics

---

## 🚧 Pending Features

### Phase 7: Event Management - NOT STARTED
**Priority: High**
- [ ] Event details page with attendee list
- [ ] Event capacity limits
- [ ] Event registration workflow
- [ ] Event categories/tags
- [ ] Advanced event filtering
- [ ] Edit event functionality

### Phase 8: Attendance & Reports - NOT STARTED
**Priority: High**
- [ ] Attendance reports by event
- [ ] Attendance reports by user
- [ ] Export reports (Excel/PDF)
- [ ] Statistics and analytics dashboard
- [ ] Attendance history view

### Phase 9: Messaging Integration - NOT STARTED
**Priority: Medium**
- [ ] WhatsApp integration (Twilio/Meta Business API)
- [ ] Telegram bot setup
- [ ] Send QR codes after registration
- [ ] Event reminder notifications
- [ ] Bulk messaging functionality

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
- [ ] Implement event capacity limits
- [ ] Add attendee registration workflow
- [ ] Add event details page with attendee list
- [ ] Implement soft delete for entities
- [ ] Add audit logging (CreatedBy, UpdatedBy, DeletedAt)

### Medium Priority
- [ ] Create admin panel for bulk operations
- [ ] Add data export functionality (Excel/PDF)
- [ ] Add user avatars
- [ ] Event categories/tags
- [ ] Advanced reporting and analytics

### Low Priority
- [ ] Export attendance reports
- [ ] Theme switcher (Dark/Light toggle)
- [ ] Email notifications

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
  - QRCodeImage (string?, optional, base64 PNG)
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
- QRCoder 1.6.0 (QR image generation)

### Frontend
- ASP.NET Core MVC
- Bootstrap 5.3.2 (Dark Theme)
- Bootstrap Icons 1.11.1
- JSON-based Localization (TR/EN)

### Patterns & Architecture
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS Pattern
- Repository Pattern
- Factory Pattern
- Service Pattern
- Dependency Injection
- **Multi-Tenant (Database-per-Tenant)**

---

## 📝 Next Steps

### Immediate (This Week)
1. Event Details page with attendee list
2. Event registration workflow
3. Attendance reports

### Short-term (Next 2 Weeks)
1. Admin role management
2. Bulk operations
3. Data export features
4. Event capacity limits

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

**QR Code Generation:**
- Backend generation using QRCoder library
- Base64 PNG storage in database
- Automatic generation on user creation and seeding
- Ready for WhatsApp/Email integration

**Clean Architecture Enforcement:**
- Web: Only references API via HTTP
- API: Manages tenant resolution and authentication
- Infrastructure: Contains tenant service and QR generation
- No cross-cutting concerns between Web and Infrastructure

---

*Last Updated: October 12, 2025*
*Status: Phase 1-5 Complete ✅, Phase 6 In Progress 🚧, Phase 7-10 Pending*
