# Trainee Management API

A robust, enterprise-grade ASP.NET Core Web API designed to manage end-to-end trainee training pipelines, assignment allocations, project submissions, and performance evaluations. This application uses a repository-like service architecture, handles complex relational tracking across multiple business modules, implements secure authentication via JWT and BCrypt, and includes comprehensive diagnostic logging using `ILogger`.

---

## 🛠️ Project Name & Technology Stack

* **Project Name:** TraineeManagement
* **Language & Framework:** C# | .NET Core Web API
* **Database Engine:** MySQL Server via Entity Framework Core (EF Core)
* **Authentication:** JWT (JSON Web Tokens) with secure **BCrypt** password hashing
* **Logging Framework:** Microsoft Extensions Logging (`ILogger`)
* **API Documentation:** Swagger / OpenAPI UI

---

## ⚡ Backend Setup Steps

### 1. Prerequisites
Ensure you have the following installed on your machine:
* **.NET SDK** (Compatible version for your project setup)
* **Visual Studio** or **VS Code** (with C# Dev Kit extension)
* **EF Core CLI Tools** (To install, run: `dotnet tool install --global dotnet-ef`)

### 2. Clone and Configure
Open your repository root and navigate to the project configuration template file (`appsettings.json`). Update your private cryptographic signing parameters:
```json
{
  "jwt": {
    "Key": "YourSuperSecretLongAndSecureKeyMustBeAtLeast32BytesLong!!",
    "Issuer": "TraineeManagementApi",
    "Audience": "TraineeManagementClients",
    "ExpiryMinutes": "60"
  }
}
```

### 3. Launching the Engine
To restore dependencies, build, and boot your local system web infrastructure pipeline:
```bash
dotnet run
```
Once up and running, open your browser and route to `http://localhost:5000/swagger` (or your console's indicated SSL loopback address) to view and test requests via the interactive OpenAPI Swagger UI environment.

---

## 🗄️ MySQL Setup Steps

### 1. Create a MySQL Target Database
Open your preferred database management tool (e.g., MySQL Workbench, DBeaver, or command line interface) and run the following statement to allocate a dedicated tracking container:
```sql
CREATE DATABASE traineemanagement;
```

### 2. Configure Connection String
Update the connection string inside your root `appsettings.json` file with your database server location, administrative user name, and account password:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=traineemanagement;User=root;Password=YOUR_PASSWORD_HERE;"
  }
}
```

---

## 🚀 EF Core Migration Commands

Synchronize your application entity designs directly with your live target schema table indexes by executing the Entity Framework CLI tools in your repository root terminal folder.

### Using the .NET Core CLI (Terminal / Command Prompt)
```bash
# 1. Generate tracking migration scripts for relational structural changes
dotnet ef migrations add AddTaskAssignmentsAndRelations

# 2. Push schema blueprints to your MySQL instance to build out database structures
dotnet ef database update
```

### Using Visual Studio Package Manager Console
```powershell
Add-Migration AddTaskAssignmentsAndRelations
Update-Database
```

---

## 🔐 Login Credentials for Testing

The following default user identities can be injected into your application database initialization logic (or added via custom data seeding scripts) to evaluate relational security features:

| Role | Username | Password (Plain Text) | BCrypt Stored Hash (Example) |
| :--- | :--- | :--- | :--- |
| **Administrator** | `Admin` | `Admin@123` | `$2a$11$e7...` (Auto-generated on creation) |

---

## 🎫 JWT Usage Instructions

To safely query secure application resources, clients must complete a standard login flow and include the generated cryptographic credentials in subsequent HTTP queries.

### Step 1: Request Token
Submit user account credentials via the public authentication portal (`POST /api/auth/login`).

### Step 2: Extract Token
Copy the value returned within the `Token` JSON property payload string.

### Step 3: Configure Authorization Header
Include this identity token inside the header configuration block of all subsequent secure requests using the standard `Bearer` scheme pattern:
```text
Authorization: Bearer <YOUR_JWT_TOKEN_STRING_HERE>
```

*(If testing through Swagger UI, click the **Authorize** lock button, input `Bearer <token>` exactly into the form box field, and click authorize).*

---

## 📋 API List & Sample JSON Requests/Responses

### 1. Authentication (`POST /api/auth/login`)
* **Description:** Validates user login credentials and returns a secure JWT bearer token.

#### Sample Request Payload
```json
{
  "username": "mentor_yash",
  "password": "MentorSecret99!"
}
```

#### Sample Response Payload (Status 200 OK)
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwi...",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "userName": "mentor_yash",
    "role": "Mentor"
  }
}
```

