# Trainee Management API

## 📂 Project Structure at a Glance

* `Controllers/`: Where the API request routes live.
* `Services/`: contains actual buisness logic .
* `Models/`: Where the data shapes and trainee objects are defined.
* `DTOs/`: contains Data tranfer objects here instead using actual model we can define what properties of model will require from client and what response can also be shared.
* `Data/`: contains dbcontext .
* `Program.cs`: The main engine file that sets up the whole app.
* `appsettings.json`: Where configuration settings are stored.

# Created
TraineeManagement application
 
## Technology Used
c# , asp.net web api, swagger
## Features Completed
- /GET/health
- GET /api/health
- GET /api/trainees/Search?=
- GET /api/trainees/{id}
- POST /api/trainees
- PUT /api/trainees/{id}
- DELETE /api/trainees/{id}

 
1. GET /api/trainees or GET /api/trainees/search?search=JohnRequest Body: None.Sample Response JSON (List<TraineeResponse>):json[
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
Use code with caution.

2. GET /api/trainees/{id}Request Body: None (ID passed via URL path, e.g., /api/trainees/101).Sample Response JSON (TraineeResponse):json{
  "id": 101,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "department": "Engineering",
  "joiningDate": "2026-06-01T00:00:00Z",
  "status": "Active"
}
Use code with caution.

3. POST /api/traineesSample Request JSON (CreateTraineeRequest):json{
  "firstName": "Alex",
  "lastName": "Jones",
  "email": "alex.jones@example.com",
  "department": "Quality Assurance",
  "joiningDate": "2026-06-15T09:00:00Z"
}
Use code with caution.Sample Response JSON (TraineeResponse via Ok status):json{
  "id": 103,
  "firstName": "Alex",
  "lastName": "Jones",
  "email": "alex.jones@example.com",
  "department": "Quality Assurance",
  "joiningDate": "2026-06-15T09:00:00Z",
  "status": "Onboarding"
}
Use code with caution.

4. PUT /api/trainees/{id }Sample Request JSON (UpdateTraineeRequest):json{
  "firstName": "Johnathan",
  "lastName": "Doe",
  "department": "DevOps",
  "status": "Completed"
}
Use code with caution.Sample Response JSON (TraineeResponse):json{
  "id": 101,
  "firstName": "Johnathan",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "department": "DevOps",
  "joiningDate": "2026-06-01T00:00:00Z",
  "status": "Completed"
}
Use code with caution.

5. DELETE /api/trainees/{id} Request Body: None.Response Body: None (Returns a clean HTTP status code 200 OK or 404 Not Found).

## How to Run
1. Prerequisites
You will just need the **.NET 10 SDK** installed on your computer.
 
2. Navigation
move into the folder: cd TraineeManagement
 
3. Run the App
To start the local development server, run: dotnet run --project TraineeManagement.Api.csproj
 
## Limitations
- Basic Text Searching Only: The search endpoint filters by a single query parameter string. It lacks advanced filter variables (e.g., date ranges, department filters, status sorting).
- Missing Pagination: The endpoints fetch all results at once. Large datasets will degrade application performance without page and pageSize limiters.In-Memory or Local Scope Routing:
- There are no global exception-handling middleware filters. Unexpected database or service errors will expose raw backend stack traces to clients.