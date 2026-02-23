# Student Grievance Portal – Database Migration Setup Guide

This project uses **Entity Framework Core (EF Core) migrations** to manage the database.

🚫 **DO NOT create database tables manually**  
🚫 **DO NOT write SQL for schema changes**  

All database structure changes are handled through **code + migrations**.

---

## 1. Core Concept (Read Once)
- Database structure is defined in **C# model classes**
- Changes are tracked using **EF Core migrations**
- Migrations are committed to GitHub
- Each developer has their **own local SQL Server database**
- Database structure remains **identical for everyone**

👉 We share **migration files**, not database files.
---

## 2. Prerequisites (Required for Every Team Member)
- Install the following on your system:

### 2.1 .NET SDK
```bash
    dotnet --version
```



### 2.2 SQL Server

* Install **SQL Server Express**
* Install **SQL Server Management Studio (SSMS)**

---

### 2.3 EF Core CLI Tool (One-Time Setup)
Run once on your system:
```bash
dotnet tool install --global dotnet-ef
```
Verify:
```bash
dotnet ef --version
```
---

## 3. Get the Project Code

### 3.1 Clone the repository (If not cloned already)

```bash
git clone <repository-url>
cd StudentGrievancePortal
```

**OR if already cloned:**
```bash
git pull
```

---

## 4. Database Setup on Your System

### 4.1 Verify connection string
In project root folder find and 
Open `appsettings.json` and ensure this exists else create:
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
        "SenderEmail": "piyushjha.2312@gmail.com",
        "Username": "piyushjha.2312@gmail.com",
        "Password": "please put the password here"
    },
    "ConnectionStrings": {
        "DefaultConnection": "Server=PIYUSH\\SQLEXPRESS01;Database=GrievanceERP;Trusted_Connection=True;TrustServerCertificate=True;"
    } 
}
```
- Everyone will not have the same **server name** and rest string will be same. Replace 'SERVERNAME' with you server name in above string.
- To finds your connection string: 
      -- Open SSMS
      -- Connect to the database
      -- In Object Explorer, right-click the server → View Connection Properties.

Check:
Server name
⚠️ Do NOT change the database name.

---

### 4.2 Create database using migrations

Run this command **from the folder containing the `.csproj` file**:

```bash
dotnet ef database update --context ApplicationDbContext
```

This will:
    * Create the database locally
    * Create all tables
    * Apply relationships and constraints

    ✅ No SQL required
    ✅ No manual database setup

---



## 5. Daily Workflow (IMPORTANT)

Whenever you pull new code from GitHub:

```bash
git pull
dotnet ef database update --context ApplicationDbContext
```
This applies **only new migrations** to your local database.

---

## 6. Team Rules (STRICT)

### ✅ Allowed

* Pull code from GitHub
* Run `dotnet ef database update`
* Use EF Core / LINQ for data access

### ❌ Not Allowed

* Creating tables in SSMS
* Writing `CREATE TABLE` or `ALTER TABLE` SQL
* Running `dotnet ef migrations add`
* Editing migration files manually
* Committing database files (`.mdf`, `.ldf`)

---

## 7. Migration Ownership Rule

* Only **ONE person** (DB Owner) creates migrations
* All other members **only apply migrations**

If you need a database change:
➡️ Inform the DB Owner

---

## 8. How to Verify Database Setup

* Open SSMS
* Connect to SQL Server
* Expand `GrievanceERP`
* Verify tables:
  * Users
  * Roles
  * Departments

---



## 📌 Initial Seed Data


### Insert Users

```sql
INSERT INTO Users (ERP_Id, FullName, Email, PasswordHash, RoleId, DeptId)
VALUES  ('STU001', 'Student_1', 'student@college.edu', '123', 1, 1),
        ('CC001', 'Prof. Singh', 'coordinator@college.edu', '123', 2, 1);
        ('STU002', 'Piyush Jha', 'piyush@bvicam.in', '123', 1, 2);
        ('CC002', 'Dr.Saumya', 'coordinator3@college.edu', '123', 2, 3);
```


## 📌 Sample Queries

```sql
SELECT * FROM Grievances;
SELECT * FROM Users;
SELECT * FROM Departments;
```

---

## ✅ Notes

* `TicketNumber` is auto-generated (e.g., `GRV-1`, `GRV-2`)
* Foreign keys ensure role-based and department-based access
* Designed for integration with **ASP.NET MVC / .NET Core** backend

---

**Project:** Institutional Grievance Redressal System
