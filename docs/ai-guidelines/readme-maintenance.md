# README Maintenance Guide

This guide explains how README files should be updated incrementally in PCMS.

## General Rules

- Keep README files short, accurate, and useful.
- Do not document features as completed unless they are implemented.
- Use "Planned", "In progress", or "Implemented" when feature status matters.
- Add setup commands only after they are tested or confirmed.
- Do not expose secrets, connection strings, tokens, credentials, private keys, or private deployment details.
- Do not include real patient data.
- Prefer clear setup instructions over long explanations.
- Do not duplicate detailed documentation from `docs/`; link to it when needed.
- Update README files when a major setup step, feature, or workflow becomes available.

## Root README

The root `README.md` is the main entry point for the repository.

It should include:

- Project description
- Current status
- Tech stack
- Planned or implemented MVP features
- Repository structure
- Basic navigation notes
- License or usage note

Keep it high-level.

## Backend README

Create or update `backend/README.md` when backend setup exists.

It should include:

- Backend stack
- Solution structure
- How to run the API
- Environment variables or user secrets example
- Database setup
- Migration commands
- Test commands
- Important backend notes

## Frontend README

Create or update `frontend/README.md` when frontend setup exists.

It should include:

- Frontend stack
- How to install dependencies
- How to run locally
- Environment variables example
- Build command
- Test command
- Localization/RTL notes when implemented

## Docs README

Create or update `docs/README.md` when documentation grows.

It should include:

- Documentation folder map
- What each docs section contains
- How to use the docs

## Incremental Update Rule

When updating README files:

1. Check what changed in the project.
2. Update only the relevant README file or section.
3. Do not rewrite the whole README unless requested.
4. Keep wording professional and concise.
5. Mark unavailable features as planned, not completed.
