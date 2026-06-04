# Trainee Management API

Hey there! This is a simple, no-nonsense backend Web API built with C# and .NET 10 to help manage and track trainee information. 

I built this project to keep track of trainee profiles, managing everything from basic details to their progress in one place.

---

## 🔥 What it does

* **Manages Profiles:** Built-in endpoints to add, view, update, and delete trainee data.
* **Ready to Test:** Comes with a `.http` file, so you can test the API endpoints directly inside VS Code or Visual Studio without needing Postman.
* **Environment Ready:** Has separate setup files for development and production so nothing breaks when moving to a live server.

---

## 🛠️ Built With

* **Backend:** C# and .NET 10.0
* **Structure:** Clean, standard Controller-based Web API layout.

---

## 🚀 How to run it on your machine

### 1. Prerequisites
You will just need the **.NET 10 SDK** installed on your computer.

### 2. Setup
Open your terminal, clone this project, and move into the folder:
```bash
git clone https://github.com
cd TraineeManagement
```

### 3. Run the App
To start the local development server, run:
```bash
dotnet run --project TraineeManagement.Api.csproj
```

Once it runs, look at your terminal output to find the local URL (usually something like `http://localhost:5000` or `https://localhost:7001`) and open it in your browser or API client to test it out!

---

## 📂 Project Structure at a Glance

* `Controllers/`: Where the API request routes live.
* `Models/`: Where the data shapes and trainee objects are defined.
* `Program.cs`: The main engine file that sets up the whole app.
* `appsettings.json`: Where I store configuration settings.
