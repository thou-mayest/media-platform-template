# Media Platform Template

Modular .NET 8 API, .NET Aspire development host, PostgreSQL persistence, and a static Astro frontend.

## Prerequisites

- .NET SDK 10.0.202, used to build the `net8.0` projects and `.slnx` solution
- Node.js 22.12 or newer
- Docker Desktop for Aspire PostgreSQL and container builds

## Local Development

Configure Aspire secrets once:

```powershell
dotnet user-secrets set "Parameters:password" "a-strong-postgres-password" --project src/CleanModular.AppHost
dotnet user-secrets set "Parameters:jwt-signing-key" "a-random-signing-key-with-at-least-32-characters" --project src/CleanModular.AppHost
dotnet user-secrets set "Parameters:bootstrap-admin-password" "a-strong-admin-password" --project src/CleanModular.AppHost
```

Then run the complete development environment:

```powershell
dotnet run --project src/CleanModular.AppHost
```

The bootstrap administrator email defaults to `admin@example.test` in Development. Change `BootstrapAdmin__Email` and `BootstrapAdmin__Name` as needed. The bootstrap password is never stored as plaintext.

## Authentication

Obtain a bearer token:

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@example.test",
  "password": "your-bootstrap-password"
}
```

All `/api/users` endpoints require an administrator JWT.

## Production Configuration

Required API settings:

- `ConnectionStrings__MainDb`
- `Jwt__SigningKey` with at least 32 characters
- `Jwt__Issuer`
- `Jwt__Audience`
- `BootstrapAdmin__Email` and `BootstrapAdmin__Password` for initial provisioning
- `ReverseProxy__KnownProxies__0` (and subsequent indexes) for each trusted proxy address
- `Cors__AllowedOrigins__0` (and subsequent indexes) for each deployed frontend origin

Set `Database__ApplyMigrations=true` only for a controlled migration instance or job. Normal production API replicas should not perform schema changes during startup.

Required frontend build settings:

- `SITE_URL`, an HTTPS public origin
- `PUBLIC_INQUIRY_EMAIL`, the public artwork inquiry address
- `PUBLIC_API_URL`, the public API root origin used for artwork view tracking

Artwork views are aggregate counts only. The API does not persist IP addresses,
user agents, referrers, or visitor identifiers. The browser attempts one view per
artwork per tab session, while API and edge rate limits mitigate automated inflation.
The homepage's Newest section uses explicit catalog-added dates; Most viewed is
shown only after the API has real view data and displays the recorded counts.

## Validation

```powershell
dotnet restore CleanModular.slnx
dotnet build CleanModular.slnx --no-restore --configuration Release --warnaserror
dotnet test src/Tests/ArchTests/CleanModular.ArchTests.csproj --no-build --configuration Release

$env:SITE_URL = "https://example.test"
$env:PUBLIC_INQUIRY_EMAIL = "inquiries@example.test"
npm ci --prefix AstroFrontend
npm run check --prefix AstroFrontend
npm run audit --prefix AstroFrontend
```

Build the API container from the repository root:

```powershell
docker build --file src/Host.WebApi/Dockerfile --tag media-platform-api .
```
