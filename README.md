# Trainee Management API

A lightweight, robust ASP.NET Core Web API built with C# for managing trainee records, leveraging an EF Core In-Memory database provider for local execution.

---

## 🛠️ Technology Stack
* **Language & Framework:** C# / .NET 10 SDK
* **Database Provider:** Entity Framework Core (In-Memory Database)
* **API Documentation:** Swagger / OpenAPI

---

## 📂 Project Architecture

```text
TraineeManagement/
│
├── Controllers/       # API endpoints, request routing, and HTTP status code maps
├── Services/          # Centralized business logic and operational service layer
├── Models/            # Core data shapes and domain-driven entities
├── DTOs/              # Data Transfer Objects filtering client input/response shapes
├── Data/              # ApplicationDbContext setup and persistent configurations
├── Program.cs         # Main application bootstrapper and dependency injection container
└── appsettings.json   # Global environment runtime configurations
```

---

## 🚀 Features & Endpoints

### 🩺 Health Diagnostics
* `GET` `/health` - Service operational check
* `GET` `/api/health` - Internal system diagnostics check

### 👤 Trainee Operations
* `GET` `/api/trainees` - Retrieve all trainees
* `GET` `/api/trainees/search?search={query}` - Filter trainees by matching text
* `GET` `/api/trainees/{id}` - Retrieve a specific trainee by primary key ID
* `POST` `/api/trainees` - Register a new trainee profile
* `PUT` `/api/trainees/{id}` - Modify details of an active trainee profile
* `DELETE` `/api/trainees/{id}` - Remove a trainee from the database

---

## 📋 API Contracts & Payloads

### 1. Get All or Search Trainees
* **Route:** `GET /api/trainees` OR `GET /api/trainees/search?search=John`
* **Request Body:** None

#### Response Body (`200 OK` - `List<TraineeResponse>`)
```json
[
  {
    "id": 101,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "department": "Engineering",
    "joiningDate": "2026-06-01T00:00:00Z",
    "status": "Active"
  },
  {
    "id": 102,
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@example.com",
    "department": "Product",
    "joiningDate": "2026-06-05T00:00:00Z",
    "status": "Active"
  }
]
```

---

### 2. Get Trainee By ID
* **Route:** `GET /api/trainees/{id}` (e.g., `/api/trainees/101`)
* **Request Body:** None

#### Response Body (`200 OK` - `TraineeResponse`)
```json
{
  "id": 101,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "department": "Engineering",
  "joiningDate": "2026-06-01T00:00:00Z",
  "status": "Active"
}
```

---

### 3. Create Trainee
* **Route:** `POST /api/trainees`

#### Request Body (`CreateTraineeRequest`)
```json
{
  "firstName": "Alex",
  "lastName": "Jones",
  "email": "alex.jones@example.com",
  "department": "Quality Assurance",
  "joiningDate": "2026-06-15T09:00:00Z"
}
```

#### Response Body (`200 OK` - `TraineeResponse`)
```json
{
  "id": 103,
  "firstName": "Alex",
  "lastName": "Jones",
  "email": "alex.jones@example.com",
  "department": "Quality Assurance",
  "joiningDate": "2026-06-15T09:00:00Z",
  "status": "Onboarding"
}
```

---

### 4. Update Trainee
* **Route:** `PUT /api/trainees/{id}`

#### Request Body (`UpdateTraineeRequest`)
```json
{
  "firstName": "Johnathan",
  "lastName": "Doe",
  "department": "DevOps",
  "status": "Completed"
}
```

#### Response Body (`200 OK` - `TraineeResponse`)
```json
{
  "id": 101,
  "firstName": "Johnathan",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "department": "DevOps",
  "joiningDate": "2026-06-01T00:00:00Z",
  "status": "Completed"
}
```

---

### 5. Delete Trainee
* **Route:** `DELETE /api/trainees/{id}`
* **Request Body:** None
* **Response Status Codes:** Returns a blank `200 OK` on successful removal, or a `404 Not Found` if the record does not exist.

---

## 🏃 Getting Started & How to Run

### 1. Prerequisites
Ensure you have the latest **.NET 10 SDK** installed on your system.

### 2. Navigation
Open your terminal and step directly into the project directory:
```bash
cd TraineeManagement
```

### 3. Execution
Launch the local Kestrel Web Server targeting the API application:
```bash
dotnet run --project TraineeManagement.Api.csproj
```

Once running, navigate to the local hosting address printed in your terminal (typically `http://localhost:5000/swagger` or similar) to interactively trace requests via the Swagger UI.

---

## ⚠️ Known Limitations

* **Basic Text Searching:** The search implementation handles raw, single-parameter text matching. Advanced filtering criteria (such as status sorting or dynamic date boundaries) are currently unavailable.
* **Lack of Data Pagination:** Endpoints yield all query results simultaneously inside a single payload. High volumes of records may cause processing performance degradation.
* **Volatile Persistence Constraints:** Uses an EF Core In-Memory provider context; stopping or restarting the runtime engine execution processes automatically destroys all modified data records.
* **Raw Exception Management:** The API lacks specialized global Exception Filter Middleware. Uncaught system exceptions can propagate backend traces directly to API consumers.
