# Complaint & Leave Management System

A role-based ASP.NET Core MVC training project developed during my summer training with the IT Department at THDC India Limited.

> This is a portfolio/training project, not an official THDC India Limited product.

## End-to-end workflows

### Employee
- Register and log in
- Passwords are stored with ASP.NET Core password hashing
- Submit a complaint
- View only their own complaints
- Edit a complaint while it is still pending
- View the admin response and final status
- Submit a leave application
- View only their own leave requests and decisions

### Admin
- Protected admin dashboard
- Review all complaints
- Respond and mark complaints Resolved or Rejected
- Review leave applications
- Approve or reject leave requests
- Export complaint and leave reports as PDF

## Stack

- ASP.NET Core MVC / .NET 8
- C#
- Entity Framework Core
- MySQL 8
- Razor Views
- Bootstrap
- ASP.NET Core Session
- ASP.NET Core PasswordHasher

PDF reports use a small managed PDF writer included in the repository, so there is no external wkhtmltopdf/native runtime dependency.

## Quick start

### Option A: Docker for MySQL

Requirements: .NET 8 SDK and Docker Desktop.

```bash
docker compose up -d mysql
dotnet restore
dotnet run
```

The development connection string matches the disposable Docker database. On first development startup the app creates the required schema automatically.

Open the HTTPS/HTTP URL printed by `dotnet run`.

### Demo admin

Development startup creates a synthetic local admin if it does not already exist:

```text
Email: admin@training.local
Password: TrainingAdmin123!
```

This account is for local demonstration only. Do not reuse these credentials anywhere else.

Normal users should register through the UI.

## Existing older local database

Earlier versions of this training project stored passwords as plaintext. The current login flow can accept a matching legacy password once and immediately upgrades it to an ASP.NET Core password hash.

The former hard-coded personal MySQL password has been removed from the current source. If that password was ever used outside a disposable local database, rotate it because Git history may still contain the old value.

## Security improvements in the current version

- Server-side session checks protect user workflows.
- Every admin action verifies the Admin session role.
- Complaint ownership is enforced before edits.
- Leave identity is taken from the logged-in session rather than trusted form fields.
- POST actions use anti-forgery validation.
- Passwords are hashed.
- Registration and reset no longer depend on external stored procedures.
- Admin PDF exports are protected.
- Local database credentials are explicitly disposable development values.

## Build verification

```bash
dotnet restore WebPage.sln
dotnet build WebPage.sln --configuration Release
```

GitHub Actions runs restore + Release build for pushes and pull requests.

## Project structure

```text
Controllers/
  AccountController.cs
  AdminController.cs
  ComplaintController.cs
  LeaveController.cs
  PdfController.cs
Data/
Models/
Services/
  SimplePdf.cs
Views/
Program.cs
WebPage.csproj
docker-compose.yml
```

## Notes

- Development uses `Database.EnsureCreated` for a frictionless portfolio/demo setup. A production system should use reviewed EF Core migrations.
- Session authentication is suitable for this training project; a production deployment should use ASP.NET Core Identity/cookie authentication with hardened account recovery and operational controls.
- THDC-branded/public-site imagery in old dashboard views is part of the training UI imitation and should not be presented as an official THDC service.

## Author

**Satyam Shukla**

GitHub: https://github.com/bytebysatyam  
LinkedIn: https://www.linkedin.com/in/satyamshukla10/  
Portfolio: https://satyam-shukla-portfolio-jet.vercel.app/
