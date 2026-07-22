# CLAUDE.md — AquaTrack

Este archivo es la memoria persistente del proyecto para Claude Code. Como la sesión se reinicia
cada vez que se reabre VSCode, **cualquier sesión nueva debe leer este archivo y `PROGRESS.md`
completos antes de escribir una sola línea de código**, y resumir brevemente el estado antes de continuar.

## 1. Resumen del proyecto

AquaTrack es una plataforma web de gestión de operaciones para explotaciones acuícolas: control de
lotes de cultivo, parámetros ambientales, alimentación, turnos de personal e incidencias.

Es un **proyecto de portfolio**, no una app de producción real, pero debe comportarse y verse como
un producto cuidado. Es la pieza central del portfolio fullstack de Rafael (React/React Native,
Node.js, ASP.NET Core/C#, SQL Server, MongoDB), inspirado en su experiencia real de 3 años en
acuicultura (responsable de turno de fin de semana y suplente de jefe de operaciones) y como socio
fundador de una startup de microalgas.

**Importante — originalidad:** esta es una implementación diseñada desde cero. No se replica
código, pantallas, nombres de tablas ni lógica de negocio de ningún empleador anterior. Solo se
reutiliza conocimiento general del dominio (qué se controla en una explotación acuícola).

## 2. Stack tecnológico y por qué

| Pieza | Elección | Por qué |
|---|---|---|
| Frontend | React + TypeScript + Vite | Estándar de facto, tooling rápido, tipado fuerte |
| Estilos | Tailwind CSS | Velocidad de desarrollo, consistencia visual sin CSS a medida |
| Routing | React Router | Estándar en SPAs de React |
| Estado de servidor | TanStack Query (React Query) | Cache, refetch e invalidación sin Redux manual |
| Gráficos | Recharts | Gráficos de series temporales (parámetros ambientales) con poco boilerplate |
| Backend | ASP.NET Core (C#) | Mostrar dominio de .NET; arquitectura en capas real |
| ORM | Entity Framework Core (migraciones) | En el otro proyecto del portfolio se usa Dapper + SPs; aquí se diversifica mostrando ORM + migrations |
| Base de datos | PostgreSQL | Diversifica frente a SQL Server (otro proyecto); buen hosting gratuito (Neon/Supabase) |
| Auth | JWT + roles (Admin, JefeDeTurno, Operario) | Estándar stateless, fácil de defender en entrevista |
| Testing backend | xUnit + Moq | Estándar .NET |
| Testing frontend | Vitest + React Testing Library | Estándar Vite/React |
| Docs API | Swagger / OpenAPI | Autodocumentado, estándar en .NET |
| Contenerización | Docker + docker-compose | `docker-compose up` levanta todo en local |
| CI/CD | GitHub Actions | Lint + tests en cada push/PR |
| Despliegue | Backend: Railway/Render · Frontend: Vercel · DB: Neon/Supabase | Capas gratuitas, URL pública final |

Si alguna de estas decisiones deja de tener sentido técnico según avanza el proyecto, se debe
comentar con Rafael antes de cambiarla — no cambiar por cuenta propia.

## 3. Estructura del repositorio

Monorepo (decisión tomada 2026-07-22): un único repo con `/frontend` y `/backend`, un solo
`docker-compose.yml` y un solo README. Facilita la navegación para quien revise el portfolio y la
continuidad entre sesiones.

```
aquatrack/
├── backend/
│   ├── src/
│   │   ├── AquaTrack.Domain/          # Entidades, enums, excepciones de dominio. Sin dependencias externas.
│   │   ├── AquaTrack.Application/     # Casos de uso, DTOs, interfaces, validadores (FluentValidation)
│   │   ├── AquaTrack.Infrastructure/  # EF Core DbContext, migraciones, repositorios
│   │   └── AquaTrack.Api/             # Controllers, Program.cs, middleware, DI, appsettings
│   └── tests/
│       ├── AquaTrack.Domain.Tests/
│       ├── AquaTrack.Application.Tests/
│       └── AquaTrack.Api.IntegrationTests/
├── frontend/
│   └── src/
│       ├── api/                # Cliente API + hooks de React Query
│       ├── components/         # Componentes UI compartidos/reutilizables
│       ├── features/           # Carpetas por feature: auth, facilities, environmental-params,
│       │                       # alerts, feeding, shifts, incidents, traceability, dashboard, users
│       ├── layouts/            # Layouts de página (shell autenticado, layout público)
│       ├── routes/             # Componentes a nivel de ruta (React Router)
│       ├── hooks/              # Hooks compartidos
│       ├── lib/                # Utilidades, cliente HTTP, helpers
│       ├── types/              # Tipos globales/compartidos
│       └── context/            # Auth context, etc.
└── .github/workflows/          # Pipelines de CI/CD
```

Ninguno de los proyectos (.NET solution, Vite app) está scaffoldeado todavía — solo existe el
esqueleto de carpetas con `.gitkeep`. Eso es el próximo paso.

## 4. Convenciones

- **Idioma de código:** nombres de variables, funciones, clases y comentarios en inglés.
- **Idioma de documentación:** README en español e inglés (o al menos inglés) para alcance internacional.
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`, etc.). Commits pequeños y frecuentes, no monolíticos — sirven de bitácora del proyecto y de red de seguridad entre sesiones.
- **Arquitectura backend:** capas Domain → Application → Infrastructure → Api. Nada de lógica de negocio en los controllers.
- **Validación:** en frontend y backend (FluentValidation o DataAnnotations en el backend).
- **Errores:** middleware centralizado de manejo de errores, respuestas consistentes.
- **Testing:** unit tests en lógica de negocio crítica (cálculo de alertas, trazabilidad) + integration tests en los endpoints principales. No se busca 100% de cobertura, sí intención clara.

## 5. Comandos útiles

_(Se completará según se vaya scaffoldeando cada parte — todavía no hay proyectos .NET ni npm inicializados)._

```
# Backend (pendiente de scaffold)
cd backend && dotnet build
cd backend && dotnet test
cd backend/src/AquaTrack.Api && dotnet ef migrations add <Name>
cd backend/src/AquaTrack.Api && dotnet ef database update

# Frontend (pendiente de scaffold)
cd frontend && npm install
cd frontend && npm run dev
cd frontend && npm run test
cd frontend && npm run lint

# Todo el stack (pendiente de docker-compose.yml)
docker-compose up
```

## 6. Cómo trabajamos

- Feature por feature, no todo a la vez.
- Antes de empezar una feature nueva: plan breve (archivos a tocar, decisiones de diseño) y esperar
  confirmación si hay ambigüedad de alcance.
- Priorizar código limpio y bien tipado por encima de velocidad.
- Al final de cada sesión: actualizar este archivo (sección "Estado actual"), marcar avances en
  `PROGRESS.md`, anotar decisiones técnicas relevantes, y hacer commit de todo con Conventional Commits.

## 7. Decisiones técnicas registradas

- **2026-07-22** — Nombre del proyecto: **AquaTrack** (confirmado tras proponer alternativas como
  AquaOps, TankPulse).
- **2026-07-22** — Monorepo en vez de repos separados frontend/backend: prioriza facilidad de
  navegación para revisores del portfolio y continuidad entre sesiones sobre el realismo de
  "microservicios separados".
- **2026-07-22** — Arquitectura backend en capas (Domain/Application/Infrastructure/Api) siguiendo
  Clean Architecture, para diferenciarse del otro proyecto del portfolio (que usa Dapper + SPs) y
  mostrar dominio de EF Core + separación de responsabilidades.
- **2026-07-22** — Estructura de frontend organizada por feature (`features/<nombre>`) en vez de por
  tipo técnico (`components/`, `pages/` planos), para que cada dominio funcional (alertas,
  alimentación, turnos...) sea autocontenido y fácil de razonar.
- **2026-07-22** — Roles en código en inglés: `Admin`, `ShiftLead`, `Operator` (en el documento
  original: Admin, JefeDeTurno, Operario). Por la convención de código en inglés de la sección de
  convenciones; la UI puede mostrar las etiquetas traducidas sin que el enum/rol en backend use
  español. **Pendiente de confirmar con Rafael** si prefiere mantener esos nombres exactos u otros.

## 8. Estado actual

**Última sesión:** 2026-07-22

**Hecho:**
- Repositorio git inicializado en `aquatrack/` (rama `main`).
- `.gitignore` creado (.NET + Node + Docker + env files).
- Esqueleto de carpetas creado para backend (Clean Architecture: Domain/Application/Infrastructure/Api
  + tests) y frontend (estructura por feature).
- `CLAUDE.md` y `PROGRESS.md` creados.

**En qué se está trabajando ahora mismo:**
- Nada en curso — a la espera de decidir el siguiente paso (scaffolding real del backend .NET y del
  frontend Vite).

**Próximos pasos inmediatos:**
1. Scaffoldear el proyecto .NET (solution + 4 proyectos por capa + proyectos de test) dentro de `backend/`.
2. Scaffoldear el proyecto Vite + React + TS + Tailwind dentro de `frontend/`.
3. Diseñar el modelo de dominio inicial (entidades: Facility/Lote, EnvironmentalReading, Alert, User/Role) antes de escribir código de negocio — Fase 1 del MVP.
4. Configurar docker-compose con Postgres para desarrollo local.
5. Primer commit de scaffolding con Conventional Commits (`chore: scaffold backend and frontend projects`).

**Bloqueos/problemas conocidos:** ninguno.
