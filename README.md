# AI Impact Evaluator

A tool that helps businesses determine whether using AI in a project creates enough value to justify its environmental and economic cost.

**Team:** Hassane Ramdjee, Kenza Braham, Chenchen Qiu, Roméo Pivat, Hugo De Sousa Dias, Léon Gard — EFREI Paris

---

## What it does

Users describe their AI use case and receive a multi-dimensional impact evaluation:

- **Environmental** — energy (kWh), carbon footprint (kg CO₂), water consumption (L)
- **Economic** — cost per request (USD)
- **Social** — hours saved vs. risk score

The system returns a verdict (approved / rejected) and stores the evaluation history per user.

---

## Tech stack

| Layer | Technology |
|---|---|
| Frontend | Vue 3, Vite, Vue Router 5 |
| Backend | .NET 10, ASP.NET Core Web API |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Database | PostgreSQL 17 via Entity Framework Core (Npgsql) |
| API testing | Bruno |

---

## Project structure

```
Innovation_Project/
├── frontend/                        # Vue 3 app
│   └── src/
│       ├── pages/                   # LoginPage, MainPage
│       ├── components/              # ResultsSection, AILevelIndicator, ImpactCards
│       └── router.js
├── Back-end_Innovation_Project/     # ASP.NET Core API
│   ├── APP/                         # Controllers + DTOs
│   ├── LOGIC/                       # Services, interfaces, ImpactCalculator
│   ├── PERSIST/                     # EF Core DbContext
│   ├── MODEL/                       # AppUser, EvaluationHistory
│   ├── COMMON/                      # Error codes, ServiceException
│   └── Migrations/
├── BD/                              # PostgreSQL backup
├── Bruno/                           # API test collection
└── docs/                            # Poster and sketchnote
```

---

## Prerequisites

- [Node.js](https://nodejs.org/) ≥ 22.18.0
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [PostgreSQL 17](https://www.postgresql.org/)

---

## Setup

### 1. Backend configuration

`appsettings.json` and `launchSettings.json` are excluded from git for security. Request them from the team, then place them at:

```
Back-end_Innovation_Project/appsettings.json
Back-end_Innovation_Project/Properties/launchSettings.json
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

### 2. Database

Create the PostgreSQL database, then apply migrations:

```bash
cd Back-end_Innovation_Project
dotnet ef database update
```

Alternatively, restore from the backup:

```bash
psql -U postgres -d <db_name> < BD/Innovation_project_BackUp.sql
```

### 3. Install backend dependencies

```bash
cd Back-end_Innovation_Project
dotnet restore
```

### 4. Install frontend dependencies

```bash
cd frontend
npm install
```

---

## Running the app

### Backend

```bash
cd Back-end_Innovation_Project
dotnet run
```

API runs at `http://localhost:5051`

### Frontend

```bash
cd frontend
npm run dev
```

App runs at `http://localhost:5173`

---

## API endpoints

All evaluation endpoints require a `Bearer` token in the `Authorization` header.

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | No | Create an account |
| `POST` | `/api/auth/login` | No | Login, returns JWT |
| `POST` | `/api/Evaluation/calculate` | Yes | Run impact evaluation |
| `PUT` | `/api/Evaluation/{id}/score` | Yes | Attach AI usefulness rating |
| `GET` | `/api/Evaluation/history` | Yes | Get user's evaluation history |

### Supported models

| Model | Provider options |
|---|---|
| GPT OSS 20B | Microsoft, Amazon, Référence |
| GPT OSS 120B | Microsoft, Amazon, Référence |
| DeepSeek V3.1 | Microsoft, Amazon, Référence |
| DeepSeek R1 | Microsoft, Amazon, Référence |

---

## API testing

A [Bruno](https://www.usebruno.com/) collection is available in `Bruno/test/` covering the full flow: register → login → calculate → score → history.

---

## Conventions

- **Code in English** — variable names, function names, comments, class names
- **UI in French** — all text displayed to the user
