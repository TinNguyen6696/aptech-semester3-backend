# Talent Platform — Backend API

Backend API for the Talent Platform, a place where users showcase their talents through videos, engage with communities, join contests, and connect with mentors and recruiters. Built with ASP.NET Core 9 and EF Core.

---

## Tech Stack

| Category | Technology |
|---|---|
| Framework | [ASP.NET Core 9](https://learn.microsoft.com/aspnet/core) |
| Language | [C# 13](https://learn.microsoft.com/dotnet/csharp/) (.NET 9) |
| ORM | [Entity Framework Core 9](https://learn.microsoft.com/ef/core/) |
| Database | [SQL Server](https://www.microsoft.com/sql-server) |
| Authentication | [JWT Bearer](https://learn.microsoft.com/aspnet/core/security/authentication/jwt) — access + refresh token |
| API Docs | [Swagger / Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) + OpenAPI |
| API Testing | [Postman](https://www.postman.com/) |
| Architecture | Layered (Controller → Service → Repository) |

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) (all platforms)
- [EF Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet) — `dotnet tool install --global dotnet-ef`
- SQL Server:
  - **macOS / Linux** — [Docker](https://www.docker.com/products/docker-desktop/) (SQL Server runs in a container)
  - **Windows** — [SQL Server Express / Developer](https://www.microsoft.com/sql-server/sql-server-downloads) or LocalDB (ships with Visual Studio)

---

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd TalentShowcase.Api
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Set up SQL Server

<details open>
<summary><b>macOS / Linux (Docker)</b></summary>

Run SQL Server in a container:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name talentshowcase-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

- The container listens on port `1433`.
- `sa` is the default admin user; the password is whatever you set in `MSSQL_SA_PASSWORD` (must meet SQL Server's complexity rules: 8+ chars, upper, lower, digit, symbol).
- To stop / start it again later: `docker stop talentshowcase-sql` / `docker start talentshowcase-sql`.

</details>

<details>
<summary><b>Windows (SQL Server / LocalDB)</b></summary>

Install **SQL Server Express/Developer** or use **LocalDB** (already installed with Visual Studio). No container needed — the service runs natively.

- LocalDB server name: `(localdb)\\MSSQLLocalDB`
- SQL Server Express server name: `localhost\\SQLEXPRESS`

You can use **Windows Authentication** (no username/password needed).

</details>

### 4. Configure settings

Set your connection string and JWT config in `appsettings.json`.

<details open>
<summary><b>macOS / Linux (Docker — SQL auth)</b></summary>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TalentShowcase;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<a-long-random-secret-key-at-least-32-chars>",
    "Issuer": "TalentShowcase.Api",
    "Audience": "TalentShowcase.Client",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

> Use the same password you passed to `MSSQL_SA_PASSWORD` in step 3.

</details>

<details>
<summary><b>Windows (Windows Authentication)</b></summary>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TalentShowcase;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<a-long-random-secret-key-at-least-32-chars>",
    "Issuer": "TalentShowcase.Api",
    "Audience": "TalentShowcase.Client",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

> For LocalDB, use `Server=(localdb)\\MSSQLLocalDB` instead. `Trusted_Connection=True` uses your Windows account — no username/password required.

</details>

### 5. Apply database migrations

```bash
dotnet ef database update
```

This creates the `TalentShowcase` database and all tables.

### 6. Run the API

```bash
dotnet run
```

The API will be available at:

- HTTP — `http://localhost:5208`
- HTTPS — `https://localhost:7285`
- Swagger UI — `http://localhost:5208/swagger`

---

## Available Commands

| Command | Description |
|---|---|
| `dotnet run` | Start the API |
| `dotnet build` | Build the project |
| `dotnet ef migrations add <Name>` | Create a new migration |
| `dotnet ef database update` | Apply migrations to the database |
| `dotnet ef migrations remove` | Remove the last migration |

---

## Testing the API

The API is tested using [Postman](https://www.postman.com/).

### Setup

1. Make sure the API is running (`dotnet run`).
2. In Postman, create an environment with a `baseUrl` variable set to `http://localhost:5208`.
3. For protected endpoints, set the `Authorization` header to `Bearer {{accessToken}}`, where `accessToken` is saved after signing in.

### Typical auth flow

| Step | Method | Endpoint | Notes |
|---|---|---|---|
| Sign up | `POST` | `/api/auth/register` | Create a new account |
| Sign in | `POST` | `/api/auth/login` | Returns access + refresh token |
| Get current user | `GET` | `/api/auth/me` | Requires `Bearer` access token |
| Refresh token | `POST` | `/api/auth/refresh` | Exchange refresh token for a new access token |

> Tip: in Postman, save the access token to an environment variable in the login request's **Scripts** tab so it's reused automatically across requests.

---

## Architecture

The project follows a layered architecture to keep concerns separated:

```
Controller   → handles HTTP requests/responses, no business logic
Service      → business logic, validation, orchestration
Repository   → data access (EF Core), no business logic
AppDbContext → EF Core configuration & DbSets
```

Both Repositories and Services are split into `Interfaces/` and `Implementations/` and registered via dependency injection in `Extensions/`.

---

## Project Structure

```
TalentShowcase.Api/
├── Common/                  # Shared types (e.g. Result<T> API wrapper)
├── Controllers/             # API entry points: receive request, call Service, return response
├── DTOs/                    # Data transfer objects exchanged with the client (decoupled from Entities)
├── Data/
│   ├── AppDbContext.cs      # EF Core configuration
│   ├── Configurations/      # Fluent API config, split per entity
│   ├── Seeders/             # Seed data (provinces, sample accounts)
│   └── Migrations/          # EF-generated migrations
├── Models/
│   ├── Entities/            # Database table classes + BaseEntity
│   └── Enums/               # UserRole, TalentCategory, VideoVisibility, SkillLevel, AchievementType
├── Repositories/
│   ├── Interfaces/          # Repository contracts (Generic + per-entity)
│   └── Implementations/     # Repository implementations
├── Services/
│   ├── Interfaces/          # Service contracts
│   └── Implementations/     # Business logic
├── Helpers/                 # JWT, password hashing, mapping
├── Middlewares/             # jti denylist check, centralized error handling
├── Extensions/              # DI / Auth registration to keep Program.cs clean
├── appsettings.json         # ConnectionString, JWT config
└── Program.cs               # Application startup
```

---

## Authentication

The API uses JWT with an access + refresh token strategy:

- **Access token** — short-lived, sent on every request via the `Authorization: Bearer` header.
- **Refresh token** — long-lived, stored in the database, used to obtain a new access token.
- **jti denylist** — a middleware checks each request's token `jti` against a denylist table to support logout/revocation.

> Detailed auth flow and endpoints will be documented as the feature is implemented.

---

## API Response Format

All endpoints return a consistent envelope via `Result<T>`:

```json
{
  "data": {},
  "isSuccess": true,
  "mes": "Optional message"
}
```
