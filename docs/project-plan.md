# Attend - Event Registration System
## Project Development Plan

### Project Information
- **Project Location**: `C:\Users\Alisettar\source\repos\Attend\`
- **Version Control**: Git
- **Current Status**: Phase 1-9, 13-15 Complete ✅ | Phase 14 MOSTLY DONE 🚀 | DEPLOYED TO AZURE
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

### Phase 13: Deployment - COMPLETED ✅
**Priority: High**
- ✅ Azure App Service configuration (Basic Tier)
- ✅ Environment variables setup (appsettings.Production.json)
- ✅ HTTPS enforcement (Azure default)
- ✅ Production database strategy (SQLite on Azure)
- ✅ CI/CD pipeline (.github/workflows) - GitHub Actions
- ✅ Service Principal authentication
- ✅ CORS configuration for production
- ✅ Automated deployment on master branch push
- ✅ Separate API/Web workflows

**Production URLs:**
- API: https://api-gencligianlamasanati.azurewebsites.net
- Web: https://gencligianlamasanati.azurewebsites.net

### Phase 14: Public Registration Form - COMPLETED ✅
**Priority: HIGH - 100% DONE**

**Tenant Hash System - COMPLETED ✅**
- ✅ Tenant hash/slug generation (Erkekler: 7k9m2x5w, Kadınlar: 3p8n6r4t)
- ✅ appsettings.json with tenant hash mappings (Dev + Production)
- ✅ TenantService.ResolveTenantByHash() implementation
- ✅ Tenant resolution by hash in public endpoints

**Backend (API) - 90% COMPLETED ✅**
- ✅ PublicRegisterCommand (MediatR)
- ✅ PublicRegisterCommandHandler with scoped tenant service
- ✅ PublicRegisterCommandValidator (FluentValidation)
  - ✅ TR phone format validation: `^(\+90|0)?5\d{9}# Attend - Event Registration System
## Project Development Plan

