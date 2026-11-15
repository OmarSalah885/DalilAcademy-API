# Dalil Academy LMS API

A complete Learning Management System (LMS) backend built with **ASP.NET Core Web API** following **Onion Architecture**.

This project was developed step-by-step through a practical course covering real-world features from basic CRUD to external API integration.

---

## Features

- Full Onion Architecture (Core / Infrastructure / API)
- Entity Framework Core + PostgreSQL (Code First + Migrations)
- DTOs + FluentValidation
- JWT Authentication & Authorization (Roles + Claims)
- Global Exception Handling
- File Upload (Course images)
- External API Integration (Currency conversion using ExchangeRate-API)
- Courses with nested Enrollments (projection – no N+1)
- Clean separation of concerns

---

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL
- FluentValidation
- JWT Bearer Authentication
- BCrypt (password hashing)
- HttpClient (typed client for external APIs)
- Swagger / OpenAPI

---

## Project Structure

```
DalilAcademy/
├── DalilAcademy.API/          → Controllers, Program.cs, Middleware
├── DalilAcademy.Core/         → Entities, DTOs, Interfaces, Validators
└── DalilAcademy.Infra/        → EF Core, Repositories, Services, External API
```

---

## Getting Started

### 1. Clone the repository
```bash
git clone <your-repo-url>
cd DalilAcademy
```

### 2. Update connection string
Open `appsettings.json` and set your PostgreSQL connection string.

### 3. Set User Secrets (important)
```bash
cd DalilAcademy.API
dotnet user-secrets set "ExchangeRate:ApiKey" "YOUR_EXCHANGERATE_API_KEY"
dotnet user-secrets set "Jwt:Key" "YOUR_SUPER_SECRET_JWT_KEY_HERE"
```

### 4. Apply migrations
```bash
dotnet ef database update --project DalilAcademy.Infra --startup-project DalilAcademy.API
```

### 5. Run the API
```bash
dotnet run --project DalilAcademy.API
```

Swagger will open automatically at the root (`https://localhost:xxxx`).

---

## Main Endpoints

| Method | Endpoint                        | Description                        | Auth     |
|--------|---------------------------------|------------------------------------|----------|
| POST   | `/api/auth/register`            | Register new user                  | No       |
| POST   | `/api/auth/login`               | Login and get JWT                  | No       |
| GET    | `/api/courses`                  | Get all courses                    | Optional |
| GET    | `/api/courses/with-enrollments` | Courses + enrolled students        | Yes      |
| POST   | `/api/currency/convert`         | Convert price using live rates     | Optional |
| ...    | Other CRUD endpoints            | Courses, Students, Instructors...  | Yes      |

---

## Notes

- API keys and JWT secrets are stored using **User Secrets** (never committed).
- Course images are saved under `wwwroot/images` (ignored by git).
- The currency conversion uses [ExchangeRate-API](https://www.exchangerate-api.com/).

---

## Course Progress

This repository contains the work up to **Lecture 10** (External APIs + Capstone Task).
