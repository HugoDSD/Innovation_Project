# Setup Guide

> Prerequisites, configuration, database setup, and how to run the app.

## Prerequisites

- [Node.js](https://nodejs.org/) ≥ 22.18.0
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [PostgreSQL 17](https://www.postgresql.org/)

## 1. Backend configuration

`appsettings.json` and `launchSettings.json` are excluded from git for security. Request them from the team, then place them at:

```
backend/appsettings.json
backend/Properties/launchSettings.json
```

`appsettings.json` must contain:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=<db_name>;Username=<user>;Password=<password>"
  },
  "JwtSettings": {
    "Secret": "<your-secret-key>",
    "ExpiryInHours": 2
  }
}
```

## 2. Database

Create the PostgreSQL database, then apply migrations:

```bash
cd backend
dotnet ef database update
```

Alternatively, restore from the backup:

```bash
psql -U postgres -d <db_name> < BD/Innovation_project_BackUp.sql
```

## 3. Install backend dependencies

```bash
cd backend
dotnet restore
```

## 4. Install frontend dependencies

```bash
cd frontend
npm install
```

## Running the app

### Backend

```bash
cd backend
dotnet run
```

API runs at `http://localhost:5051`

### Frontend

```bash
cd frontend
npm run dev
```

App runs at `http://localhost:5173`
