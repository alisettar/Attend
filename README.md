# Attend - Event Registration System

A modern, lightweight event registration and attendance tracking system built with Clean Architecture and Domain-Driven Design principles.

## Features

- 📅 **Event Management** - Create, edit, and manage events
- 👥 **User Registration** - Simple registration for attendees
- 📱 **QR Code Check-in** - Generate unique QR codes for each registration
- ✅ **Real-time Attendance** - Track attendance with QR code scanning
- 💬 **Messaging Integration** - Send QR codes via WhatsApp/Telegram
- 🔐 **Admin Authentication** - Secure admin area for event management
- 📊 **Dashboard** - Overview of events and attendees

## Tech Stack

### Backend
- **ASP.NET Core 9.0** - Modern web framework
- **Carter** - Minimal API modules
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **Entity Framework Core** - ORM
- **SQLite** - Embedded database

### Frontend
- **ASP.NET Core MVC** - Server-side rendering
- **Bootstrap 5** - Responsive UI framework
- **jQuery** - Client-side interactions
- **Html5-QRCode** - QR code scanning

### Architecture
- **Clean Architecture** - Separation of concerns
- **Domain-Driven Design** - Business logic focus
- **CQRS Pattern** - Command/Query separation
- **Repository Pattern** - Data access abstraction

## Project Structure

```
Attend/
├── src/
│   ├── Attend.Domain/          # Business entities and rules
│   ├── Attend.Application/     # Use cases (Commands/Queries)
│   ├── Attend.Infrastructure/  # Data access and external services
│   ├── Attend.API/            # HTTP API (Carter modules)
│   └── Attend.Web/            # MVC Frontend
├── docs/                       # Documentation
└── Attend.sln                 # Solution file
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 or VS Code
- SQLite (included with .NET)

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/attend.git
cd attend
```

2. Restore dependencies
```bash
dotnet restore
```

3. Run database migrations
```bash
cd src/Attend.API
dotnet ef database update
```

4. Start the API
```bash
cd src/Attend.API
dotnet run
```

5. Start the Web application (in a new terminal)
```bash
cd src/Attend.Web
dotnet run
```

6. Open your browser
- API: `https://localhost:7001`
- Web: `https://localhost:7002`

## Configuration

### API Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=AttendDb.db"
  },
  "AdminCredentials": {
    "Username": "admin",
    "Password": "your-hashed-password"
  }
}
```

### Web Configuration (appsettings.json)
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001/"
  }
}
```

## Development Roadmap

- [x] Phase 1: Core Foundation
- [ ] Phase 2: Core API Development
- [ ] Phase 3: Basic Web Interface
- [ ] Phase 4: QR Code System
- [ ] Phase 5: Admin Authentication
- [ ] Phase 6: Messaging Integration
- [ ] Phase 7: Deployment

See [Project Plan](docs/project-plan.md) for detailed development phases.

## Deployment

### Azure App Service (Free Tier)

1. Create two App Services (API + Web)
2. Configure connection strings
3. Deploy via GitHub Actions or Azure DevOps

See [Deployment Guide](docs/deployment.md) for detailed instructions.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Inspired by modern event management systems
