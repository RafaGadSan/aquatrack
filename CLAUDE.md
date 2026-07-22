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
  español. Ya implementado (migración + seed data), no solo propuesto.
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
- **2026-07-22** — Rebanada vertical de Auth: **solo Login en Fase 1**, sin endpoint público de
  registro. Confirmado con Rafael: la gestión de usuarios es explícitamente un ítem de Fase 2
  ("solo Admin"), y encaja mejor con el dominio (herramienta operativa interna, no alta pública).
  Los usuarios de Fase 1 vienen de `DbInitializer` (seed, un usuario por rol).
- **2026-07-22** — `AuthService.LoginAsync` devuelve `Result<AuthResponse>` (éxito/fallo con
  mensaje) en vez de lanzar una excepción para "credenciales inválidas": es un resultado de negocio
  esperado, no una violación de invariante. Se reserva `DomainException` para invariantes reales.
  `Application/Common/Result.cs` es un patrón reutilizable pensado para las próximas rebanadas.
- **2026-07-22** — Hash de contraseñas vía `Microsoft.AspNetCore.Identity.PasswordHasher<User>`
  (PBKDF2, del paquete `Microsoft.Extensions.Identity.Core`) en vez de implementar hashing a mano.
  JWT vía `System.IdentityModel.Tokens.Jwt`, firma HS256, expiración de 8h (duración de un turno) y
  **sin refresh tokens** en el MVP — simplificación deliberada; revisar si hace falta rotación de
  tokens más adelante.
- **2026-07-22** — `AquaTrackDbContext` solo mapea `User` por ahora. Las demás entidades de dominio
  (`Facility`, `EnvironmentalReading`, `ParameterThreshold`, `Alert`) ya existen en `Domain` pero se
  mapean/migran en sus propias rebanadas verticales, no todas de golpe.
- **2026-07-22** — Migraciones EF Core y seed de datos demo se ejecutan automáticamente al arrancar
  la Api (`Program.cs`, antes de `app.Run()`). Simplificación deliberada para una demo de portfolio
  de una sola instancia — en un despliegue real esto sería un paso de release separado, no algo que
  ocurra en cada arranque. Este bloque se **salta bajo el entorno `"Testing"`** para que los tests de
  integración puedan sustituir su propio DbContext (SQLite) sin que esto compita con una migración
  real de Postgres.
- **2026-07-22** — `JwtSettings` se resuelve vía `IOptions<JwtSettings>` dentro de un
  `.Configure<IOptions<JwtSettings>>(...)` perezoso, **no** leyendo `builder.Configuration` en una
  variable local antes de `builder.Build()`. Motivo (bug real encontrado y corregido): una lectura
  anticipada no ve las fuentes de configuración que `WebApplicationFactory` añade para los tests de
  integración, que se componen más tarde en el pipeline de construcción del host — con la lectura
  anticipada, el secreto JWT llegaba vacío solo en tests, dando un 500 críptico. La forma perezosa
  es además más correcta en general, no solo un parche para tests.
- **2026-07-22** — Validación de arranque: `Jwt:Secret` debe tener ≥32 bytes (256 bits, mínimo de
  HS256); si no, la app falla al arrancar con un mensaje claro (`ValidateOnStart`) en vez de un 500
  la primera vez que alguien hace login. El `.env.example` original tenía un secreto de 26
  caracteres — se corrigió a uno más largo.
- **2026-07-22** — Puerto host de Postgres en `docker-compose.yml` remapeado de 5432 a **5433**
  (`POSTGRES_HOST_PORT`, con default). Motivo (encontrado en esta misma máquina): un Postgres nativo
  (no Docker) ya escuchaba en 5432, y una API corriendo localmente con `dotnet run` se conectaba al
  Postgres equivocado sin ningún error de Docker — solo un fallo de autenticación confuso. El
  contenedor sigue siendo accesible como `postgres:5432` en la red interna de Docker; solo cambia el
  mapeo de puerto del host.
