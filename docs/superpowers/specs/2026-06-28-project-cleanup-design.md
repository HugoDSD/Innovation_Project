# Project-Wide Cleanup & Normalization — Design

**Date:** 2026-06-28
**Status:** Approved

## Goal

Enforce naming/coding best practices across the repo, simplify the backend
architecture to a conventional ASP.NET layout, remove dead code and stray
files, consolidate `.gitignore`s, and ensure docs/readmes stay consistent with
the result.

## Constraints

- **Code in English, UI in French** (project convention — unchanged).
- **No .NET SDK in this environment.** Backend changes are verified by
  exhaustive grep + preserving the HTTP/JSON contract; `dotnet build` is run
  by the user locally.
- **Frontend↔backend contract must be preserved** — routes (`api/Auth/*`,
  `api/Evaluation/*`) and JSON field names are unchanged, so the frontend needs
  no code changes.
- Git history preserved via `git mv` for all renames.

## Decisions (locked)

- Backend project/assembly/root namespace: **`InnovationProject`**.
- Backend internal structure: **conventional ASP.NET layout**, folders ==
  namespaces.

## 1. Backend rename → `InnovationProject`

- `backend/Back-end_Innovation_Project.csproj` → `backend/InnovationProject.csproj`
- `backend/Back-end_Innovation_Project.http` → `backend/InnovationProject.http`
- Root namespace `Back_end_Innovation_Project` → `InnovationProject` in every
  `.cs` file, including `Migrations/` and the EF model snapshot (string
  entity identifiers included).
- `M1.slnx` project path updated to the new `.csproj`.
- `RootNamespace` in the `.csproj` updated (or removed, since it now matches the
  assembly name).

## 2. Conventional ASP.NET layout

Folder moves (namespaces updated to match, single-rooted under
`InnovationProject`):

| Current | New folder | New namespace |
|---|---|---|
| `App/*Controller.cs` | `Controllers/` | `InnovationProject.Controllers` |
| `App/DTO.cs` | `Dtos/` (split, one file per DTO) | `InnovationProject.Dtos` |
| `Logic/` (services + calculator) | `Services/` | `InnovationProject.Services` |
| `Domain/` (interfaces) | `Interfaces/` | `InnovationProject.Interfaces` |
| `Model/` | `Models/` | `InnovationProject.Models` |
| `Persist/AppDb.cs` | `Data/` | `InnovationProject.Data` |
| `Migrations/` | `Data/Migrations/` | `InnovationProject.Data.Migrations` |
| `Common/` | **deleted** | — |

Class renames for convention (update all references + DI registration):

- `EvalController` → `EvaluationController` (uses explicit `[Route("api/Evaluation")]`, so the route is unchanged)
- `EvaluationServices` → `EvaluationService`
- `EvaluationHistoryDTO` → `EvaluationHistoryDto` (JSON property names are
  derived from the DTO's *properties*, not the class name, so the wire format
  is unchanged)

`Dtos/` split — `DTO.cs` becomes one file per type:
`RegisterDto.cs`, `LoginDto.cs`, `EvaluationHistoryDto.cs`,
`EvaluationRequestDto.cs`, `EvaluationResultDto.cs`, `EvaluationAiScoreDto.cs`.

## 3. Dead code & empty files

- Delete `Common/ErrorCode.cs` and `Common/ServiceException.cs` (never
  referenced anywhere in the codebase).
- Delete empty `Persist/EvaluationPersist.cs`.

## 4. `.gitignore` consolidation + untrack build artifacts

- `backend/obj/` is currently **committed**. Untrack with
  `git rm -r --cached backend/obj` (files stay on disk, regenerate on build).
- Merge `./.gitignore` (Node/Vite) and `backend/.gitignore` (.NET) into a single
  root `.gitignore` with clearly commented sections: OS, IDE, Node/frontend,
  .NET/backend, secrets (`appsettings.json`, `appsettings.Development.json`,
  `launchSettings.json`). Delete `backend/.gitignore`.

## 5. Stray file cleanup

- Delete `requierement.txt` (empty, misspelled).
- Delete root `package-lock.json` (orphan — there is no root `package.json`).
- Remove tracked `.DS_Store` files and confirm `.DS_Store` is ignored.

## 6. `docs/` contents & naming

- Move the **setup guide** out of `README.md` into **`docs/SETUP.md`**:
  the **Prerequisites**, **Setup** (configuration / database / install), and
  **Running the app** sections. `README.md` keeps overview (What it does / How
  it works / Tech stack / Project structure / API reference) and a pointer:
  "Setup → see [docs/SETUP.md](docs/SETUP.md)".
- Uppercase assets to match `TODO.md`: `poster.pdf` → `POSTER.pdf`,
  `sketchnote.png` → `SKETCHNOTE.png`.

Resulting `docs/`:

```
docs/
├── SETUP.md
├── TODO.md
├── POSTER.pdf
└── SKETCHNOTE.png
```

## 7. Frontend

No structural changes — already conventional (`pages/`, `components/`,
`services/`, PascalCase). The API contract is preserved, so no code changes are
needed. `npm run build` is run after the backend changes as a sanity check that
nothing referencing shared expectations broke.

## 8. Final consistency pass

- Exhaustive grep: no `Back-end_Innovation_Project`, `Back_end_Innovation_Project`,
  or old folder/class names remain (outside `obj/`).
- Update `README.md` (structure tree → new backend folders; setup pointer),
  `docs/TODO.md` (paths/names), `CLAUDE.md` (if affected), `M1.slnx`.
- `npm run build` passes.
- Verify frontend service endpoints still match backend routes and DTO field
  names (unchanged by design).

## Verification

- Backend: exhaustive grep proves rename completeness; contract preserved;
  **user runs `dotnet build` locally**.
- Frontend: `npm run build` passes.
- Docs: cross-checked against the final file/folder names.

## Out of scope

- No behavioral/logic changes to the API or UI.
- No frontend restructuring.
- No new features.
