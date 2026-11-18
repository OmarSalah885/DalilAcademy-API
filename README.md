# Dalil Academy LMS API
 
A Learning Management System (LMS) backend built with **ASP.NET Core Web API** following **Onion Architecture**.
Students enroll in courses and pay for them through **PayPal Sandbox** (no real money moves).
 
---
 
## Features
 
- Onion Architecture (Core / Infrastructure / API)
- Entity Framework Core + PostgreSQL (Code First + Migrations)
- DTOs + FluentValidation
- JWT Authentication & Authorization (roles: Admin / Instructor / Student)
- Global exception handling with ProblemDetails
- Course image upload
- Currency conversion using ExchangeRate-API
- Courses with nested enrollments (projection, no N+1)
- **Enrollment payments with PayPal Sandbox** (Pending → Paid)
- **Bonus:** course capacity, course search, payment history
---
 
## Tech Stack
 
- ASP.NET Core 8
- Entity Framework Core + Npgsql (PostgreSQL)
- FluentValidation
- JWT Bearer Authentication + BCrypt
- Typed `HttpClient` (ExchangeRate-API and PayPal Orders API v2)
- Swagger / OpenAPI
---
 
## Project Structure
 
```
DalilAcademy/
├── DalilAcademy.API/     → Controllers, Program.cs, Middleware
├── DalilAcademy.Core/    → Entities, DTOs, Interfaces, Validators, Exceptions
└── DalilAcademy.Infra/   → EF Core, Repositories, Services, PayPal gateway
```
 
---
 
## Getting Started
 
### 1. Requirements
 
