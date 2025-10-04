# Attend - Event Registration System
## Project Development Plan

### Project Information
- **Project Location**: `C:\Users\Alisettar\source\repos\Attend\`
- **Version Control**: Git
- **Repository Structure**:
  ```
  Attend/
  ├── .git/
  ├── .gitignore
  ├── README.md
  ├── Attend.sln
  └── src/
      ├── Attend.Domain/
      ├── Attend.Application/
      ├── Attend.Infrastructure/
      ├── Attend.API/
      └── Attend.Web/
  ```

### Overview
Simple event registration system with QR code attendance tracking, built with Clean Architecture and DDD principles.

### Tech Stack
- **Backend API**: ASP.NET Core 9.0 (Carter Minimal API)
- **Frontend**: ASP.NET Core 9.0 MVC
- **Database**: SQLite + EF Core
- **Architecture**: Clean Architecture + DDD
- **Patterns**: CQRS (MediatR), Repository Pattern
- **Validation**: FluentValidation
- **Communication**: Web → API (HttpClient)
- **Deployment**: Azure App Service (Free Tier)

---

## Phase 1: Core Foundation
**Duration: Week 1**

### Project Structure Setup
```
Attend.sln
├── Attend.Domain
│   ├── BaseClasses/
│   │   └── Entity.cs
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Event.cs
│   │   └── Attendance.cs
│   ├── ValueObjects/
│   │   ├── QRCode.cs
│   │   ├── Email.cs
│   │   └── PhoneNumber.cs
│   └── Enums/
│       └── AttendanceStatus.cs
├── Attend.Application
│   ├── Data/
│   │   ├── Users/
│   │   │   ├── Commands/ (CreateUserCommand, UpdateUserCommand)
│   │   │   └── Queries/ (GetUsersQuery, GetUserByIdQuery)
│   │   ├── Events/
│   │   │   ├── Commands/ (CreateEventCommand, UpdateEventCommand)
│   │   │   └── Queries/ (GetEventsQuery, GetEventByIdQuery)
│   │   └── Attendances/
│   │       ├── Commands/ (RegisterAttendanceCommand, CheckInCommand)
│   │       └── Queries/ (GetAttendeesQuery)
│   ├── Repositories/
│   │   ├── IUserRepository.cs
│   │   ├── IEventRepository.cs
│   │   └── IAttendanceRepository.cs
│   ├── Behaviors/
│   │   └── ValidationPipelineBehavior.cs
│   └── DependencyInjection.cs
├── Attend.Infrastructure
│   ├── Persistence/
│   │   ├── AttendDbContext.cs
│   │   └── Configurations/
│   ├── Repositories/
│   │   ├── UserRepository.cs
│   │   ├── EventRepository.cs
│   │   └── AttendanceRepository.cs
│   ├── Services/
│   │   ├── QRCodeService.cs
│   │   ├── WhatsAppService.cs
│   │   └── TelegramService.cs
│   └── DependencyInjection.cs
├── Attend.API
│   ├── Modules/
│   │   ├── UsersModule.cs
│   │   ├── EventsModule.cs
│   │   └── AttendancesModule.cs
│   ├── Middleware/
│   │   └── GlobalExceptionHandler.cs
│   └── Program.cs
└── Attend.Web
    ├── Controllers/
    │   ├── HomeController.cs
    │   ├── EventsController.cs
    │   ├── UsersController.cs
    │   ├── AttendanceController.cs
    │   └── AdminController.cs
    ├── Services/
    │   ├── Interfaces/
    │   │   ├── IEventService.cs
    │   │   ├── IUserService.cs
    │   │   └── IAttendanceService.cs
    │   ├── EventService.cs
    │   ├── UserService.cs
    │   └── AttendanceService.cs
    ├── Models/
    │   └── ViewModels/
    ├── Views/
    │   ├── Home/
    │   ├── Events/
    │   ├── Users/
    │   ├── Attendance/
    │   ├── Admin/
    │   └── Shared/
    └── Program.cs
