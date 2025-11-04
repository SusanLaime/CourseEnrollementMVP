## Repo snapshot

This is a small Razor Pages (.NET 8) app implementing a simple course enrollment MVP.

- Project root: the web app project (open `Program.cs` at the root).
- Data layer: `Data/AppDbContext.cs`, models in `Data/Models/*`.
- UI: Razor Pages under `Pages/` (e.g. `Pages/Login.cshtml.cs`, `Pages/Director/*`, `Pages/Students/*`).

## Big picture architecture (what to know first)

- Single ASP.NET Core web app (Razor Pages) hosting UI and server code.
- EF Core with a single `AppDbContext` using SQLite (configured in `Program.cs` via `UseSqlite("Data Source=app.db")`).
- Cookie-based authentication named "CookieAuth" (see `Program.cs` AddAuthentication/AddCookie and `Pages/Login.cshtml.cs`).
- Seed logic runs at startup (in `Program.cs`) and will create basic Users and Courses on first run.

Why this structure: the app keeps server-side rendering (Razor Pages) simple — pages are small handlers that use DI to get `AppDbContext` and operate directly on domain models.

## Key developer workflows and commands

- Build: `dotnet build` in the project folder (the same folder that contains the .csproj).
- Run: `dotnet run` — app will migrate the SQLite database automatically and seed sample data.
- Database: the app calls `db.Database.Migrate()` at startup so EF migrations are applied at runtime; there are no test-run migrations here, so if you add migrations use the normal EF commands in the project directory:

  - Create migration: `dotnet ef migrations add <Name>`
  - Update database locally: `dotnet ef database update` (optional because app performs migrate on startup)

Note: the app uses BCrypt for password hashing (see `Pages/Login.cshtml.cs` and seeded users in `Program.cs`). If adding users, store passwords with `BCrypt.Net.BCrypt.HashPassword("...")`.

## Project-specific conventions and patterns

- Namespace layout: Data and models live under `CourseEnrollmentMVP.Data` / `CourseEnrollmentMVP.Data.Models`.
- Razor Page handlers use constructor DI to receive `AppDbContext` and operate synchronously or asynchronously. Example: `public LoginModel(AppDbContext db) => _db = db;`.
- Role strings are literal values on `User.Role` ("Student", "Director"). Pages redirect based on these exact values (see `Login.cshtml.cs`).
- Enrollment status is a string: use "PENDING", "APPROVED", "DENIED" (see `Data/Models/Enrollment.cs`).
- Cookie auth details: cookie scheme is "CookieAuth" and cookie name is "jwt" — code expects this naming; changing it requires updating authentication and any SignIn/SignOut calls.

## Where to make common changes (with examples)

- Change DB provider or connection: edit `Program.cs` where `builder.Services.AddDbContext` is called. Example currently:

  - `options.UseSqlite("Data Source=app.db")`

- Change authentication behavior: `Program.cs` AddAuthentication/AddCookie block controls cookie name and LoginPath.
- Modify seed data: the startup seeding block in `Program.cs` inserts sample `User` and `Course` rows — update there for developer accounts.
- Login logic and claim set: `Pages/Login.cshtml.cs` shows creating Claims (Name, Email, Role, UserId) and the SignIn call. If new claims are needed, add them here.

## Integration points and external dependencies

- SQLite (EF Core) — single DB file at `app.db` by default.
- BCrypt.Net (NuGet) — used for password hashing/verification.
- Static assets live in `wwwroot/` and bundled libraries under `wwwroot/lib/` (Bootstrap, jQuery, validation).

## Small examples to follow when editing code

- Querying users (Login): `var user = _db.Users.FirstOrDefault(u => u.Email == Email);`
- Adding claims and signing-in (Login): create `ClaimsIdentity` with scheme "CookieAuth" and call `HttpContext.SignInAsync("CookieAuth", principal);`.
- Redirect paths after login: student -> `/Student/Courses`, director -> `/Director/Dashboard` (change in `Login.cshtml.cs`).

## Quick checklist for pull requests

- Update or add EF migrations if you change models and include a short note in your PR about DB changes.
- If changing authentication or role semantics, update `Pages/Login.cshtml.cs` and any page authorization checks.
- Keep model changes backward-compatible with seeded data or update the seed block accordingly.

## If you need more context

- Look at these files for canonical examples:
  - `Program.cs` (DI, DB, auth, seed)
  - `Data/AppDbContext.cs` (DbSets)
  - `Data/Models/*` (domain models)
  - `Pages/Login.cshtml.cs` (auth flow)
  - `Pages/Director/*` and `Pages/Students/*` for role-specific pages

If anything in this file is unclear or you want me to expand a section (example commands, sample PR checklist, or add common quick-fixes), tell me what to add or change.