---

### 2. Task Allocation (`POST /api/task-assignments`)
* **Description:** Pairs an existing trainee with a mentor and a learning task. Runs automated date check safeguards.

#### Sample Request Payload
```json
{
  "traineeId": 5,
  "mentorId": 2,
  "learningTaskId": 14,
  "status": "Assigned"
}
```

#### Sample Response Payload (Status 200 OK)
```json
{
  "id": 42,
  "traineeId": 5,
  "traineeName": "John Doe",
  "mentorId": 2,
  "mentorName": "Yash Gandhi",
  "learningTaskId": 14,
  "taskTitle": "Build a REST API",
  "status": "Assigned",
  "assignedDate": "2026-06-15T19:15:00",
  "dueDate": "2026-06-30T23:59:59"
}
```

---

### 3. Update Assignment Status (`PUT /api/task-assignments/{id}/{status}`)
* **Description:** Safely alters the status tracker parameter across standard system milestones.
* **URL Example:** `/api/task-assignments/42/InProgress`

#### Sample Response Payload (Status 200 OK)
```json
{
  "id": 42,
  "traineeId": 5,
  "traineeName": "John Doe",
  "mentorId": 2,
  "mentorName": "Yash Gandhi",
  "learningTaskId": 14,
  "taskTitle": "Build a REST API",
  "status": "InProgress",
  "assignedDate": "2026-06-15T19:15:00",
  "dueDate": "2026-06-30T23:59:59"
}
```

---

### 4. Code Solution Deliverables (`POST /api/submissions`)
* **Description:** Allows trainees to attach external code solution hyperlinks against an open assignment.

#### Sample Request Payload
```json
{
  "taskAssignmentId": 42,
  "status": "Submitted",
  "notes": "Completed the challenge along with additional unit testing suites.",
  "submissionUrl": "https://github.com"
}
```

#### Sample Response Payload (Status 200 OK)
```json
{
  "id": 101,
  "status": "Submitted",
  "notes": "Completed the challenge along with additional unit testing suites.",
  "submissionUrl": "https://github.com",
  "submittedDate": "2026-06-18T14:30:00",
  "taskAssignmentId": 42
}
```

---

### 5. Evaluation Matrix (`POST /api/reviews`)
* **Description:** Empowers assigned mentors to grade student submissions, track evaluations, and return feedback.

#### Sample Request Payload
```json
{
  "submissionId": 101,
  "reviewStatus": "Reviewed",
  "score": 95,
  "feedback": "Excellent clean code structure. Great work on testing coverage!"
}
```

#### Sample Response Payload (Status 200 OK)
```json
{
  "id": 201,
  "submissionId": 101,
  "mentorId": 2,
  "mentorName": "Yash Gandhi",
  "feedback": "Excellent clean code structure. Great work on testing coverage!",
  "score": 95,
  "reviewStatus": "Reviewed",
  "reviewedDate": "2026-06-19T09:15:00"
}
```

---

## ⚠️ Known Limitations

* **Missing In-App Database Seeding:** The architecture relies on external tools or pre-existing relational entries to populate initial core administrative users.
* **Basic Text/String Filtering Checks:** Core operational parameters like validation for assignment lifecycle changes (`Status`) rely on string matching statements instead of application-wide compiled types.
* **No Database Index Adjustments:** Relational queries (like pulling entries by `TraineeId` or searching for unique `UserName` parameters) perform standard operations without explicit indexing optimizations on frequently hit tables.
* **Lack of Concurrency Control:** Does not map transactional handling blocks or optimization tokens to resolve concurrent state conflicts if multiple services alter the same entity properties simultaneously.

---

## 🛡️ Security Checklist

* [ ] Change default app configuration secrets and enforce strong secret signature constraints (minimum 256-bit keys) across production environment configs.
* [ ] Enforce HTTPS rules across transport layers to guard session data during transfer.
* [ ] Review structural mapping architectures to make sure database tracking fields (such as `PasswordHash`) are strictly contained inside internal layers and never leaked via flat response entities.
