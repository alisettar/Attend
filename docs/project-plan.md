# Attend - Event Registration System
## Project Development Plan

### Project Information
- **Project Location**: `C:\Users\Alisettar\source\repos\Attend\`
- **Version Control**: Git
- **Current Status**: Phase 9 Complete ✅
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

### Phase 6: User Management & QR Generation - COMPLETED ✅
**Completed:**
- ✅ User Details page with full information display
- ✅ User Details page with attendance history table (paginated)
- ✅ Backend QR code image generation (QRCoder library)
- ✅ User.QRCodeImage field (base64 PNG storage)
- ✅ QRCodeService (IQRCodeService interface + implementation)
- ✅ Automatic QR generation on user creation
- ✅ QR generation during database seeding
- ✅ QR code display and download functionality
- ✅ User list with Details/Edit/Delete actions
- ✅ Full localization support
- ✅ Optional fields (Email, Phone) - only Name required

### Phase 7: Event Management - COMPLETED ✅
**Completed:**
- ✅ Event Details page with attendee list
- ✅ Event statistics (Total Registered, Checked In, Cancelled)
- ✅ Status filtering (All/Registered/CheckedIn/Cancelled)
- ✅ Pagination UI with smart ellipsis
- ✅ Full TR/EN localization
- ✅ Quick Check-in button (→ Scanner page)
- ✅ Responsive table design with icons

### Phase 8: QR Scanner Revision - COMPLETED ✅
**Completed:**
- ✅ Revised Scanner page (ScannerController)
- ✅ Event selection dropdown (required)
- ✅ USB Scanner support (keyboard emulation)
- ✅ Auto-focus + refocus logic (smart focus management)
- ✅ Camera Scanner integration (html5-qrcode)
- ✅ Dual mode: USB/Camera toggle
- ✅ Insert/Update logic (auto-register if not exists)
- ✅ Duplicate check handling (AlreadyCheckedIn status)
- ✅ User name + status feedback (3s auto-hide)
- ✅ Backend: CheckInByQRCodeCommand with EventId
- ✅ API: CheckInResult response (userName, isNewCheckIn, status)
- ✅ AttendanceResponse.Status → string conversion
- ✅ Full localization (30+ new keys)
- ✅ Navbar and Home page links updated

### Phase 9: Reports Dashboard - COMPLETED ✅
**Completed:**
- ✅ Reports Dashboard page (ReportsController)
- ✅ GetDashboardStatisticsQuery (MediatR)
- ✅ Dashboard statistics (Total Events, Users, Attendances, Check-ins)
- ✅ Check-in rate calculation and progress bar
- ✅ Top 5 Events by check-in count
- ✅ Top 5 Active Users/Participants by check-in count
- ✅ ReportService (Web layer)
- ✅ Gradient stat cards matching Home page design
- ✅ Full TR/EN localization (10+ new keys)
- ✅ Navbar Reports link

### UI/UX Enhancements - COMPLETED ✅
**Completed:**
- ✅ Consistent widget heights (h-100 class) on Home page
- ✅ Gradient card design for Reports page matching Home
- ✅ SVG Favicon with gradient "A" logo
- ✅ Terminology update: "Kullanıcılar" → "Katılımcılar" (Users → Participants)
- ✅ All localization files updated (TR/EN)

---

## 🚧 Pending Features

### Phase 10: Event Management Enhancements - NOT STARTED
**Priority: Medium**
- [ ] Event capacity limits
- [ ] Event registration workflow
- [ ] Event categories/tags
- [ ] Advanced event filtering
- [ ] Edit event functionality improvements

### Phase 11: Messaging Integration - NOT STARTED
**Priority: Medium**
- [ ] WhatsApp integration (Twilio/Meta Business API)
- [ ] Telegram bot setup
- [ ] Send QR codes after registration
- [ ] Event reminder notifications
- [ ] Bulk messaging functionality

### Phase 12: Admin & Bulk Operations - NOT STARTED
**Priority: Medium**
- [ ] Admin role management
- [ ] Protected routes for admin actions
- [ ] Bulk user import from Excel/CSV
- [ ] Bulk user export functionality
- [ ] Data export (Excel/PDF reports)

### Phase 13: Deployment - NOT STARTED
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
- [ ] Implement soft delete for entities
- [ ] Add audit logging (CreatedBy, UpdatedBy, DeletedAt)

### Medium Priority
- [ ] Create admin panel for bulk operations
- [ ] Add user avatars
- [ ] Event categories/tags
- [ ] Advanced reporting and analytics

### Low Priority
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
1. Event capacity management
2. Admin role implementation
3. Bulk operations planning

### Short-term (Next 2 Weeks)
1. Bulk user import/export
2. Advanced event filtering
3. Event categories

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
*Status: Phase 1-9 Complete ✅, Phase 10-13 Pending*
