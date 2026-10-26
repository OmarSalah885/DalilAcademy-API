# Dalil Academy API

ASP.NET Core Web API built with **Onion Architecture** and **EF Core Code First** using PostgreSQL.

## Solution Structure

- **DalilAcademy.API** → Controllers, Program.cs, appsettings.json
- **DalilAcademy.Core** → Entities + Interfaces
- **DalilAcademy.Infra** → DbContext, Repositories, Services, Configurations, Migrations

## Features

- Onion Architecture + Dependency Injection
- Full CRUD operations for Courses (GET, POST, PUT, DELETE)
- Business rule: Course title must be unique
- Switched from In-Memory Repository to PostgreSQL using EF Core
- Code First Migrations

## Technologies

- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Onion Architecture
- Dependency Injection

## How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Update the connection string in `appsettings.json` with your PostgreSQL password:
   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=DalilAcademyDB;Username=postgres;Password=YOUR_PASSWORD"
   ```
4. Open **Package Manager Console**
   - Set **Default project** = `DalilAcademy.Infra`
5. Run the following command:
   ```powershell
   Update-Database
   ```
6. Set `DalilAcademy.API` as the Startup Project and run it
7. Open Swagger and test the endpoints

## API Endpoints

| Method | Endpoint              | Description               |
|--------|-----------------------|---------------------------|
| GET    | /api/Courses          | Get all courses           |
| GET    | /api/Courses/{id}     | Get course by ID          |
| POST   | /api/Courses          | Create a new course       |
| PUT    | /api/Courses/{id}     | Update an existing course |
| DELETE | /api/Courses/{id}     | Delete a course           |
