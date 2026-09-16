# Project Overview

A production-ready template for building modular monolith applications using .NET and Clean Architecture. Each module is fully isolated with its own clean architecture: application, domain, infrastructure, and presentation layers, enabling independent development while maintaining a single deployable application.

The frontend is built with Astro, providing a modern, high-performance web experience. The development environment is orchestrated using .NET Aspire, while production deployments are containerized with Docker Compose and fronted by Traefik as a reverse proxy.

## Architecture Overview

- Modular Monolith architecture
- Clean Architecture per module
- CQRS and Domain-Driven Design (DDD)
- Integration events with asynchronous messaging
- PostgreSQL and Entity Framework Core
- Docker Compose production deployment
- .NET Aspire local orchestration
- Traefik reverse proxy
- CI/CD ready

# docs

# migrations

- create migraitons, example for user module:
```powershell
dotnet ef migrations add Initial --project .\src\Users.Infrastracture\Users.Infrastracture.csproj --startup-project .\src\Host.WebApi\Host.WebApi.csproj --context UsersDbContext -o Migrations
```

start container without aspire (for persistent container aspire doesnt allow port mapping and persistent containers are acting up)
```bash
docker run --name postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -v ./postgres-data:/var/lib/postgresql postgres:18.3
```

## Development seed data

The API applies migrations and then idempotently creates the users configured in the
`UserSeed` section. Seeding is enabled only in `appsettings.Development.json` by default.
Existing users are matched by normalized email and are left unchanged, so restarting the
API does not create duplicates.

The development accounts are:

| Role | Email | Password |
| --- | --- | --- |
| Admin | `admin@localhost.test` | `Admin123!` |
| User | `user@localhost.test` | `User123!` |
| Premium user | `premium@localhost.test` | `Premium123!` |

Set `UserSeed:Enabled` to `false`, or override the section with user secrets or
environment variables, when these local-only accounts are not wanted. User creation goes
through the domain and repository pipeline, so `UserCreated` events are emitted for newly
seeded users and downstream modules can provision related profiles.