- .NET 8 SDK
- PostgreSQL
- A PayPal Developer account with a **Sandbox** app (Client ID and Secret) and the sandbox
  personal (buyer) and business (seller) accounts
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`
### 2. Configure non-secret settings
 
`DalilAcademy.API/appsettings.json` contains only non-secret values. Check these:
 
```json
"PayPal": {
  "BaseUrl": "https://api-m.sandbox.paypal.com",
  "ReturnUrl": "https://localhost:7085/index.html",
  "CancelUrl": "https://localhost:7085/index.html",
  "JodToUsdRate": 1.41
}
```
 
Change the port in `ReturnUrl` / `CancelUrl` if your app runs on a different one.
 
### 3. Set User Secrets
 
All secrets live in User Secrets and are **not** in the repository or the zip.
 
```bash
cd DalilAcademy.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=DalilAcademyDB;Username=postgres;Password=YOUR_DB_PASSWORD"
dotnet user-secrets set "Jwt:Key" "YOUR_LONG_RANDOM_JWT_KEY_AT_LEAST_32_CHARS"
dotnet user-secrets set "ExchangeRate:ApiKey" "YOUR_EXCHANGERATE_API_KEY"
dotnet user-secrets set "PayPal:ClientId" "YOUR_PAYPAL_SANDBOX_CLIENT_ID"
dotnet user-secrets set "PayPal:ClientSecret" "YOUR_PAYPAL_SANDBOX_SECRET"
```
 
### 4. Apply migrations
 
From the solution folder:
 
```bash
dotnet ef database update --project DalilAcademy.Infra --startup-project DalilAcademy.API
```
 
Or in the Visual Studio Package Manager Console (startup project `DalilAcademy.API`):
 
```powershell
Update-Database -Project DalilAcademy.Infra -StartupProject DalilAcademy.API
```
 
### 5. Run the API
 
```bash
dotnet run --project DalilAcademy.API
```
 
Open Swagger in the browser (the port is shown in the console).
 
---
 
## Payment Flow (test from Swagger)
 
1. Log in as **Admin** and authorize Swagger with the JWT.
2. `POST /api/enrollments` for a paid course. The enrollment is created as `Pending`
   (a course with price 0 is created as `Paid` directly).
3. `POST /api/enrollments/{id}/pay`. Copy `approvalUrl` from the response.
4. Open `approvalUrl` in the browser, log in with the **sandbox buyer** account and approve.
   PayPal sends you back to the return URL.
5. `POST /api/enrollments/{id}/capture`. The payment becomes `Completed` (with `PaidAt`)
   and the enrollment becomes `Paid`.
The amount is calculated on the server: `course price (JOD) × JodToUsdRate`, rounded to 2 decimals.
A PayPal order must be captured within 3 hours of creation, otherwise call `/pay` again.
 
---
 
## Endpoints
 
### Auth
 
| Method | Endpoint             | Description       | Auth |
|--------|----------------------|-------------------|------|
| POST   | `/api/auth/register` | Register new user | No   |
| POST   | `/api/auth/login`    | Login and get JWT | No   |
 
### Courses
 
| Method | Endpoint                         | Description                                                       | Auth  |
|--------|----------------------------------|-------------------------------------------------------------------|-------|
| GET    | `/api/courses`                   | Paged list. Query: `page`, `pageSize`, `minPrice`, `maxPrice`, **`search`** (title, case-insensitive) | No |
| GET    | `/api/courses/count`             | Count with the same filters (`minPrice`, `maxPrice`, `search`)    | No    |
| GET    | `/api/courses/{id}`              | Course by id                                                      | No    |
| POST   | `/api/courses`                   | Create course (optional `maxStudents`)                            | Admin |
| PUT    | `/api/courses/{id}`              | Update course                                                     | Admin / Instructor |
| DELETE | `/api/courses/{id}`              | Delete course                                                     | Admin |
| GET    | `/api/courses/with-enrollments`  | Courses with enrolled students                                    | Yes   |
| GET    | **`/api/courses/{courseId}/students`** | Paid students of a course (404 for unknown course)          | Admin |
 
### Enrollments and payments (new)
 
| Method | Endpoint                             | Description                                                    | Auth  |
|--------|--------------------------------------|----------------------------------------------------------------|-------|
| POST   | `/api/enrollments`                   | Create enrollment (`Pending`, or `Paid` if price is 0). 409 if duplicate or course is full | Admin |
| PUT    | `/api/enrollments/{id}`              | Move to another course. 409 if the enrollment is `Paid`        | Admin |
| DELETE | `/api/enrollments/{id}`              | Delete. 409 if the enrollment is `Paid`                        | Admin |
| GET    | `/api/enrollments/me`                | The student's `Paid` courses only                              | Student |
| POST   | `/api/enrollments/{id}/pay`          | Create a PayPal order and a `Created` payment. Returns `approvalUrl` | Admin |
| POST   | `/api/enrollments/{id}/capture`      | Capture the latest `Created` payment. Enrollment becomes `Paid` | Admin |
| GET    | `/api/enrollments/{id}/payments`     | Payment history, newest first (bonus)                          | Admin |
 
Other CRUD endpoints (Students, Instructors) are unchanged. `POST /api/currency/convert` converts a price using live rates.
 
### Error responses for payments
 
| Situation                                        | Status |
|--------------------------------------------------|--------|
| Unknown enrollment                               | 404    |
| `/pay` or `/capture` on an already `Paid` enrollment | 409 |
| `/capture` without a `Created` payment           | 400 "Call /pay first." |
| Capture failed or order not approved yet         | 400 (payment becomes `Failed`, enrollment stays `Pending`) |
| PayPal unreachable                               | 503    |
 
---
 
## Bonus Features
 
- **Course capacity:** optional `maxStudents` when creating or updating a course (empty means
  unlimited). Only `Paid` enrollments count toward the limit. A full course returns **409** when:
  - creating an enrollment (`POST /api/enrollments`),
  - moving a `Pending` enrollment to it (`PUT /api/enrollments/{id}`),
  - calling `/pay` or `/capture` for an enrollment of that course (checked before PayPal is
    called, so nobody is charged for a full course).
- **Course search:** `GET /api/courses?search=net&page=1&pageSize=10`.
- **Payment history:** `GET /api/enrollments/{id}/payments`.
---
 
## Notes
 
- Secrets (DB password, JWT key, ExchangeRate key, PayPal Client ID and Secret) are stored in
  **User Secrets** and never committed.
- Course images are saved under `wwwroot/images` (ignored by git).
- PayPal runs in **Sandbox** mode only. Prices are in JOD and converted to USD with a fixed rate
  from `appsettings.json`.
 