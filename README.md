# Trainee Management API

A robust, enterprise-grade ASP.NET Core microservices architecture designed to manage end-to-end trainee training pipelines, assignment allocations, project submissions, and asynchronous event-driven performance evaluations. This application uses a repository-like service architecture, handles complex relational tracking across multiple business modules, implements secure authentication via JWT and BCrypt, and includes comprehensive diagnostic logging using `ILogger`.

---

## 🛠️ Project Name & Technology Stack

* **Project Name:** TraineeManagement
* **Language & Framework:** C# | .NET Core Web API (Microservices Layout)
* **Infrastructure Orchestration:** Docker Compose Multi-Container Stack
* **Database Engine:** MySQL Server via Entity Framework Core (EF Core)
* **Distributed Memory Caching:** Redis Cache Engine
* **Event Broker Architecture:** RabbitMQ Message Broker
* **Authentication:** JWT (JSON Web Tokens) with secure **BCrypt** password hashing
* **Logging Framework:** Microsoft Extensions Logging (`ILogger`)
* **API Documentation:** Swagger / OpenAPI UI

### Container Service Grid Layout Matrix
* **`traineemanagement-api` (Port `5254`)**: Core API layer handling authentication, HTTP requests, and pipeline tracking operations.
* **`traineemanagement-worker`**: Asynchronous background service worker consuming event-driven message packets from broker nodes.
* **`trainingdirectory-api` (Port `5050`)**: Independent data catalog microservice tracking core directories.
* **`local_mysql` (Port `3307` mapped to `3306`)**: Persistent relational database layer running custom database instances.
* **`local_redis` (Port `6380` mapped to `6379`)**: Distributed high-speed memory cache designed to scale database read streams.
* **`local_rabbitmq` (Ports `5672` / `15672`)**: Enterprise message broker routing communication blocks.

---

## ⚡ Backend Setup Steps

### 1. Prerequisites
Ensure you have the following installed on your machine:
* **Docker Desktop** / **Docker Compose Engine CLI**
* **.NET SDK** (Compatible version for local migration compilation)
* **Visual Studio** or **VS Code** (with C# Dev Kit extension)
* **EF Core CLI Tools** (To install, run: `dotnet tool install --global dotnet-ef`)

### 2. Clone and Configure
Open your repository root folder and ensure your project configuration variables are configured inside your local `.env` environment variables template file:

```env
# =======================================================================
# TraineeManagement Local Stack Configuration Template
# INSTRUCTIONS: Copy this file to '.env' and fill in your actual secrets.
# =======================================================================

# --- DATABASE CONFIGURATION ---
MYSQL_ROOT_PASSWORD=your_local_secure_password_here
MYSQL_DATABASE=TraineeManagement

# --- RABBITMQ CONFIGURATION ---
RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest

# --- DOTNET CONFIGURATION ---
ASPNETCORE_ENVIRONMENT=Development

# --- SECURE JWT KEY CONFIGURATION ---
# Must be a long, secure cryptographic string (minimum 16-32 characters)
JWT_KEY=your_secret_jwt_signing_key_here

# --- HOST FILE STORAGE PATH CONFIGURATION ---
FILESTORAGE_UPLOADPATH=Uploads
```
### 3. Launching the Engine
To pull base components, build local source volumes, and boot up your local system web infrastructure pipeline, execute the orchestration engine from your repository root terminal folder:
```bash
# Spin up the complete container architecture in detached background mode
docker compose up -d --build

# Monitor real-time streaming console diagnostics across all microservices
docker compose logs -f
```
Once up and running, open your browser and route to `http://localhost:5254/swagger` to view and test requests via the interactive OpenAPI Swagger UI environment.

---

## 🗄️ MySQL Setup Steps

### 1. Create a MySQL Target Database
The multi-container infrastructure allocates and provisions your isolated relational storage engine automatically based on the variables declared within your `.env` configuration file. No manual storage construction scripts are required.

### 2. Configure Connection String
The API cluster dynamically reads host locations, ports, user profiles, and passwords directly out of your local environmental variable layer. The containerized API pipeline maps internal connection structures seamlessly without modifying static json files.

---

## 🚀 EF Core Migration Commands

Synchronize your application entity designs directly with your live containerized target schema table indexes (accessible locally via redirected loopback port `3307`) by executing the Entity Framework CLI tools in your repository root terminal folder.

### Using the .NET Core CLI (Terminal / Command Prompt)
```bash
# 1. Generate tracking migration scripts for relational structural changes
dotnet ef migrations add AddTaskAssignmentsAndRelations --project TraineeManagement.Api

# 2. Push schema blueprints to your containerized MySQL instance running on Port 3307
dotnet ef database update --project TraineeManagement.Api --connection "Server=localhost;Port=3307;Database=TraineeManagement;User=root;Password=your_local_secure_password_here;"
```

### Using Visual Studio Package Manager Console
```powershell
Add-Migration AddTaskAssignmentsAndRelations -Project TraineeManagement.Api
Update-Database -Project TraineeManagement.Api -ConnectionString "Server=localhost;Port=3307;Database=TraineeManagement;User=root;Password=your_local_secure_password_here;"
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
Copy the value returned within the `token` JSON property payload string.

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

* **Missing In-App Database Seeding:** The architecture relies on external orchestration variables, seeding migration setups, or pre-existing relational entries to populate initial core administrative users.
* **Basic Text/String Filtering Checks:** Core operational parameters like validation for assignment lifecycle changes (`Status`) rely on string matching statements instead of application-wide compiled types.
* **No Database Index Adjustments:** Relational queries (like pulling entries by `TraineeId` or searching for unique `UserName` parameters) perform standard operations without explicit indexing optimizations on frequently hit tables.
* **Lack of Concurrency Control:** Does not map transactional handling blocks or optimization tokens to resolve concurrent state conflicts if multiple microservices alter the same entity properties simultaneously.

---

## 🛡️ Security Checklist

* [x] **Decoupled Application Secrets:** Default configurations are fully extracted from code structures and injected safely using runtime environment arrays via Docker Compose `.env` file orchestration parameters.
* [x] **Enforce Strong Signatures:** Restricts production cryptographic validation steps to highly complex keys requiring minimum 256-bit lengths (>= 32 characters) to bypass key safety vulnerabilities.
* [x] **Transport Layer Redirection:** Pipeline forces active HTTPS/SSL constraints to ensure session tokens are not intercepted over unencrypted communication loops.
* [x] **Data Exposure Minimization:** Implements structural mapping architectures ensuring database storage properties (such as `PasswordHash`) are wrapped strictly inside domain boundaries and never exposed via flat public JSON response schemas.
