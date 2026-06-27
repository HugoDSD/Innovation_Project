# AI Impact Evaluator

A tool that helps businesses determine whether using AI in a project creates enough value to justify its environmental and economic cost.

**Team:** Hassane Ramdjee, Kenza Braham, Chenchen Qiu, Roméo Pivat, Hugo De Sousa Dias, Léon Gard — EFREI Paris

---

## What it does

Generative AI consumes real resources — electricity, water, money — and carries legal and data risks. Yet most teams integrate it without ever measuring whether the value it creates actually justifies that cost.

This tool gives any business employee a structured way to evaluate an AI integration project and receive a clear, data-backed recommendation.

## How it works

The evaluation follows three steps:

**1. Describe the project**
The user provides context about what the AI will be used for: which tasks it will handle, how many hours it is expected to save (broken down by reports, images, and presentations), and the project's risk profile (data sensitivity and legal risk on a 1–5 scale).

**2. Configure the AI**
The user selects the AI model and cloud provider they plan to use, and provides the expected token volumes (input and output). The tool supports GPT OSS 20B, GPT OSS 120B, DeepSeek V3.1, and DeepSeek R1, across Microsoft, Amazon, and reference infrastructure.

**3. Evaluate and analyse**
The tool computes a full multi-dimensional impact report:

- **Environmental impact** — total energy consumed (kWh), carbon footprint (kg CO₂) based on the French electricity mix (0.0801 kg CO₂/kWh), and water consumption (L) based on the provider's water usage effectiveness ratio
- **Economic impact** — total cost in USD based on per-token pricing for the selected model
- **Social impact** — total hours saved across task types, and a composite risk score

Based on these metrics, the system delivers a verdict:
- **Approuvé** — the time savings outweigh the environmental impact and risks are under control
- **Rejeté** — the benefits do not compensate for the impact or risk level

After reviewing the results, the user can attach a usefulness rating to the evaluation (Utile, Moyen, Non utile, or Mieux sans IA), and all evaluations are saved to a personal history for future reference.

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
