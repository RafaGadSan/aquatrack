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

```
# Backend (backend/AquaTrack.sln — .NET 8 SDK, verificado con SDK 8.0.400)
cd backend && dotnet build
cd backend && dotnet test
cd backend && dotnet run --project src/AquaTrack.Api
# Migraciones EF Core (pendiente: aún no hay DbContext, se añade en Fase 1)
cd backend && dotnet ef migrations add <Name> --project src/AquaTrack.Infrastructure --startup-project src/AquaTrack.Api
cd backend && dotnet ef database update --project src/AquaTrack.Infrastructure --startup-project src/AquaTrack.Api

# Frontend (frontend/ — Vite 8 + React 19 + TS, Node v22.17.0 / npm 10.9.2)
cd frontend && npm install
cd frontend && npm run dev
cd frontend && npm run build       # tsc -b && vite build
cd frontend && npm run test        # vitest (watch mode); usar `npx vitest run` para un solo pase
cd frontend && npm run lint        # oxlint

# Todo el stack (docker-compose.yml en la raíz: postgres + backend + frontend)
cp .env.example .env   # solo la primera vez
docker compose up -d
docker compose down
# frontend: http://localhost:5173 · backend/swagger: http://localhost:5000/swagger · postgres: localhost:5432
```

**Notas de versiones (importante para futuras sesiones):**
- El SDK de .NET instalado es 8.0.400, pero `dotnet add package` sin `--version` resuelve a la
  última versión de NuGet (probado: EF Core resolvió a 10.0.10, que exige `net10.0` y rompe la
  build). **Al añadir paquetes de Microsoft.EntityFrameworkCore.\* o de ASP.NET Core hay que fijar
  la versión explícitamente a la línea 8.0.x** (ej. `--version 8.0.11`) para que coincida con el
  `TargetFramework net8.0` de los proyectos.
- El frontend quedó con **Tailwind CSS v4** (no v3): no hay `tailwind.config.js` ni `postcss.config.js`
  — la v4 usa el plugin `@tailwindcss/vite` y `@import "tailwindcss";` en `index.css`. Si se necesita
  personalizar el tema, se hace con `@theme` dentro del CSS, no en un config JS.
- El template de Vite actual usa **oxlint** en vez de ESLint (`npm run lint` → `oxlint`).
- `vite.config.ts` importa `defineConfig` desde `vitest/config` (no desde `vite`) para poder incluir
  el bloque `test: {...}` con tipado correcto.
- `vitest run` sin tests devuelve **exit code 1** por defecto ("No test files found") — hay que usar
  `--passWithNoTests` (así está en `frontend-ci.yml`) mientras no existan tests todavía. `dotnet test`
  con proyectos de test vacíos, en cambio, sí devuelve exit code 0 de forma nativa.
- El frontend se sirve en Docker vía Nginx (`frontend/nginx.conf`, con fallback a `index.html` para
  el routing de React Router), no con `vite dev` — así el contenedor se parece al despliegue real
  (Vercel sirviendo estáticos). `VITE_API_URL` se hornea en build time (build arg de Docker /
  variable de entorno de Vite), no es una env var de runtime.
- Stack completo verificado end-to-end con `docker compose build && docker compose up -d`: los 3
  contenedores (postgres healthy, backend con Swagger en `/swagger`, frontend con Nginx) responden
  correctamente. Backend aún no tiene DbContext real conectado a Postgres — eso llega en Fase 1.

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
- **2026-07-22** — Fase 1 modela solo `Facility` (con su propio `FacilityStatus`:
  Empty/Active/Harvesting); la entidad `Batch`/lote con trazabilidad completa (siembra→cosecha,
  especie, eventos históricos) se deja para Fase 2. Confirmado explícitamente con Rafael para no
  modelar de más en el MVP. `Batch` referenciará `FacilityId` cuando se añada, sin romper esta capa.
- **2026-07-22** — `ParameterThreshold` admite `FacilityId` nulo como "umbral global por defecto"; un
  umbral específico de instalación tiene prioridad sobre el global para el mismo parámetro
  (resuelto en `AlertEvaluator.ResolveThreshold`). Permite que una instalación con condiciones
  atípicas (p. ej. agua más cálida a propósito) no dispare falsas alertas.
- **2026-07-22** — `EnvironmentalReading` solo valida corrección *estructural* (pH entre 0-14,
  valores no negativos), no rangos "biológicamente aceptables" — eso es responsabilidad de
  `ParameterThreshold`/`Alert`, no una invariante de la entidad. Una lectura con temperatura
  peligrosamente alta es una medición válida que debe generar una alerta, no una excepción.
- **2026-07-22** — Entidades de dominio con constructor privado sin parámetros (para EF Core) +
  constructor público con invariantes, setters privados, y una clase base `Entity` con igualdad por
  Id. Patrón DDD estándar; evita entidades anémicas y deja las reglas de negocio dentro del dominio.

## 8. Estado actual

**Última sesión:** 2026-07-22

