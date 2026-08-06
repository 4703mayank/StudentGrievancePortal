<div align="center">

# 🎓 BVICAM Student Grievance Portal

**A secure, transparent, ERP-ready web portal for submitting, tracking, and resolving student grievances.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?style=flat&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat&logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-Code--First-blue?style=flat)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=flat&logo=microsoftsqlserver)](https://www.microsoft.com/en-in/sql-server)
[![Bootstrap](https://img.shields.io/badge/UI-Bootstrap%205-7952B3?style=flat&logo=bootstrap)](https://getbootstrap.com/)
[![Gemini API](https://img.shields.io/badge/AI%20Chatbot-Google%20Gemini-4285F4?style=flat&logo=google)](https://ai.google.dev/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat)](LICENSE.txt)

</div>

---

## 📖 Overview

The **Student Grievance Portal** is a full-stack, web-based grievance management system built for an academic institution (BVICAM, New Delhi). It replaces slow, untracked, paper/email-based complaint handling with a **secure, role-based, digitally auditable** workflow.

Students can submit grievances (including **anonymously**), track their status in real time, and get notified by email as their case progresses. Class Coordinators get a dedicated dashboard to review, prioritize, resolve, and report on grievances across departments — without ever seeing the identity of an anonymous complainant.

The system was designed to plug into an institution's existing **ERP** for authentication, and ships with an **AI chatbot** (powered by the Google Gemini API) that answers portal-related queries only.

> 📄 A detailed dissertation covering the system design, database schema, UML diagrams, and test reports for this project is available on request / in the academic submission.

---

## ✨ Features

- 🔐 **Role-based authentication** — separate, restricted dashboards for Students and Class Coordinators
- 📝 **Grievance submission** — structured form with subject, description, and department targeting
- 🕵️ **Anonymous grievance handling** — coordinator never sees the identity of an anonymous complainant
- 🚦 **Automated priority suggestion** — grievances auto-tagged as High / Moderate / Low / Default
- 📊 **Real-time status tracking** — Submitted → Under Review → Resolved, visible to the student instantly
- 🗂️ **Department Summary Reports** — filterable grievance statistics with **CSV export** and **printable table view**
- 📧 **Email notifications** — automated SMTP (Gmail) alerts to students on every status change
- 🤖 **AI-powered chatbot** — Google Gemini–backed assistant restricted to portal-related queries
- 🧱 **Code-first database** — schema fully managed and versioned through EF Core migrations (no manual SQL)
- 📱 **Responsive UI** — Bootstrap 5 + jQuery interface that works across modern browsers

---

## 🏗️ Tech Stack

| Layer            | Technology                                                        |
|-------------------|---------------------------------------------------------------------|
| **Backend**       | ASP.NET Core (.NET 8), C# 12, MVC architecture, Razor Views          |
| **Database**      | Microsoft SQL Server (SQL Express), Entity Framework Core (code-first, migrations) |
| **Frontend**      | Bootstrap 5, jQuery 3.6                                              |
| **Email**         | SMTP (Gmail) via `System.Net.Mail`                                   |
| **AI Chatbot**    | Google Gemini API                                                    |
| **Tooling**       | Visual Studio 2022, SQL Server Management Studio (SSMS), NuGet, Git/GitHub |

---

## 🧩 System Architecture

The application follows a classic **MVC** flow with EF Core as the ORM layer:

```
Browser (Bootstrap + jQuery)
        │  HTTP request
        ▼
ASP.NET Core Controllers  ──►  Business Logic (auth, grievance workflow, priority engine)
        │                                   │
        ▼                                   ▼
   Razor Views                    Entity Framework Core (ORM)
                                            │
                                            ▼
                                    SQL Server Database
```

**Core entities:** `Users` · `Roles` · `Departments` · `Grievances`

- A **User** submits many **Grievances** (1:N)
- A **User** belongs to one **Department** (N:1)
- A **Role** (Student / Class Coordinator) has many **Users** (1:N)
- A **Grievance** is assigned to one **Department** (N:1)

Authentication currently uses independent institutional credentials and is **designed to support future integration** with the institution's live ERP system.

---

## 👥 User Roles

| Role | Capabilities |
|------|--------------|
| **Student** | Secure login · submit grievances (incl. anonymous) · real-time status tracking · view grievance history |
| **Class Coordinator** | Review assigned grievances · update status & add resolution remarks · view department summary reports · export CSV/print reports — **without seeing anonymous complainant identity** |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server Express + [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (one-time):

  ```bash
  dotnet tool install --global dotnet-ef
  dotnet ef --version
  ```

### 1. Clone the repository

```bash
git clone https://github.com/4703mayank/StudentGrievancePortal.git
cd StudentGrievancePortal
```

### 2. Configure `appsettings.json`

Create/update `appsettings.json` in the project root:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "BVICAM Grievance Portal",
    "SenderEmail": "your-email@gmail.com",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=GrievanceERP;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Replace `YOUR_SERVER_NAME` with your local SQL Server instance name (find it in SSMS → right-click server → *View Connection Properties*). Do **not** rename the database.

### 3. Apply database migrations

The database schema is entirely code-first — **no manual SQL required**.

```bash
dotnet ef database update --context ApplicationDbContext
```

This creates the database, all tables, and their relationships/constraints locally.

### 4. Run the application

```bash
dotnet run
```

Then open the URL shown in the console (e.g. `https://localhost:7101`).

### Daily workflow (for contributors)

```bash
git pull
dotnet ef database update --context ApplicationDbContext
```

---

## 🗃️ Database Schema

| Table | Description |
|---|---|
| `Users` | Student & Coordinator accounts (ERP ID, name, email, password hash, role, department) |
| `Roles` | `Student` / `Class Coordinator` role definitions |
| `Departments` | Institutional departments grievances can be routed to |
| `Grievances` | Ticket number, subject, description, status, priority, resolution details, timestamps |

All tables and relationships are defined via **C# model classes** and versioned through **EF Core migrations** — schema changes are made in code, never by hand in SSMS.

---

## 📁 Project Structure

```
StudentGrievancePortal/
├── StudentGrievancePortal/     # Main ASP.NET Core MVC project
│   ├── Controllers/            # Request handling & business logic
│   ├── Models/                 # EF Core entity models
│   ├── Views/                  # Razor views (Student, Coordinator, Shared)
│   ├── Migrations/             # EF Core code-first migrations
│   └── wwwroot/                # Static assets (CSS/JS/Bootstrap)
├── StudentGrievancePortal.sln  # Visual Studio solution file
├── LICENSE.txt                 # MIT License
└── README.md
```

---

## 🧪 Testing

The system was validated through unit, integration, functional, database, security, UI, email, and chatbot testing — covering login flows, anonymous submission, role-based access restrictions, status transitions, report exports, and Gemini chatbot query scoping.

---

## 🔭 Roadmap / Future Scope

- [ ] Live integration with institutional ERP for authentication & data sync
- [ ] File/document attachment support on grievance submission
- [ ] Admin panel for managing users, departments & configuration
- [ ] Mobile app (Android/iOS)
- [ ] SMS notifications alongside email
- [ ] Two-factor authentication (2FA)
- [ ] ML-based grievance categorisation & priority prediction
- [ ] Migration to a cloud-hosted database (e.g. Azure SQL) for production

---

## 🤝 Contributing

This started as an academic team project. Issues and pull requests are welcome:

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes
4. Open a pull request

Please **do not** run `dotnet ef migrations add` or hand-edit migration files without coordinating — the database schema is centrally managed through EF Core migrations.

---

## 📄 License

Distributed under the **MIT License**. See [`LICENSE.txt`](LICENSE.txt) for details.

---

## 👤 Author

**Mayank Sharma** 
- [GitHub](https://github.com/4703mayank)
- [LinkedIn](https://www.linkedin.com/in/mayank-sharma-1b3ba0283/)
