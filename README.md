# Complaint & Leave Management System

A role-based web application developed during my summer training with the IT Department at THDC India Limited.

The project focuses on two internal workflows: employee complaint management and leave application/approval. It was built as a training project using ASP.NET Core MVC, C#, Entity Framework Core, and MySQL.

> **Portfolio note:** This repository is shared to demonstrate my engineering work completed during industrial training. It is not an official THDC India Limited product. Any organization-specific branding, confidential information, credentials, or sensitive data should be excluded from the public repository.

## Core Features

### Authentication & User Access
- User registration and login
- Session-based login tracking
- Role-aware user/admin flows
- Form validation through view models

### Complaint Management
- Submit complaints
- View user-specific complaints
- Administrative complaint review
- Admin response workflow
- Complaint status handling

### Leave Management
- Submit leave applications
- Store leave request details
- Administrative approval/review workflow

### Administration
- Admin controller for privileged workflows
- Dashboard-oriented views
- Central access to complaint and leave records

### PDF Support
- PDF-related controller and DinkToPdf integration are included in the project structure

## Tech Stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core MVC (.NET 8) |
| Language | C# |
| Database | MySQL |
| ORM | Entity Framework Core |
| MySQL Provider | Pomelo.EntityFrameworkCore.MySql |
| Session | ASP.NET Core Session |
| PDF Integration | DinkToPdf |
| Frontend | Razor Views / HTML / CSS / Bootstrap-style MVC UI |

## Application Flow

```text
User Registration / Login
          ↓
   Session Created
          ↓
 ┌────────┴─────────┐
 │                  │
 ▼                  ▼
Complaint          Leave
Workflow           Workflow
 │                  │
 ▼                  ▼
Database           Database
 │                  │
 └────────┬─────────┘
          ▼
      Admin Review
          ↓
 Response / Approval
```

## Project Structure

```text
Controllers/
├── AccountController.cs
├── AdminController.cs
├── ComplaintController.cs
├── HomeController.cs
├── LeaveController.cs
└── PdfController.cs

Data/
└── ApplicationDbContext.cs

Models/
├── ApplicationUser.cs
├── Complaint.cs
├── LeaveApplication.cs
├── LoginViewModel.cs
├── RegisterViewModel.cs
└── ...

Views/
Program.cs
WebPage.csproj
```

## Engineering Highlights

- Structured the application using the MVC pattern
- Used Entity Framework Core with MySQL for persistence
- Implemented session-based authentication flow
- Separated complaint and leave logic into dedicated controllers
- Added admin-facing workflows for operational review
- Used view models for login, registration, and related form handling
- Integrated PDF tooling into the application stack

## Getting Started

### Prerequisites

- .NET 8 SDK
- MySQL 8.x
- Visual Studio 2022 or VS Code with C# tooling

### Clone the repository

```bash
git clone https://github.com/bytebysatyam/Summer-Training-Project-THDC-.git
cd Summer-Training-Project-THDC-
```

### Configure the database

Add a local MySQL connection string in your development configuration using the `DefaultConnection` key expected by `Program.cs`.

Example structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=your_database;user=your_user;password=your_password"
  }
}
```

Do not commit real database credentials.

### Restore dependencies

```bash
dotnet restore
```

### Run the application

```bash
dotnet run
```

The default MVC route opens the account login flow.

## Current Limitations

- The project was built for training/learning purposes rather than as a production deployment.
- Session-based authentication is intentionally simple and can be upgraded to a stronger identity/authentication architecture.
- Deployment configuration should be reviewed before production use.
- Organization-specific visuals or sample data should be sanitized before public sharing.

## Future Improvements

- Replace custom session auth with ASP.NET Core Identity
- Add stronger role/permission enforcement
- Add audit logging
- Add automated tests for controllers and services
- Improve error handling and validation
- Add pagination/filtering for admin views
- Containerize the application for reproducible deployment
- Add CI checks for build and tests

## Author

**Satyam Shukla**

Developed as part of industrial training at THDC India Limited.

- GitHub: https://github.com/bytebysatyam
- LinkedIn: https://www.linkedin.com/in/satyamshukla10/
- Portfolio: https://satyam-shukla-portfolio-jet.vercel.app/
- Email: shuklasatyam48890@gmail.com