- **2026-07-22** — Enums serializados como string en JSON (`JsonStringEnumConverter` global en
  `Program.cs`), no como el entero por defecto — un contrato de API con `"role": 0` es frágil ante
  reordenar el enum. Los clientes de test deben registrar el mismo converter para deserializar.
- **2026-07-22** — Mensajes de validación de FluentValidation forzados a inglés
  (`ValidatorOptions.Global.LanguageManager.Enabled = false`), porque por defecto siguen la cultura
  del SO — en esta máquina (Windows en español) salían en español, pero en el contenedor Linux
  desplegado saldrían en inglés. Sin esto, el comportamiento de validación depende silenciosamente
  de dónde se ejecuta.

## 8. Estado actual

**Última sesión:** 2026-07-22

**Resumen del proyecto a día de hoy:** Fase 0 (setup) completa. Del checklist de Fase 1 MVP están
hechos el modelo de dominio y la rebanada vertical de Auth (login); quedan CRUD de instalaciones,
parámetros ambientales, alertas y dashboard. El detalle línea a línea de qué se tocó en cada sesión
vive en el historial de git (`git log --oneline`), no hace falta repetirlo aquí — esta sección solo
recoge el estado y lo que no es obvio a partir del código.

**Hecho hasta ahora (por área):**
- **Repo y CI:** repo público en https://github.com/RafaGadSan/aquatrack, monorepo, `.gitignore`,
  Docker Compose (postgres + backend + frontend) y dos workflows de GitHub Actions
  (`backend-ci.yml`, `frontend-ci.yml`) — ambos verificados en verde contra pushes reales, no solo
  localmente.
- **Frontend:** scaffold Vite 8 + React 19 + TS + Tailwind v4 + React Router + TanStack Query +
  Recharts + Vitest, sin features de negocio todavía.
- **Backend — Dominio (Fase 1):** entidades `User`, `Facility`, `EnvironmentalReading`,
  `ParameterThreshold`, `Alert` + servicio `AlertEvaluator` (cálculo de alertas). 30 unit tests. Ver
  §7 para las decisiones de diseño (Facility-only, umbrales por instalación vs. globales, etc.).
- **Backend — Auth (Fase 1, rebanada vertical completa):** `POST /api/auth/login` funcionando de
  extremo a extremo — probado tanto con `dotnet run` local contra Postgres dockerizado como con el
  stack 100% dockerizado (`docker compose up`). Incluye: JWT (HS256, 8h, sin refresh tokens),
  hashing de contraseñas (PBKDF2 vía ASP.NET Core Identity), `AquaTrackDbContext` con la primera
  migración (`InitialCreate`, solo tabla `Users`), seed de 3 usuarios demo (uno por rol — ver
  credenciales en `DbInitializer.cs`), middleware de errores centralizado, y 11 tests (8 unit con
  Moq + 3 integration con `WebApplicationFactory`+SQLite in-memory). Todavía **sin registro
  público** (decisión confirmada, ver §7) — los usuarios solo existen vía seed.
- Ningún endpoint más allá de Auth (Facility, EnvironmentalReading, Alert no tienen DbContext
  mapping, repos, servicios de Application ni controllers todavía).

**En qué se está trabajando ahora mismo:** nada en curso.

**Próximos pasos inmediatos:**
1. Siguiente rebanada vertical natural: CRUD de `Facility` (Application + Infrastructure/EF mapping
   + migración + Api controller, protegido con `[Authorize]` ahora que la auth existe) — sería el
   primer endpoint que realmente ejercita los roles (p. ej. solo Admin/ShiftLead pueden crear).
2. Cuando llegue esa rebanada: decidir cómo probar autorización por rol (tests de integración con un
   token generado a medida, no solo login).

**Bloqueos/problemas conocidos:** ninguno. Ver §7 para varios gotchas de entorno ya resueltos y
documentados (puerto de Postgres, longitud mínima del secreto JWT, timing de `IOptions`) para que no
se repitan si se tocan esas zonas otra vez.
