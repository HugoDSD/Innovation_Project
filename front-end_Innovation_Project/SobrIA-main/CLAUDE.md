# CLAUDE.md

## Language conventions

- **Output / responses to the user**: always in English, whatever the input language.
- **Documentation (docs)**: always in French.
- **Code** (identifiers, comments, commit messages): always in English.

## Commit conventions

**Commit after every completed task/change** — don't wait to be asked. Each
logical unit of work gets its own commit on `main`.

Use [Conventional Commits](https://www.conventionalcommits.org/): every commit
message follows `type(optional scope): description`.

- **Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`,
  `build`, `ci`, `chore`, `revert`.
- **Description**: imperative mood, lowercase, no trailing period
  (e.g. `feat(form): add workflow frequency field`).
- **Breaking changes**: append `!` after the type/scope (`feat!: ...`) and/or
  add a `BREAKING CHANGE:` footer.
- Keep the subject line under ~72 characters; use the body for details.