**Hecho:**
- Repositorio git inicializado en `aquatrack/` (rama `main`), 2 commits.
- `.gitignore` creado (.NET + Node + Docker + env files).
- Esqueleto de carpetas para backend (Clean Architecture) y frontend (por feature).
- `CLAUDE.md`, `PROGRESS.md`, `README.md` creados.
- **Backend scaffoldeado y compilando:** `backend/AquaTrack.sln` con
  `AquaTrack.Domain`/`AquaTrack.Application`/`AquaTrack.Infrastructure`/`AquaTrack.Api` (webapi con
  controllers) + 3 proyectos de test xUnit, todos referenciados en la solución y con las referencias
  de proyecto correctas (Application→Domain, Infrastructure→Application, Api→Application+Infrastructure).
  Paquetes base instalados: EF Core 8.0.11 + Npgsql provider + Design (Infrastructure),
  FluentValidation 11.9.2 (Application), JWT Bearer 8.0.11 (Api), Moq (tests unitarios),
  Mvc.Testing (integration tests). Boilerplate de plantilla (WeatherForecast, Class1, UnitTest1)
  eliminado. `dotnet build` verificado sin errores/warnings.
- **Frontend scaffoldeado y compilando:** Vite 8 + React 19 + TypeScript, fusionado dentro de la
  estructura por feature ya existente. Tailwind CSS v4 vía `@tailwindcss/vite`, React Router,
  TanStack Query, Recharts, Axios instalados. Vitest + React Testing Library + jsdom configurados
  (`vite.config.ts` con bloque `test`). `npm run build` y `npm run lint` verificados sin errores.
  `App.tsx`/`index.css` limpiados del contenido de demo de Vite.
- Ningún DbContext, entidad de dominio, controller ni componente de negocio todavía — solo
  estructura y dependencias base (según lo pactado: "antes de escribir la primera línea de lógica
  de negocio").

- **Docker + CI completados (Fase 0 cerrada):** `docker-compose.yml` en la raíz con `postgres`
  (16-alpine, healthcheck), `backend` (Dockerfile multi-stage SDK→aspnet runtime) y `frontend`
  (Dockerfile multi-stage node build→Nginx). `.env.example` con las variables (credenciales de
  Postgres, `JWT_SECRET`, `VITE_API_URL`). Verificado end-to-end: `docker compose build` +
  `docker compose up -d` levanta los 3 servicios y responden por HTTP (frontend 200, backend
  `/swagger` 200, postgres healthy).
- Dos workflows de GitHub Actions (`.github/workflows/backend-ci.yml`,
  `frontend-ci.yml`), cada uno disparado solo por cambios en su carpeta (`paths:`). Backend:
  restore+build+test con .NET 8. Frontend: `npm ci` + lint (oxlint) + `vitest run --passWithNoTests`
  + build.
- **Repo remoto creado y primer push hecho:** https://github.com/RafaGadSan/aquatrack (público,
  vía `gh repo create --source=. --remote=origin`). Los dos workflows de CI corrieron de verdad
  contra el push inicial y terminaron en verde (`Backend CI` ~41s, `Frontend CI` ~19s) — confirma
  que el pipeline no es solo teórico.
- **Modelo de dominio de Fase 1 implementado** en `backend/src/AquaTrack.Domain/`: entidades
  `User`, `Facility`, `EnvironmentalReading`, `ParameterThreshold`, `Alert` (+ `Common/Entity` base
  y `Exceptions/DomainException`), enums `Role`/`FacilityType`/`FacilityStatus`/
  `EnvironmentalParameter`/`AlertStatus`, y el servicio de dominio `Services/AlertEvaluator` que
  calcula qué alertas disparar a partir de una lectura + los umbrales aplicables. 30 unit tests en
  `AquaTrack.Domain.Tests` (invariantes de entidades, límites de `ParameterThreshold`, reglas de
  `AlertEvaluator`), todos en verde tanto en local como en el `Backend CI` del push. Ver §7 para las
  decisiones de diseño (Facility-only en Fase 1, umbrales globales vs. por instalación, validación
  estructural vs. de negocio en `EnvironmentalReading`).
- Todavía sin `DbContext`/persistencia, sin capa `Application` (casos de uso/DTOs) ni controllers —
  el dominio existe pero no hay forma de guardarlo ni exponerlo todavía.

**En qué se está trabajando ahora mismo:**
- Nada en curso. Dominio de Fase 1 listo; siguiente incremento natural es la persistencia (EF Core
  `DbContext` + configuración de entidades + primera migración) o la capa `Application`, según se
  decida al retomar.

**Próximos pasos inmediatos:**
1. Decidir el siguiente incremento de Fase 1: `DbContext` + mapeo EF Core + primera migración
   (para poder persistir lo modelado), o empezar por la capa `Application` (casos de uso de
   auth/CRUD) usando el dominio en memoria/tests primero. Plantear con Rafael antes de empezar.
2. Confirmar la decisión pendiente de nombres de roles en inglés (`Admin`/`ShiftLead`/`Operator`) — ver §7.

**Bloqueos/problemas conocidos:** ninguno.
