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

## Running locally

```bash
cp .env.example .env
docker compose up -d
```

- Frontend: http://localhost:5173
- Backend / Swagger: http://localhost:5000/swagger
- Postgres: localhost:5432

## Status

Backend and frontend are scaffolded (no domain/business logic yet) and verified to build and run
together via Docker. See `PROGRESS.md` for the phase-by-phase checklist.
