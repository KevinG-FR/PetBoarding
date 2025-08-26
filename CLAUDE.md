# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Architecture

This is a **PetBoarding** application with a full-stack architecture consisting of:

### Backend (.NET Clean Architecture)
Located in `Core_PetBoarding_Backend/` with the following layers:
- **PetBoarding_Api**: Presentation layer using minimal APIs with endpoint mapping pattern
- **PetBoarding_Domain**: Core business logic with entities, value objects, and domain services  
- **PetBoarding_Application**: Application layer implementing CQRS with commands/queries and handlers
- **PetBoarding_Infrastructure**: External service implementations (JWT, authentication)
- **PetBoarding_Persistence**: Data access with Entity Framework Core and PostgreSQL
- **Tests/ArchitectureTests**: Architecture constraint validation tests

### Frontend (Angular 19)
Located in `ng_PetBoarding_app/` with:
- Standalone components architecture (no NgModules)
- Feature-based structure with shared components/services
- Angular Material + Bootstrap 5.3.7 for UI
- Signals for state management
- Feature modules: auth, prestations, reservations, profile, pets, vaccinations, basket

## Development Commands

### Backend (.NET)
```bash
# Navigate to backend directory
cd Core_PetBoarding_Backend

# Build entire solution
dotnet build

# Run API locally (from PetBoarding_Api directory)
cd PetBoarding_Api
dotnet run

# Run with Docker (includes PostgreSQL database)
docker-compose up --build

# Entity Framework migrations
dotnet ef migrations add <MigrationName> --project PetBoarding_Persistence --startup-project PetBoarding_Api
dotnet ef database update --project PetBoarding_Persistence --startup-project PetBoarding_Api

# Run architecture tests
dotnet test Tests/ArchitectureTests/
```

### Frontend (Angular)
```bash
# Navigate to frontend directory  
cd ng_PetBoarding_app

# Install dependencies
npm install

# Development server
npm start
# or
ng serve

# Build for production
npm run build

# Lint code with ESLint + Prettier
npm run lint

# Watch mode for development
npm run watch
```

### Docker Development
```bash
# Start full stack with PostgreSQL
cd Core_PetBoarding_Backend
docker-compose up --build

# API endpoints:
# HTTP: http://localhost:5000
# HTTPS: https://localhost:5001  
# Swagger: https://localhost:5001/swagger

# Database:
# PostgreSQL: localhost:5432
# Database: petboarding
# User/Password: postgres/postgres
```

## Key Architectural Patterns

### Backend Implementation
- **Clean Architecture**: Domain-centric with dependency inversion between layers
- **CQRS Pattern**: Commands and queries with dedicated handlers in Application layer
- **Minimal APIs**: Endpoint mapping pattern instead of traditional controllers (see `AuthenticationEndpoints.cs`)
- **Repository + UoW**: Data access abstraction with Unit of Work pattern
- **JWT Authentication**: Bearer tokens with refresh token mechanism via interceptors
- **Value Objects**: Strong typing for domain primitives (`UserId`, `Email`, `PrestationId`, etc.)
- **Partial Classes**: Endpoint organization (e.g., `UsersEndpoints.CreateUser.cs`)

### Frontend Implementation  
- **Standalone Components**: No NgModules, using Angular 19 standalone architecture
- **Signals**: Reactive state management with Angular signals instead of traditional observables
- **Feature Organization**: Business feature folders with shared components/services separation
- **Injection Function**: Using `inject()` function instead of constructor dependency injection
- **Contracts Pattern**: DTOs in feature/contracts folders, separate from component models

### Authentication Flow
- JWT access tokens (1-hour expiry) with refresh tokens
- Permission-based authorization using claims and policies
- CORS configured for Angular development server (localhost:4200)
- Interceptor handles automatic token refresh on 401 responses

## Database Configuration

### Connection & Setup
- **Database Engine**: PostgreSQL
- **Docker Host**: `petboarding.postgres.database:5432`
- **Local Host**: `localhost:5432`
- **Database Name**: `petboarding`
- **Credentials**: `postgres/postgres`

### Entity Framework
- Migrations auto-applied on startup via `ApplyMigrations()` extension
- Configuration classes in `PetBoarding_Persistence/Configurations/`
- Seeding data available for Prestations (`SeedPrestationsData.sql`)
- Repository pattern implementation in `Repositories/`

## Important Development Notes

### Technology Stack Requirements
- **.NET 8+** with C# 12 features
- **Angular 19** with TypeScript strict mode
- **Node.js** (for Angular development)
- **Docker Desktop** (for containerized development)
- **PostgreSQL** (database)

### Coding Conventions (from .github/copilot-instructions.md)
- **Backend**: Follow Clean Architecture, SOLID principles, async/await patterns, CQRS
- **Frontend**: Use standalone components, signals over observables, inject() pattern, OnPush change detection
- **File Organization**: Feature-based for specific scope, shared for cross-cutting concerns
- **DTOs**: Place in contracts folders, keep separate from component models
- **Windows 11 Compatibility**: Scripts and tools should be Windows-compatible

### API Documentation
- **Swagger/OpenAPI**: Available at `/swagger` endpoint during development
- **Authentication Endpoints**: `/api/auth/*` (login, refresh token)
- **User Management**: `/api/users/*` 
- **Business Logic**: `/api/prestations/*`, `/api/reservations/*`
- JWT Bearer token authentication required for protected endpoints

### Architecture Testing
- NetArchTest-based architecture constraint validation
- Enforces Clean Architecture dependency rules
- Validates layer separation and domain isolation
- Run with `dotnet test Tests/ArchitectureTests/`

This application manages pet boarding services with authentication, service management, and reservation booking capabilities.