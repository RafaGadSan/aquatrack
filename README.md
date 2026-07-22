# AquaTrack

Operations management platform for aquaculture facilities — culture batches, environmental
parameters, feeding, staff shifts and incident tracking.

> 🚧 Work in progress — portfolio project. Full README (screenshots, architecture diagram, setup
> instructions, tech decisions) will land as the MVP takes shape. See `CLAUDE.md` for the current
> project state and `PROGRESS.md` for the phase-by-phase checklist.

## Stack

- **Frontend:** React + TypeScript + Vite, Tailwind CSS, React Router, TanStack Query, Recharts
- **Backend:** ASP.NET Core (C#), Entity Framework Core, layered architecture (Domain/Application/Infrastructure/Api)
- **Database:** PostgreSQL
- **Auth:** JWT with role-based access (Admin, ShiftLead, Operator)
- **Testing:** xUnit + Moq (backend), Vitest + React Testing Library (frontend)
- **Infra:** Docker + docker-compose, GitHub Actions CI

## Status

Repository scaffolding in progress. No runnable code yet — see `PROGRESS.md`.
