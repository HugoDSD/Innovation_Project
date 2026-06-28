# Project Cleanup & Normalization — Implementation Plan

> **For agentic workers:** Use superpowers:subagent-driven-development or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Normalize naming, simplify the backend to a conventional ASP.NET layout, remove dead code and stray files, consolidate `.gitignore`, and keep docs/readmes consistent — without changing any runtime behavior.

**Architecture:** Pure structural refactor. Backend renamed to `InnovationProject` with folders == namespaces (Controllers/Services/Interfaces/Models/Dtos/Data). The HTTP routes and JSON field names are preserved, so the frontend is untouched. Bulk edits use a zsh-safe `while read` + `perl`-to-temp pattern (the environment blocks `perl -i`'s rename and zsh does not word-split `$(...)`).

**Tech Stack:** .NET 10 / ASP.NET Core / EF Core (backend), Vue 3 / Vite (frontend), git.

## Global Constraints

- Code in English; UI in French (unchanged).
- No .NET SDK here — verify the backend by exhaustive grep; the user runs `dotnet build` locally.
- Preserve the frontend↔backend contract: routes `api/Auth/*`, `api/Evaluation/*` and all JSON field names unchanged.
- Use `git mv` for renames to preserve history.
- Bulk in-place edit pattern (use everywhere a multi-file replace is needed):
  ```bash
  grep -rlE 'PATTERN' --include='*.cs' . | grep -v 'obj/' | while IFS= read -r f; do
    perl -pe 's/OLD/NEW/g' "$f" > "$f.__tmp" && cat "$f.__tmp" > "$f" && rm -f "$f.__tmp"
  done
  ```

---

## Task 1: Repo hygiene — untrack build artifacts, consolidate .gitignore, remove stray files

**Files:**
- Delete (untrack): `backend/obj/**` (tracked build output)
- Create/replace: `./.gitignore` (single consolidated file)
- Delete: `backend/.gitignore`, `requierement.txt`, `package-lock.json` (root orphan), tracked `.DS_Store` files

- [ ] **Step 1: Untrack committed build artifacts**

```bash
cd /Users/chenchen/Dev/Innovation_Project
git rm -r --cached backend/obj
```
Expected: lists `backend/obj/...` files being removed from the index (they remain on disk).

- [ ] **Step 2: Untrack stray/committed junk files**

```bash
git rm --cached -f requierement.txt package-lock.json 2>/dev/null
git rm --cached --ignore-unmatch '*.DS_Store' .DS_Store frontend/.DS_Store 2>/dev/null
```

- [ ] **Step 3: Write the consolidated root `.gitignore`**

Replace `./.gitignore` with:
```gitignore
# ===== OS =====
.DS_Store
.AppleDouble
.LSOverride
Thumbs.db
ehthumbs.db

# ===== Editors / IDE =====
.vscode/*
!.vscode/extensions.json
.vs/
.idea/
*.suo
*.user
*.ntvs*
*.njsproj
*.sw?

# ===== Logs =====
logs
*.log
npm-debug.log*
yarn-debug.log*
yarn-error.log*
pnpm-debug.log*

# ===== Frontend (Node / Vite) =====
node_modules
dist
dist-ssr
coverage
*.local
*.tsbuildinfo
.eslintcache
*.timestamp-*-*.mjs

# ===== Backend (.NET) =====
bin/
obj/
TestResults/

# ===== Secrets / local config =====
appsettings.json
appsettings.Development.json
**/Properties/launchSettings.json
```

- [ ] **Step 4: Delete the now-redundant backend gitignore and the physical stray files**

```bash
git rm -f backend/.gitignore
rm -f requierement.txt package-lock.json .DS_Store frontend/.DS_Store
```

- [ ] **Step 5: Verify**

```bash
git status --short | grep -E 'obj/|\.DS_Store|requierement|package-lock' || echo "no junk tracked"
git check-ignore backend/obj/project.assets.json backend/appsettings.json .DS_Store
```
Expected: no tracked junk remains; `git check-ignore` echoes all three paths (they are ignored).

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "chore: untrack build artifacts, consolidate gitignore, remove stray files"
```

---

## Task 2: Delete backend dead code

`Common/ErrorCode.cs` and `Common/ServiceException.cs` are never referenced; `Persist/EvaluationPersist.cs` is empty. `Program.cs` imports the `Common` namespace and must stop.

**Files:**
- Delete: `backend/Common/ErrorCode.cs`, `backend/Common/ServiceException.cs`, `backend/Persist/EvaluationPersist.cs`
- Modify: `backend/Program.cs` (remove the `Common` using)
- Modify: `backend/Model/EvaluationHistory.cs` (it has `using ...Common;` — verify/remove if unused)

- [ ] **Step 1: Confirm `Common` types are truly unused**

```bash
cd /Users/chenchen/Dev/Innovation_Project/backend
grep -rnE '\b(ErrorCode|ServiceException)\b' --include='*.cs' . | grep -v 'obj/' | grep -vE 'Common/(ErrorCode|ServiceException)\.cs'
```
Expected: no output (no references outside the definitions themselves).

- [ ] **Step 2: Delete the dead files**

```bash
git rm Common/ErrorCode.cs Common/ServiceException.cs Persist/EvaluationPersist.cs
rmdir Common 2>/dev/null || true
```

- [ ] **Step 3: Remove the `Common` import from `Program.cs`**

Delete this line from `backend/Program.cs`:
```csharp
using Back_end_Innovation_Project.Common;
```

- [ ] **Step 4: Remove the `Common` import from `EvaluationHistory.cs` if present and unused**

Check `backend/Model/EvaluationHistory.cs` for `using Back_end_Innovation_Project.Common;`. The class uses only `[Key]`, `[ForeignKey]`, and primitive types, so if the `Common` using is present, delete that line.

- [ ] **Step 5: Verify no references to deleted namespace remain**

```bash
grep -rnE 'Innovation_Project\.Common|\bErrorCode\b|\bServiceException\b' --include='*.cs' . | grep -v 'obj/' || echo "Common fully removed"
```
Expected: `Common fully removed`.

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "refactor: remove dead Common/ types and empty EvaluationPersist.cs"
```

---

## Task 3: Rename root namespace and project files to `InnovationProject`

Replace the root namespace `Back_end_Innovation_Project` → `InnovationProject` everywhere, rename the `.csproj`/`.http`, update `RootNamespace` and the solution.

**Files:**
- Modify: every `backend/**/*.cs` (excluding `obj/`) — root namespace token
- Rename: `backend/Back-end_Innovation_Project.csproj` → `backend/InnovationProject.csproj`
- Rename: `backend/Back-end_Innovation_Project.http` → `backend/InnovationProject.http`
- Modify: the `.csproj` (`RootNamespace`), `M1.slnx`

- [ ] **Step 1: Replace the root namespace token across all source**

```bash
cd /Users/chenchen/Dev/Innovation_Project/backend
grep -rlE 'Back_end_Innovation_Project' --include='*.cs' . | grep -v 'obj/' | while IFS= read -r f; do
  perl -pe 's/Back_end_Innovation_Project/InnovationProject/g' "$f" > "$f.__tmp" && cat "$f.__tmp" > "$f" && rm -f "$f.__tmp"
done
grep -rnE 'Back_end_Innovation_Project' --include='*.cs' . | grep -v 'obj/' || echo "no underscore-name refs remain"
```
Expected: `no underscore-name refs remain`.

- [ ] **Step 2: Rename the project and http files**

```bash
git mv Back-end_Innovation_Project.csproj InnovationProject.csproj
git mv Back-end_Innovation_Project.http InnovationProject.http
```

- [ ] **Step 3: Update `RootNamespace` in the `.csproj`**

In `backend/InnovationProject.csproj`, change:
```xml
<RootNamespace>Back_end_Innovation_Project</RootNamespace>
```
to:
```xml
<RootNamespace>InnovationProject</RootNamespace>
```

- [ ] **Step 4: Update the `.http` file host variable if it references the old name**

Open `backend/InnovationProject.http`. If it contains a variable like `@Back-end_Innovation_Project_HostAddress`, rename it to `@InnovationProject_HostAddress` (and its usage).

- [ ] **Step 5: Update the solution project path**

In `M1.slnx`, change the path to:
```xml
<Project Path="backend/InnovationProject.csproj" />
```

- [ ] **Step 6: Verify**

```bash
cd /Users/chenchen/Dev/Innovation_Project
grep -rn 'Back-end_Innovation_Project\|Back_end_Innovation_Project' --include='*.cs' --include='*.csproj' --include='*.slnx' --include='*.http' backend M1.slnx | grep -v 'obj/' || echo "old project name fully gone"
ls backend/InnovationProject.csproj backend/InnovationProject.http
```
Expected: `old project name fully gone`; both files listed.

- [ ] **Step 7: Commit**

```bash
git add -A
git commit -m "refactor: rename backend project and root namespace to InnovationProject"
```

---

## Task 4: Convert to conventional ASP.NET layout

Folders == namespaces under `InnovationProject`. Rename folders, sub-namespaces, three classes, and split DTOs.

**Files:**
- Move: `App/AuthController.cs`, `App/EvalController.cs` → `Controllers/`
- Move/split: `App/DTO.cs` → `Dtos/*.cs` (6 files)
- Move: `Logic/*.cs` → `Services/`
- Move: `Domain/*.cs` → `Interfaces/`
- Move: `Model/*.cs` → `Models/`
- Move: `Persist/AppDb.cs` → `Data/`; `Migrations/` → `Data/Migrations/`
- Modify: `Program.cs` (usings + DI type), all moved files (namespaces), class renames

**Interfaces (names other tasks/files rely on after this task):**
- Namespaces: `InnovationProject.Controllers`, `InnovationProject.Dtos`, `InnovationProject.Services`, `InnovationProject.Interfaces`, `InnovationProject.Models`, `InnovationProject.Data`, `InnovationProject.Data.Migrations`
- Classes: `EvaluationController`, `EvaluationService`, `EvaluationHistoryDto`

- [ ] **Step 1: Create the new folders and move files with git**

```bash
cd /Users/chenchen/Dev/Innovation_Project/backend
mkdir -p Controllers Dtos Services Interfaces Models Data/Migrations
git mv App/AuthController.cs Controllers/AuthController.cs
git mv App/EvalController.cs Controllers/EvaluationController.cs
git mv Logic/AuthService.cs Services/AuthService.cs
git mv Logic/EvaluationServices.cs Services/EvaluationService.cs
git mv Logic/ImpactCalculator.cs Services/ImpactCalculator.cs
git mv Domain/IAuthService.cs Interfaces/IAuthService.cs
git mv Domain/IEvaluationService.cs Interfaces/IEvaluationService.cs
git mv Model/AppUser.cs Models/AppUser.cs
git mv Model/EvaluationHistory.cs Models/EvaluationHistory.cs
git mv Persist/AppDb.cs Data/AppDb.cs
git mv Migrations/*.cs Data/Migrations/
rmdir App Logic Domain Model Persist Migrations 2>/dev/null || true
```

- [ ] **Step 2: Rewrite sub-namespaces to match folders (bulk)**

```bash
grep -rlE 'InnovationProject\.(App|Logic|Model|Persist|Migrations)' --include='*.cs' . | grep -v 'obj/' | while IFS= read -r f; do
  perl -pe '
    s/InnovationProject\.App\.Controllers/InnovationProject.Controllers/g;
    s/InnovationProject\.App\.DTOs/InnovationProject.Dtos/g;
    s/InnovationProject\.Logic\.Services/InnovationProject.Services/g;
    s/InnovationProject\.Logic\.Calculators/InnovationProject.Services/g;
    s/InnovationProject\.Logic\.Interfaces/InnovationProject.Interfaces/g;
    s/InnovationProject\.Model\b/InnovationProject.Models/g;
    s/InnovationProject\.Persist\b/InnovationProject.Data/g;
    s/InnovationProject\.Migrations\b/InnovationProject.Data.Migrations/g;
  ' "$f" > "$f.__tmp" && cat "$f.__tmp" > "$f" && rm -f "$f.__tmp"
done
grep -rnE 'InnovationProject\.(App|Logic|Persist)\b|InnovationProject\.Model\b|InnovationProject\.Migrations\b' --include='*.cs' . | grep -v 'obj/' || echo "sub-namespaces normalized"
```
Expected: `sub-namespaces normalized`.

- [ ] **Step 3: Rename the three classes (bulk, whole-word)**

```bash
grep -rlE '\b(EvalController|EvaluationServices|EvaluationHistoryDTO)\b' --include='*.cs' . | grep -v 'obj/' | while IFS= read -r f; do
  perl -pe 's/\bEvalController\b/EvaluationController/g; s/\bEvaluationServices\b/EvaluationService/g; s/\bEvaluationHistoryDTO\b/EvaluationHistoryDto/g;' "$f" > "$f.__tmp" && cat "$f.__tmp" > "$f" && rm -f "$f.__tmp"
done
grep -rnE '\b(EvalController|EvaluationServices|EvaluationHistoryDTO)\b' --include='*.cs' . | grep -v 'obj/' || echo "classes renamed"
```
Expected: `classes renamed`. (`EvaluationService` as a substring of nothing else; `\b` keeps `IEvaluationService` intact because `EvaluationServices` requires the trailing `s` — verify next step.)

- [ ] **Step 4: Verify the interface name was not corrupted**

```bash
grep -rn 'IEvaluationService' --include='*.cs' . | grep -v 'obj/' | head
grep -rn 'IEvaluationServic\b' --include='*.cs' . | grep -v 'obj/' && echo "CORRUPTION!" || echo "IEvaluationService intact"
```
Expected: `IEvaluationService intact`. (The `EvaluationServices`→`EvaluationService` rule has a trailing `s`; `IEvaluationService` has no trailing `s`, so it is untouched.)

- [ ] **Step 5: Split `Dtos/DTO.cs` into one file per DTO**

```bash
git rm Dtos/DTO.cs 2>/dev/null; rm -f Dtos/DTO.cs
```
Then create the six files:

`Dtos/RegisterDto.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace InnovationProject.Dtos;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    public required string Name { get; set; }
    public required string Surname { get; set; }

    public string? CompanyName { get; set; }
}
```

`Dtos/LoginDto.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace InnovationProject.Dtos;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Password { get; set; }
}
```

`Dtos/EvaluationHistoryDto.cs`:
```csharp
namespace InnovationProject.Dtos;

public class EvaluationHistoryDto
{
    public string Id { get; set; } = string.Empty;

    public string ModelName { get; set; } = string.Empty;
    public string AiScore { get; set; } = string.Empty;

    // --- Environmental metrics ---
    public double CarbonFootprint { get; set; }
    public double WaterFootprintLiters { get; set; }
    public double EnergyKwh { get; set; }

    public double CostUsd { get; set; }
    public double HoursSaved { get; set; }
    public double RiskScore { get; set; }

    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

`Dtos/EvaluationRequestDto.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace InnovationProject.Dtos;

public class EvaluationRequestDto
{
    public required string ModelName { get; set; }
    public required string Provider { get; set; }
    public long InputTokens { get; set; }
    public long OutputTokens { get; set; }

    public double HoursSavedReports { get; set; }
    public double HoursSavedImages { get; set; }
    public double HoursSavedPresentations { get; set; }

    [Range(1, 5)]
    public int DataSensitivity { get; set; }

    [Range(1, 5)]
    public int LegalRisk { get; set; }
}
```

`Dtos/EvaluationResultDto.cs`:
```csharp
namespace InnovationProject.Dtos;

public class EvaluationResultDto
{
    public bool IsApproved { get; set; }
    public int EvaluationId { get; set; }
    public string Message { get; set; } = string.Empty;

    // Environmental impact
    public double TotalEnergyKwh { get; set; }
    public double TotalCarbonKg { get; set; }
    public double TotalWaterLiters { get; set; }

    // Economic impact
    public double TotalCostUsd { get; set; }

    // Social impact
    public double TotalHoursSaved { get; set; }
    public double RiskScore { get; set; }
}
```

`Dtos/EvaluationAiScoreDto.cs`:
```csharp
namespace InnovationProject.Dtos;

public class EvaluationAiScoreDto
{
    // Only the AI score is included, not the evaluation id, since the id is passed in the URL (for security and optimization — RESTful design)
    public string AiScore { get; set; } = string.Empty;
}
```

- [ ] **Step 6: Update `Program.cs` usings and DI registration**

In `backend/Program.cs`, replace the namespace usings block (lines importing Model/Persist/Logic.Services/Logic.Interfaces) so it reads:
```csharp
using InnovationProject.Models;
using InnovationProject.Data;
using InnovationProject.Services;
using InnovationProject.Interfaces;
```
and ensure the DI line registers the renamed service:
```csharp
builder.Services.AddScoped<IEvaluationService, EvaluationService>();
```

- [ ] **Step 7: Verify namespaces match folders and the contract is intact**

```bash
echo "=== namespace declarations ==="; grep -rhE '^namespace ' --include='*.cs' . | grep -v 'obj/' | sort -u
echo "=== routes unchanged ==="; grep -rnE '\[Route|\[Http' Controllers/*.cs
echo "=== DI uses EvaluationService ==="; grep -n 'AddScoped<IEvaluationService' Program.cs
```
Expected namespaces: `InnovationProject.Controllers`, `InnovationProject.Dtos`, `InnovationProject.Services`, `InnovationProject.Interfaces`, `InnovationProject.Models`, `InnovationProject.Data`, `InnovationProject.Data.Migrations`. Routes still `api/[controller]` (Auth) and `api/Evaluation`.

- [ ] **Step 8: Commit**

```bash
cd /Users/chenchen/Dev/Innovation_Project
git add -A
git commit -m "refactor: adopt conventional ASP.NET layout (Controllers/Services/Interfaces/Models/Dtos/Data)"
```

---

## Task 5: Docs — move setup guide to `docs/SETUP.md`, uppercase assets

**Files:**
- Create: `docs/SETUP.md`
- Modify: `README.md` (remove moved sections, add pointer, fix structure tree)
- Rename: `docs/poster.pdf` → `docs/POSTER.pdf`, `docs/sketchnote.png` → `docs/SKETCHNOTE.png`

- [ ] **Step 1: Create `docs/SETUP.md`** with the Prerequisites, Setup, and Running-the-app content moved verbatim from `README.md`, headed by:
```markdown
# Setup Guide

> Prerequisites, configuration, database setup, and how to run the app.
```
(Move the existing **Prerequisites**, **Setup**, and **Running the app** sections from `README.md` into this file, updating the backend path references to the conventional layout where mentioned.)

- [ ] **Step 2: Trim `README.md`** — remove the moved sections and insert, right after the Tech-stack/Project-structure area:
```markdown
## Setup

See **[docs/SETUP.md](docs/SETUP.md)** for prerequisites, configuration, database setup, and how to run the app.
```

- [ ] **Step 3: Update the `README.md` project-structure tree** to the conventional backend layout:
```
├── backend/                         # ASP.NET Core API (InnovationProject)
│   ├── Controllers/                 # AuthController, EvaluationController
│   ├── Services/                    # AuthService, EvaluationService, ImpactCalculator
│   ├── Interfaces/                  # IAuthService, IEvaluationService
│   ├── Dtos/                        # request/response DTOs
│   ├── Models/                      # AppUser, EvaluationHistory
│   └── Data/                        # AppDb + Migrations
```

- [ ] **Step 4: Uppercase the doc assets**

```bash
cd /Users/chenchen/Dev/Innovation_Project
git mv docs/poster.pdf docs/POSTER.pdf
git mv docs/sketchnote.png docs/SKETCHNOTE.png
```

- [ ] **Step 5: Verify**

```bash
ls docs
grep -n 'docs/SETUP.md' README.md
grep -nE 'Prerequisites|dotnet run|npm install' README.md && echo "WARN: setup content still in README" || echo "setup fully moved"
```
Expected: `docs/` shows `SETUP.md TODO.md POSTER.pdf SKETCHNOTE.png` (+ `superpowers/`); README links SETUP.md; setup content moved.

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "docs: move setup guide to docs/SETUP.md and uppercase doc assets"
```

---

## Task 6: Final consistency sweep

**Files:**
- Modify (as needed): `docs/TODO.md`, `CLAUDE.md`, `README.md`

- [ ] **Step 1: Global sweep for any stale name anywhere (excluding build output and git)**

```bash
cd /Users/chenchen/Dev/Innovation_Project
grep -rnE 'Back-end_Innovation_Project|Back_end_Innovation_Project|\b(EvalController|EvaluationServices|EvaluationHistoryDTO)\b' . \
  --exclude-dir=obj --exclude-dir=bin --exclude-dir=.git --exclude-dir=node_modules --exclude-dir=dist 2>/dev/null || echo "no stale references anywhere"
```
Expected: `no stale references anywhere`.

- [ ] **Step 2: Update `docs/TODO.md` and `CLAUDE.md` references** to any renamed backend paths/folders so they match the conventional layout (e.g. `backend/App/...` → `backend/Controllers|Services|...`). Use the bulk pattern if multiple.

- [ ] **Step 3: Frontend still builds**

```bash
cd frontend && npm run build 2>&1 | tail -4
```
Expected: `✓ built in ...` with no errors.

- [ ] **Step 4: Confirm the frontend↔backend contract is intact**

```bash
cd /Users/chenchen/Dev/Innovation_Project
echo "=== frontend endpoints ==="; grep -hoE "apiFetch\(\`?'?[^,\`)]*" frontend/src/services/*.js
echo "=== backend routes ==="; grep -rnE '\[Route|\[Http' backend/Controllers/*.cs
echo "=== result DTO fields vs frontend usage ==="; \
  grep -oE 'public [A-Za-z?]+ [A-Za-z]+' backend/Dtos/EvaluationResultDto.cs | awk '{print $3}' | sort | paste -sd' ' -; \
  grep -rhoE 'results\.[a-zA-Z]+' frontend/src/components/ResultsSection.vue | sort -u | paste -sd' ' -
```
Expected: endpoints `/auth/login`, `/auth/register`, `/Evaluation/calculate`, `/Evaluation/{id}/score`, `/Evaluation/history`; routes `api/[controller]` + `api/Evaluation`; DTO fields map (camelCased) to the `results.*` the UI reads.

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "docs: final consistency pass after cleanup refactor"
```

- [ ] **Step 6: Hand off backend compile check**

State clearly to the user: the backend was not compiled here (no .NET SDK). Ask them to run `dotnet build backend/InnovationProject.csproj` (or `dotnet build M1.slnx`) to confirm, and report any errors back.

---

## Notes for the implementer

- The `EvaluationServices` → `EvaluationService` rule deliberately matches the trailing `s`; `IEvaluationService` (no trailing `s`) is untouched. Always run the Task 4 Step 4 corruption check.
- EF migration files reference entity types as **string identifiers** (e.g. `"InnovationProject.Models.AppUser"`) and via `using`; both are handled by the namespace replacements. After moving migrations into `Data/Migrations/`, their own namespace becomes `InnovationProject.Data.Migrations` — EF does not require the migrations namespace to match anything, so this compiles.
- Nothing in this plan changes runtime behavior; if `dotnet build` fails, it is a missed reference, not a logic error — grep for the exact missing symbol.