```

### Database Schema
```sql
Users (Id, Name, Email, Phone, CreatedAt)
Events (Id, Title, Description, Date, Location, MaxCapacity, QRCode, CreatedAt)
Attendances (Id, UserId, EventId, RegisteredAt, CheckedIn, CheckedInAt)
```

### Deliverables
- [ ] Solution structure
- [ ] Domain entities
- [ ] EF Core setup with SQLite
- [ ] Basic repositories

---

## Phase 2: Core API Development
**Duration: Week 2**

### API Endpoints (Carter Modules)
```
User Management:
GET    /users
POST   /users
GET    /users/{id}
PUT    /users/{id}
DELETE /users/{id}

Event Management:
GET    /events
POST   /events
GET    /events/{id}
PUT    /events/{id}
DELETE /events/{id}

Attendance & Registration:
POST   /events/{eventId}/register
GET    /events/{eventId}/attendees
POST   /events/{eventId}/checkin
GET    /attendances/{attendanceId}/qrcode

Admin Authentication:
POST   /admin/login
POST   /admin/logout
GET    /admin/verify
```

### Domain Services & External Services
- **QRCodeService**: Generate/validate QR codes (QRCoder library)
- **IMessagingService**: Interface for messaging services
  - **WhatsAppService**: Twilio or Meta Business API
  - **TelegramService**: Telegram Bot API
- **AttendanceService**: Registration and check-in logic

### Deliverables
- [ ] Complete API endpoints
- [ ] QR code generation
- [ ] Basic validation rules
- [ ] API testing (Postman/Swagger)

---

## Phase 3: Basic Web Interface
**Duration: Week 2**

### MVC Controllers & Views
- **HomeController**: Dashboard, event list
- **EventsController**: Create, edit, details, attendee list
- **UsersController**: User CRUD operations
- **AttendanceController**: QR scanner, check-in page
- **AdminController**: Login, dashboard, protected routes

### Service Layer (Web → API Communication)
- **IEventService / EventService**: HttpClient wrapper for events API
- **IUserService / UserService**: HttpClient wrapper for users API
- **IAttendanceService / AttendanceService**: HttpClient wrapper for attendance API
- **Service responsibilities**:
  - API endpoint calls via HttpClient
  - Error handling and logging
  - JSON serialization/deserialization
  - ViewModel transformations

### Key Features
- Event CRUD operations
- User registration forms
- QR code display and scanner
- Check-in interface (web-based)
- Admin authentication middleware
- Bootstrap responsive UI

### Deliverables
- [ ] Bootstrap-based responsive UI
- [ ] Basic forms and validation
- [ ] QR code display
- [ ] Event management interface

---

## Phase 4: QR Code Attendance System
**Duration: Week 1**

### QR Code Implementation
- **Generation**: Unique QR per event registration
- **Content**: `{EventId}-{UserId}-{Token}`
- **Scanning**: Web-based QR scanner
- **Validation**: Server-side checkin

### Mobile-Friendly Scanner
- HTML5 QR code scanner
- One-click checkin
- Success/error feedback

### Deliverables
- [ ] QR generation on registration
- [ ] Web QR scanner interface
- [ ] Checkin API integration
- [ ] Mobile responsive design

---

## Phase 5: Admin Authentication
**Duration: Week 1**

### Simple Auth System
- Cookie-based authentication
- Single admin account
- Admin area protection

### Admin Features
- Protected routes middleware
- Admin dashboard
- Event management controls
- User management

### Implementation
```csharp
// Simple admin credentials in appsettings
"AdminCredentials": {
  "Username": "admin",
  "Password": "hashed_password"
}
```

### Deliverables
- [ ] Admin login/logout
- [ ] Protected admin routes
- [ ] Admin dashboard UI

---

## Phase 6: Messaging Integration
**Duration: Week 2**

### WhatsApp Integration
- **Option 1**: Twilio WhatsApp API
- **Option 2**: Meta Business API
- Send QR codes after registration

### Telegram Integration
- Telegram Bot API (free)
- Bot setup and webhook
- Send QR codes via bot

### Messaging Service
```csharp
public interface IMessagingService
{
    Task SendQRCodeAsync(string phoneNumber, byte[] qrImage);
    Task SendEventReminderAsync(string phoneNumber, Event eventInfo);
}
```

### Deliverables
- [ ] WhatsApp service implementation
- [ ] Telegram bot setup
- [ ] Automatic QR code sending
- [ ] Manual resend functionality

---

## Phase 7: Deployment & Production
**Duration: Week 1**

### Azure Deployment
- **App Service**: 2 free tier apps (API + Web)
- **Database**: SQLite file in App_Data
- **Configuration**: Environment variables

### Production Checklist
- [ ] Environment configurations
- [ ] HTTPS enforcement
- [ ] Error handling & logging
- [ ] Performance optimization
- [ ] Security headers

### Deliverables
- [ ] Production deployment
- [ ] Domain configuration
- [ ] Basic monitoring setup

---

## Technical Architecture

### Clean Architecture Layers
```
├── Domain Layer (Business Rules)
│   ├── BaseClasses: Entity (with Guid Id, Equals override)
│   ├── Entities: User, Event, Attendance
│   │   └── Factory Methods: Create, Update
│   ├── Value Objects: QRCode, Email, PhoneNumber
│   └── Enums: AttendanceStatus
│
├── Application Layer (Use Cases - CQRS)
│   ├── Commands: CreateEvent, RegisterUser, CheckIn
│   │   ├── Command Handler (IRequestHandler<TCommand, TResult>)
│   │   └── Validator (AbstractValidator<TCommand>)
│   ├── Queries: GetEvents, GetAttendees, GetUserById
│   │   └── Query Handler (IRequestHandler<TQuery, TResult>)
│   ├── DTOs: EventDto, UserDto, AttendanceDto
│   ├── Repository Interfaces: IUserRepository, IEventRepository
│   ├── Behaviors: ValidationPipelineBehavior
│   └── MediatR Registration
│
├── Infrastructure Layer (External Concerns)
│   ├── Persistence: 
│   │   ├── AttendDbContext (EF Core)
│   │   └── Entity Configurations (Fluent API)
│   ├── Repositories: UserRepository, EventRepository
│   ├── Services: 
│   │   ├── QRCodeService (QRCoder library)
│   │   ├── WhatsAppService (Twilio/Meta API)
│   │   └── TelegramService (Bot API)
│   └── DependencyInjection
│
├── API Layer (Carter Minimal API)
│   ├── Modules: UsersModule, EventsModule, AttendancesModule
│   │   └── Endpoints (TypedResults pattern)
│   ├── Middleware: GlobalExceptionHandler
│   ├── OpenAPI/Swagger
│   └── MediatR injection
│
└── Web Layer (MVC + HttpClient)
    ├── Controllers: Classic MVC Controllers
    ├── Service Layer: 
    │   ├── HttpClient consuming API
    │   ├── Error handling and logging
    │   └── ViewModel transformations
    ├── Views: Razor pages
    ├── ViewModels: Separate from DTOs
    └── wwwroot: Bootstrap, jQuery
