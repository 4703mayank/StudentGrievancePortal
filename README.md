# GrievanceERP Database Setup

This document explains the **database schema and setup** for the **Student Grievance Management System (GrievanceERP)** built using **MS SQL Server**. It includes database creation, table structures, relationships, and sample seed data.

---

## 📌 Database Creation

```sql
CREATE DATABASE GrievanceERP;
GO

USE GrievanceERP;
GO
```

---

## 📌 Tables Overview

### 1️⃣ Departments Table

Stores department information.

```sql
CREATE TABLE Departments (
    DeptId INT PRIMARY KEY IDENTITY(1,1),
    DeptName NVARCHAR(100) NOT NULL
);
```

---

### 2️⃣ Roles Table

Defines system roles.

```sql
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL
);
```

**Roles Used:**

* Student
* Coordinator
* Admin

---

### 3️⃣ Users Table

Stores login and profile details of users.

```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    ERP_Id NVARCHAR(50) UNIQUE NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    RoleId INT FOREIGN KEY REFERENCES Roles(RoleId),
    DeptId INT FOREIGN KEY REFERENCES Departments(DeptId),
    CreatedDate DATETIME DEFAULT GETDATE()
);
```

---

### 4️⃣ Grievances Table

Stores grievance details submitted by students.

```sql
CREATE TABLE Grievances (
    GrievanceId INT PRIMARY KEY IDENTITY(1,1),
    TicketNumber AS ('GRV-' + CAST(GrievanceId AS NVARCHAR(10))),
    Subject NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Submitted',
    Priority NVARCHAR(20) DEFAULT 'Medium',
    StudentId INT FOREIGN KEY REFERENCES Users(UserId),
    AssignedDeptId INT FOREIGN KEY REFERENCES Departments(DeptId),
    ResolutionDetails NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);
```

---

## 📌 Initial Seed Data

### Insert Roles

```sql
INSERT INTO Roles (RoleName)
VALUES ('Student'), ('Coordinator'), ('Admin');
```

### Insert Departments

```sql
INSERT INTO Departments (DeptName)
VALUES ('Computer Science'), ('Mechanical'), ('Civil'), ('Accounts');
```

### Insert Users

```sql
INSERT INTO Users (ERP_Id, FullName, Email, PasswordHash, RoleId, DeptId)
VALUES ('STU001', 'John Doe', 'student@college.edu', '123', 1, 1),
       ('CC001', 'Prof. Smith', 'coordinator@college.edu', '123', 2, 1);
```

```sql
INSERT INTO Users (ERP_Id, FullName, Email, PasswordHash, RoleId, DeptId)
VALUES ('CC003', 'Dr.Saumya', 'coordinator3@college.edu', '123', 2, 3);
```

```sql
INSERT INTO Users (ERP_Id, FullName, Email, PasswordHash, RoleId, DeptId)
VALUES ('STU002', 'Piyush Jha', 'piyush@bvicam.in', '123', 1, 2);
```

---

## 📌 Data Updates

### Update Department Name

```sql
UPDATE Departments
SET DeptName = 'ECE'
WHERE DeptId = 4;
```

### Update User Name

```sql
UPDATE Users
SET FullName = 'Prof. Sunil'
WHERE ERP_Id = 'CC001';
```

---

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

**Author:** Piyush Jha
**Project:** Institutional Grievance Redressal System