### Project Information
- **Project Location**: `C:\Users\Alisettar\source\repos\Attend\`
- **Version Control**: Git
- **Current Status**: Phase 1-9, 13-15 Complete ✅ | Phase 14 MOSTLY DONE 🚀 | DEPLOYED TO AZURE
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

### Phase 13: Deployment - COMPLETED ✅
**Priority: High**
- ✅ Azure App Service configuration (Basic Tier)
- ✅ Environment variables setup (appsettings.Production.json)
- ✅ HTTPS enforcement (Azure default)
- ✅ Production database strategy (SQLite on Azure)
- ✅ CI/CD pipeline (.github/workflows) - GitHub Actions
- ✅ Service Principal authentication
- ✅ CORS configuration for production
- ✅ Automated deployment on master branch push
- ✅ Separate API/Web workflows

**Production URLs:**
- API: https://api-gencligianlamasanati.azurewebsites.net
- Web: https://gencligianlamasanati.azurewebsites.net

### Phase 14: Public Registration Form - MOSTLY COMPLETED 🚀
**Priority: HIGH - 90% DONE**

**Tenant Hash System - COMPLETED ✅**
- ✅ Tenant hash/slug generation (Erkekler: 7k9m2x5w, Kadınlar: 3p8n6r4t)
- ✅ appsettings.json with tenant hash mappings (Dev + Production)
- ✅ TenantService.ResolveTenantByHash() implementation
- ✅ Tenant resolution by hash in public endpoints


  - ✅ Duplicate phone check (tenant-scoped)
  - ✅ Name validation (required, max 200 chars)
- ✅ PublicModule: POST /api/public/register/{tenantHash}
- ✅ PublicModule: GET /api/public/user/by-phone/{tenantHash}
- ✅ GetUserByPhoneQuery (MediatR)
- ✅ Exception handling with user-friendly messages
- ✅ ReCaptchaService.cs implementation (Infrastructure layer)
- ✅ PhoneCheckRateLimitService (IP: 30/hour, Phone: 15/hour)
- ⚠️ Google reCAPTCHA v3 backend verification - **OPTIONAL** (not enforced)

**Frontend (Web) - COMPLETED ✅**
- ✅ RegisterController (GET/POST actions)
- ✅ RegisterController.CheckPhone endpoint (AJAX)
- ✅ Views/Register/Index.cshtml (Public registration form)
  - ✅ Mobile-first responsive design
  - ✅ Large touch targets (54px buttons)
  - ✅ Phone input masking (TR format: 05XX XXX XX XX)
  - ✅ Auto-detect existing users (blur event)
  - ✅ QR Reminder System (retrieve QR by phone)
  - ✅ Client-side validation
  - ✅ Modern gradient design matching branding
- ✅ Views/Register/Success.cshtml (QR display page)
  - ✅ QR code rendered from DB (base64)
  - ✅ User name display
  - ✅ Different messages for new/existing users
  - [ ] PNG download button - **TODO (Phase 16)**
  - [ ] WhatsApp share option - **FUTURE (Phase 11)**
- ✅ KVKK Compliance pages:
  - ✅ PrivacyPolicy.cshtml (/privacy-policy)
  - ✅ ConsentText.cshtml (/consent-text)
  - ✅ Checkbox for acceptance (required)
  - ✅ Links to policy pages in checkbox label

**Security & Compliance:**
- ✅ Google reCAPTCHA v3 frontend integration (token generation)
- ✅ ReCaptchaService backend implementation (Infrastructure/Services/ReCaptchaService.cs)
- ⚠️ Google reCAPTCHA v3 backend verification - **OPTIONAL** (not enforced in production)
- ✅ PhoneCheckRateLimitService (IP: 30/hour, Phone: 15/hour)
- ✅ Rate limiting on CheckPhone endpoint
- ✅ CSRF token validation (AntiForgeryToken)
- ✅ Input sanitization (FluentValidation)
- ✅ KVKK compliance text
- ✅ Açık Rıza Metni (explicit consent)

**Additional Features:**
- ✅ QR Reminder System (retrieve QR by phone number)
- ✅ Auto-detect existing users during registration
- ✅ Rate limiting to prevent abuse
- ✅ Event statistics endpoint (GET /events/{id}/statistics)
- ✅ DateTimeExtensions for Turkey timezone
- ✅ Scanner feedback area moved to top

**Testing:**
- [ ] Unit tests (validator, command handler) - **FUTURE**
- [ ] Integration tests (duplicate check, rate limit) - **FUTURE**
- ✅ Manual mobile responsive testing
- [ ] reCAPTCHA integration test - **FUTURE**

**Public Registration URL Format:**
- Erkekler (Men): https://gencligianlamasanati.azurewebsites.net/register/7k9m2x5w
- Kadınlar (Women): https://gencligianlamasanati.azurewebsites.net/register/3p8n6r4t

### Phase 15: Branding System - COMPLETED ✅
**Completed:**
- ✅ BrandingSettings model (AppName, CompanyName, Colors, Logo, Favicon)
- ✅ Dynamic configuration via appsettings.json
- ✅ Email anti-spam format (icon-based @ symbol)
- ✅ Full integration to all pages:
  - ✅ _Layout.cshtml (title, favicon, navbar, footer, gradient border)
  - ✅ Login.cshtml (dark theme optimized)
  - ✅ Register/Index.cshtml
  - ✅ Register/Success.cshtml
  - ✅ PrivacyPolicy.cshtml
  - ✅ ConsentText.cshtml
- ✅ site.css updated with brand colors (#667eea, #764ba2)
- ✅ Balanced color application (accent points only)
- ✅ CSS gradient variables (--accent-gradient)
- ✅ Navbar branding border stripe
- ✅ Card hover effects with primary color
- ✅ Button gradients
- ✅ Stat card top accent stripe
- ✅ Documentation (BRANDING.md)

**Email Protection:**
- Email format: `user<i class="fa-solid fa-at"></i>domain.com`
- Prevents spam scraping while maintaining visibility

### UI/UX Enhancements - COMPLETED ✅
**Completed:**
- ✅ Consistent widget heights (h-100 class) on Home page
- ✅ Gradient card design for Reports page matching Home
- ✅ SVG Favicon with gradient "A" logo
- ✅ Terminology update: "Kullanıcılar" → "Katılımcılar" (Users → Participants)
- ✅ All localization files updated (TR/EN)
- ✅ Dark theme optimization for login page

---

## 🚧 Pending Features

### Phase 16: Minor Enhancements - NOT STARTED
**Priority: Low**
- [ ] QR code PNG download button on Success page
- [ ] Unit tests for PublicRegisterCommandValidator
- [ ] Integration tests for duplicate phone check
- [ ] Localization keys validation (ensure all keys exist)
- [ ] Scanner page checkbox size increase

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
- [ ] WhatsApp share button on Registration Success page

### Phase 12: Admin & Bulk Operations - NOT STARTED
**Priority: Medium**
- [ ] Admin role management
- [ ] Protected routes for admin actions
- [ ] Bulk user import from Excel/CSV
- [ ] Bulk user export functionality
- [ ] Data export (Excel/PDF reports)

---

## 📋 Technical Debt & Improvements

### High Priority
- [ ] Add comprehensive unit test coverage
- [ ] Implement event capacity limits
- [ ] Add attendee registration workflow
- [ ] Implement soft delete for entities
- [ ] Add audit logging (CreatedBy, UpdatedBy, DeletedAt)

### Medium Priority
- [ ] Create admin panel for bulk operations
- [ ] Add user avatars
- [ ] Event categories/tags
- [ ] Advanced reporting and analytics
- [ ] Performance monitoring (Application Insights)

### Low Priority
- [ ] Theme switcher (Dark/Light toggle)
- [ ] Email notifications
- [ ] Custom domain setup (non-Azure domain)

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
- AttendDb_Erkekler.db (Tenant1, Hash: 7k9m2x5w)
- AttendDb_Kadinlar.db (Tenant2, Hash: 3p8n6r4t)
- Same structure, isolated data
- **Tenant Hash Mapping**: Hash values used in public URLs for security
- **Note:** Database files excluded from git (.gitignore), manual upload to Azure required

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
- Font Awesome 6.4.0 (@ icon for email)
- Google reCAPTCHA v3 (spam prevention)
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
1. **Phase 14 Completion** ⭐
   - Backend reCAPTCHA verification
   - Rate limiting middleware
   - QR download button
   - Testing
2. Production monitoring setup
3. Custom domain configuration

### Short-term (Next 2 Weeks)
1. Unit & integration test coverage
2. Bulk user import/export (Phase 12)
3. Advanced event filtering (Phase 10)
4. Performance optimization

### Long-term (Next Month)
1. Messaging integration (WhatsApp/Telegram) - Phase 11
2. Event categories/tags - Phase 10
3. Admin panel enhancements - Phase 12
4. Advanced analytics dashboard

---

## 🚀 Deployment Strategy

### Development
- Local: Multiple SQLite databases (one per tenant)
- API: http://localhost:5025
- Web: http://localhost:5xxx
- Tenant databases: AttendDb_Erkekler.db, AttendDb_Kadinlar.db

### Production (LIVE ✅)
- **Azure App Service (Basic Tier) × 2**
  - API: https://api-gencligianlamasanati.azurewebsites.net
  - Web: https://gencligianlamasanati.azurewebsites.net
- SQLite databases stored on Azure file system
- Database deployment: Manual upload via Kudu Console (/home/site/wwwroot/Data/)
- Environment-based configuration (appsettings.Production.json)
- GitHub Actions CI/CD (Service Principal auth)
- Auto-deploy on master branch push
- Separate workflows for API and Web
- Database backups: Manual (download via Kudu)

**Branding Configuration:**
- Managed via appsettings.json
- Colors: #667eea (primary), #764ba2 (secondary)
- Email format: Anti-spam icon protection
- Favicon: SVG gradient logo

**Public Registration URLs:**
- Erkekler: https://gencligianlamasanati.azurewebsites.net/register/7k9m2x5w
- Kadınlar: https://gencligianlamasanati.azurewebsites.net/register/3p8n6r4t

---

## 🏗️ Multi-Tenant Architecture

**Database-per-Tenant Strategy:**
- Complete data isolation
- Easy backup/restore per tenant
- No cross-tenant data leak risk
- Independent scaling per tenant
- Database files NOT in source control (security + size)

**Authentication Flow:**
1. User enters username on Web login page
2. Web sends POST to API `/api/auth/login`
3. API resolves tenant by username
4. API sets `TenantId` and `Username` cookies
5. API returns success
6. All subsequent Web → API requests include cookies
7. API TenantMiddleware reads cookie and sets tenant context
8. DbContext connects to correct tenant database

**Public Registration Flow:**
1. User visits `/register/{tenantHash}` (no authentication)
2. Web displays registration form
3. User fills name, phone, accepts KVKK
4. Form submits to API `/api/public/register/{tenantHash}`
5. API resolves tenant from hash
6. API validates phone uniqueness within tenant
7. API creates user with auto-generated QR code
8. Web redirects to success page with QR code display

**QR Code Generation:**
- Backend generation using QRCoder library
- Base64 PNG storage in database
- Automatic generation on user creation and seeding
- Ready for WhatsApp/Email integration

**Branding System:**
- Whitelabel-ready architecture
- Per-deployment configuration
- Dynamic colors, logo, company info
- Email spam protection via icon format

**Clean Architecture Enforcement:**
- Web: Only references API via HTTP
- API: Manages tenant resolution and authentication
- Infrastructure: Contains tenant service and QR generation
- No cross-cutting concerns between Web and Infrastructure

---

## 📚 Documentation

- `BRANDING.md` - Whitelabel/rebranding guide
- `AZURE_TRANSFER.md` - Deployment transfer guide
- `AZURE_COMMANDS.md` - Windows PowerShell commands
- `README.md` - Project overview
- `project-plan.md` - This file (development roadmap)

---

*Last Updated: December 7, 2024*
*Status: Phase 1-9, 13-15 Complete ✅ | Phase 14: 100% DONE 🎉 | Phase 10-12, 16 Pending | DEPLOYED TO AZURE 🚀*