```

### Key Design Patterns
- **CQRS**: MediatR for Commands and Queries separation
- **Repository Pattern**: Data access abstraction
- **Factory Pattern**: Entity.Create() static methods
- **Dependency Injection**: All layers
- **Service Layer**: Web → API communication (HttpClient)
- **TypedResults**: Strongly typed HTTP responses
- **Pipeline Behaviors**: Cross-cutting concerns (Validation)

---

## Environment Setup

### Development Requirements
- .NET 8 SDK
- Visual Studio 2022 / VS Code
- SQLite Browser (optional)

### Azure Requirements
- Azure subscription (free tier)
- 2x App Service instances
- Application Insights (optional)

### Third-Party Services
- Twilio account (WhatsApp)
- Telegram Bot Token
- QR code libraries

---

## Success Metrics

### Phase Completion Criteria
- All features working end-to-end
- Code review completed
- Documentation updated

### Production Readiness
- Zero critical bugs
- Performance benchmarks met
- Security audit passed

---

## Risk Management

### Technical Risks
- **Azure free tier limits**: Monitor usage
- **SQLite concurrency**: Consider upgrade path
- **Third-party API limits**: Implement fallbacks

### Mitigation Strategies
- Incremental deployment
- Feature flags for new functionality
- Basic monitoring setup

---

*Last Updated: October 2025*
