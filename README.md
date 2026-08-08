# StudentAPI

A RESTful Web API built with **C# (.NET)** and **SQL Server**, following a clean **3-Tier Architecture**. This project manages student records with a strong focus on database design, validated stored procedures, and proper separation of concerns between layers — built as a hands-on learning project to grow from junior to senior-level backend development practices.

## 🚀 Features

- CRUD operations for student records (Create, Read, Update, Delete)
- Clean 3-Tier Architecture (Presentation, Business Logic, Data Access)
- Shared DTO layer for consistent data contracts across all projects
- Database-level validation using `CHECK` constraints
- Stored procedures with defensive input validation
- Robust error handling using `TRY/CATCH`, `THROW`, and transactions (`XACT_ABORT`)

## 🛠️ Tech Stack

- **Backend:** C#, ASP.NET Core Web API
- **Database:** Microsoft SQL Server
- **Data Access:** ADO.NET
- **Architecture:** 3-Tier (Presentation / Business Logic / Data Access)
- **Tools:** Visual Studio, SQL Server Management Studio (SSMS)

## 📁 Solution Structure
StudentAPI (Solution)
├── SharedDTOModel → Shared DTOs used across all layers (e.g. StudentDTO)
├── StudentAPI → Presentation layer (Controllers, API endpoints)
├── StudentAPIBusinessLayer → Business logic and validation rules
├── StudentDataAccessLayer → Data access via ADO.NET and stored procedures
└── Database/
├── CreateTable.sql
└── StoredProcedures/
├── usp_GetAllStudents.sql
├── usp_GetStudentByID.sql
├── usp_GetPassedStudents.sql
├── usp_GetAverageGrade.sql
└── usp_UpdateStudent.sql

## 🏗️ Architecture Overview

This project follows a classic 3-tier separation:

- **Presentation Layer** (`StudentAPI`) — Exposes REST endpoints, handles HTTP requests/responses.
- **Business Layer** (`StudentAPIBusinessLayer`) — Contains business rules and orchestrates calls to the data layer.
- **Data Access Layer** (`StudentDataAccessLayer`) — Talks directly to SQL Server via ADO.NET and stored procedures.
- **Shared Layer** (`SharedDTOModel`) — Defines DTOs (e.g. `StudentDTO`) used as the common data contract between all layers, avoiding tight coupling between them.

## 🗄️ Database Schema

**Students Table**

| Column     | Type          | Constraint                            |
|------------|---------------|-----------------------------------------|
| StudentId  | INT           | Primary Key, Identity(1,1)              |
| FullName   | NVARCHAR(100) | Not Null, cannot be empty/whitespace    |
| Age        | INT           | Between 12 and 60                       |
| Grade      | INT           | Between 0 and 100                       |

## ⚙️ Stored Procedures

| Procedure               | Description                                        |
|--------------------------|------------------------------------------------------|
| `usp_GetAllStudents`     | Returns all students, ordered by ID                  |
| `usp_GetStudentByID`     | Returns a single student by ID with validation       |
| `usp_GetPassedStudents`  | Returns students with Grade >= 50                    |
| `usp_GetAverageGrade`    | Returns the average grade (OUTPUT parameter)         |
| `usp_UpdateStudent`      | Updates a student with full validation and transaction handling |

## 📌 Status

🚧 **In progress** — Database layer and stored procedures are complete. Currently building out the Data Access, Business, and API layers to connect everything end-to-end.

## 📖 About This Project

This project is part of my journey learning backend development with .NET and SQL Server. It's built step by step using solid engineering practices: parameterized stored procedures, defensive validation at both the database and application level, transaction handling, and a clean layered architecture — the same principles used in professional, production-grade .NET projects.