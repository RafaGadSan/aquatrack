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
- **2026-07-22** — Matriz de autorización de `Facility` en Fase 1: crear → **Admin**; leer (listar y
  por id) → cualquier rol autenticado; cambiar estado → **Admin, ShiftLead**. Sin endpoint de borrado
  — en este dominio una instalación no se elimina, se pone en estado `Empty` (`ChangeStatus`). Sin
  endpoint de rename en esta rebanada (se añade si hace falta). Es una elección por defecto, no una
  confirmación explícita de Rafael — revisar si el reparto de permisos no encaja al usarlo de verdad.
- **2026-07-22** — Índice único en `Facility.Name`: no existe todavía el concepto de "sitio/planta",
  así que dos instalaciones con el mismo nombre exacto se consideran un error, no un caso legítimo.
  Revisar si en el futuro hace falta permitir nombres repetidos entre sitios distintos.
- **2026-07-22** — La factory de tests de integración (`AquaTrackWebApplicationFactory`) siembra un
  usuario por rol y expone `LoginAsync(client, email)`, para que los tests de autorización hagan
  login real vía `POST /api/auth/login` y ejerciten el pipeline completo de JWT +
  `[Authorize(Roles=...)]`, en vez de fabricar un token a mano. Patrón a reutilizar en las próximas
  rebanadas que necesiten probar permisos.
- **2026-07-22** — Introducido `IUnitOfWork.SaveChangesAsync()` compartido, reemplazando el
  `SaveChangesAsync()` propio de cada repositorio (incluido `IFacilityRepository`, migrado en la
  misma sesión). Motivo: `EnvironmentalReadingService.RecordAsync` coordina tres repositorios
  (lectura + umbrales + alertas) en una sola transacción, y "llama a `SaveChangesAsync` en el
  repositorio X" deja de tener sentido cuando no hay un repositorio "dueño" claro de la operación.
- **2026-07-22** — `POST /api/facilities/{id}/readings` está abierto a **cualquier rol
  autenticado** (no solo Admin/ShiftLead) porque en la operación real es el operario quien toma las
  medidas — coincide con la experiencia real de Rafael en el sector. Configurar umbrales
  (`POST /api/parameter-thresholds`) sigue siendo solo Admin.
- **2026-07-22** — Registrar una lectura devuelve las alertas disparadas **inline** en la respuesta
  (`RecordReadingResult { Reading, TriggeredAlerts }`), no solo como efecto secundario invisible que
  habría que consultar aparte — mejor UX para la demo y para el frontend cuando lo consuma.
- **2026-07-22** — Las entidades `EnvironmentalReading`/`ParameterThreshold`/`Alert` se relacionan
  con `Facility`/`User` solo por `Guid` (FK a nivel de EF vía `HasOne<T>().WithMany()`), sin
  propiedades de navegación en el dominio — coherente con cómo ya estaban diseñadas las entidades de
  dominio (desacopladas entre sí, solo IDs) desde la sesión de modelado inicial.
- **2026-07-22** — Columnas `decimal` con precisión explícita en Postgres: `numeric(6,2)` para
  medidas (temperatura, oxígeno, salinidad, valores de alerta/umbral), `numeric(4,2)` para pH — para
  no depender del mapeo por defecto de Npgsql.
- **2026-07-22** — Primera rebanada de frontend: Auth (login). Sesión de usuario en React Context
  (`AuthContext`, `src/context/`) respaldada por `localStorage` bajo la clave `aquatrack.auth`
  (persiste el `AuthResponse` completo — token, expiración, datos del usuario), no una librería de
  estado global — coherente con el backend, que ya simplificó deliberadamente sin refresh tokens
  (ver entrada anterior sobre JWT de 8h). Un cliente axios único (`src/lib/httpClient.ts`) adjunta el
  Bearer token vía interceptor de request; un interceptor de response detecta 401 y hace un
  `window.location.href = '/login'` (redirect duro, no via React Router) para no acoplar el cliente
  HTTP a la instancia del router. `ProtectedRoute` (`src/routes/`) redirige a `/login` conservando la
  ruta de origen en `location.state.from` para volver ahí tras iniciar sesión.
- **2026-07-22** — Las llamadas a la API viven junto a cada feature (`src/features/<nombre>/api.ts`,
  ej. `features/auth/api.ts`), no en el `src/api/` compartido del esqueleto original — coherente con
  la organización por feature ya elegida para el frontend. `src/api/` queda vacío por ahora; se
  usará si aparece lógica de verdad transversal entre features.
- **2026-07-22** — Segunda rebanada de frontend: Facilities. Se convirtió en la ruta índice (`/`),
  reemplazando la página de bienvenida provisional (`HomePage`, eliminada) — con una sola pantalla
  real todavía no tenía sentido un placeholder intermedio. El control de estado por fila y el
  formulario de alta se muestran u ocultan según `user.role` en el propio componente (no hay
  aún un helper de permisos compartido — se extraerá cuando una tercera pantalla lo necesite,
  evitando abstraer con un solo caso de uso).
- **2026-07-22** — `src/lib/apiError.ts` (`getApiErrorMessage`) centraliza la extracción de mensajes
  de error de la API, porque el backend usa **dos formas distintas** de error (ver
  `ExceptionHandlingMiddleware`): `{ error }` para fallos de negocio (`Result.Failure`, ej. nombre de
  instalación duplicado) y `ProblemDetails { title }` para errores de validación/dominio
  (`FluentValidation`/`DomainException`). Reutilizado por login y por el alta de instalaciones;
  cualquier formulario nuevo que llame a la API debería usarlo también.
- **2026-07-22** — Gotcha real encontrado al verificar contra el backend real (no hipotético): la
  propiedad C# `PH` de `EnvironmentalReadingResponse` se serializa como `"ph"` (todo minúsculas), no
  `"pH"` — la política camelCase por defecto de `System.Text.Json` colapsa una sigla de dos letras a
  una sola minúscula inicial. Los tipos TS (`types/environmentalReading.ts`) y el formulario de
  registro de lecturas usan `ph`, no `pH`, como nombre de propiedad (la etiqueta visible en la UI sí
  sigue diciendo "pH"). Si se añade otro campo con sigla en el dominio, verificar la forma real del
  JSON con `curl` antes de asumir el casing — no fiarse de la intuición aquí.
- **2026-07-22** — Tercera rebanada de frontend: Environmental readings, alerts y thresholds. Página
  de detalle de instalación (`/facilities/:id`, `features/facilities/FacilityDetailPage.tsx`) que
  compone formulario de registro de lectura (`features/environmental-params/`), lista de lecturas
  recientes y lista de alertas de esa instalación (`features/alerts/`). Las alertas disparadas por
  una lectura se muestran **inline** justo tras el registro (vienen ya en la respuesta del POST, ver
  decisión de backend sobre `RecordReadingResult`), sin esperar a un refetch. Pantalla de umbrales
  (`/thresholds`) con alta restringida a Admin (igual que el patrón de Facilities) pero lectura
  abierta a cualquier rol. Con más de una pantalla ya tiene sentido una navegación real en
  `AuthenticatedLayout` (antes solo tenía el header); se añadió con `NavLink`.
- **2026-07-22** — Bug real encontrado en autorevisión de PR (antes de mergear): `RecordReadingForm`
  necesita `key={facility.id}` en `FacilityDetailPage.tsx`. Sin esa key, React Router **no** desmonta
  el componente al navegar entre dos instalaciones que matchean la misma ruta (`/facilities/:id`) —
  el banner de alertas disparadas y los valores del formulario de la instalación anterior quedaban
  visibles en la nueva. El bug solo se manifiesta cuando la instalación destino ya está en cache de
  TanStack Query (revisitada): si es la primera visita, el propio `isLoading` de `useFacility`
  desmonta el árbol igual, enmascarando el problema — por eso el test de regresión
  (`FacilityDetailPage.test.tsx`) visita ambas instalaciones primero para calentar la cache antes de
  reproducir el caso real. Cualquier página de detalle futura con estado local propio (no derivado de
  TanStack Query) debería tener el mismo cuidado con `key`.
- **2026-07-22** — Segundo hallazgo de la misma autorevisión: las keys de TanStack Query de
  `readings` y `alerts` estaban anidadas bajo `['facilities', facilityId, ...]` para agruparlas
  visualmente, pero `invalidateQueries` matchea por **prefijo** por defecto — invalidar
  `['facilities']` (al crear o cambiar el estado de una instalación) invalidaba también las lecturas
  y alertas de **cualquier** instalación montada. Se movieron a namespaces propios (`['readings',
  facilityId]`, `['alerts', facilityId]`), separados del prefijo `'facilities'`; `useFacility(id)`
  se queda intencionalmente bajo ese prefijo porque sí debe refrescarse cuando cambia el estado de
  la instalación. Verificado con un test que falla con la key vieja y pasa con la nueva
  (`queryKeyIsolation.test.tsx`) — no alcanza con "se ve razonable", los invariantes de invalidación
  de cache hay que probarlos.
- **2026-07-22** — Dashboard (cierra el checklist funcional de Fase 1). Backend:
  `GET /api/dashboard/summary` (`DashboardController`/`DashboardService`), cualquier rol autenticado
  — devuelve conteo de instalaciones por estado y las alertas activas de **todas** las instalaciones
  con el nombre de la instalación ya resuelto (`DashboardAlertResponse.FacilityName`), para que el
  frontend no tenga que hacer N llamadas extra. Requirió un método nuevo,
  `IAlertRepository.GetActiveAsync()` (antes solo existía `GetByFacilityIdAsync`, que filtra por una
  instalación). Frontend: el Dashboard pasa a ser la ruta índice (`/`) — más natural como pantalla de
  aterrizaje que la lista de instalaciones — y `Facilities` se mueve a `/facilities` explícito; se
  agregó navegación real (`AuthenticatedLayout`) para las tres secciones. Estadísticas como stat
  tiles simples (sin gráficos: son 3-4 categorías en una foto del momento, no series temporales —
  ver skill `dataviz`, "a veces la respuesta no es un gráfico"); reutiliza los colores de estado que
  ya usaba `FacilitiesPage`/`AlertsList`, sin introducir una paleta nueva. Verificado extremo a
  extremo con `docker compose` + `curl` (umbral→lectura→alerta→dashboard) antes de tocar el frontend,
  seguido del mismo patrón de esta sesión.
- **2026-07-22** — Bug real y serio, encontrado durante el pulido de Fase 1: **el backend no tenía
  CORS configurado**. `curl` y los tests de integración (`HttpClient` de xUnit) nunca lo iban a
  detectar porque ninguno de los dos aplica la política de mismo origen de un navegador real —
  solo apareció al automatizar un navegador headless de verdad (Playwright, instalado sobre la
  marcha en esta sesión porque `chromium-cli` no estaba disponible en este entorno Windows) para
  sacar capturas para el README: el login fallaba con `net::ERR_FAILED` en vez de devolver un error
  de credenciales. Es decir, **la app nunca había funcionado en un navegador real** desde que existe
  frontend, pese a que cada rebanada se había dado por verificada. Arreglado con
  `builder.Services.AddCors(...)` + `app.UseCors("Frontend")` en `Program.cs`, origen configurable
  vía `Cors:AllowedOrigin` (`CORS_ALLOWED_ORIGIN` en `.env`/`docker-compose.yml`, default
  `http://localhost:5173`, coincide con el puerto de Vite/Nginx en ambos casos). Test de regresión
  en `CorsTests.cs` (falla sin `UseCors`, pasa con él). **Lección para el futuro:** `curl`/tests de
  integración verifican que el backend responde correctamente: no verifican que un navegador real
  pueda *llegar* a esa respuesta. Cualquier duda sobre "¿esto funciona en la app de verdad?" necesita
  un navegador de verdad, no solo la API por su cuenta.
- **2026-07-22** — El mismo navegador headless permitió, por primera vez en el proyecto, verificar
  responsive y accesibilidad contra la app real en vez de solo leer el código:
  - **Responsive:** una captura a 390px de ancho mostró el header (`AuthenticatedLayout`)
    desbordándose horizontalmente — nombre de usuario y botón de cerrar sesión cortados. Corregido
    con un layout que apila en columna en mobile y pasa a fila en `sm:`; tablas (`FacilitiesPage`,
    `ReadingsList`, `ThresholdsPage`) envueltas en `overflow-x-auto` para que el contenido ancho
    haga scroll dentro de su propio contenedor en vez de romper el layout de la página.
  - **Accesibilidad:** auditoría con `axe-core` (inyectado en el navegador headless) contra las 5
    pantallas principales encontró 3 problemas reales: contraste insuficiente en `bg-sky-600`/
    `text-sky-600` (oscurecido a `sky-700`/`sky-800` en botones y links, ver contraste WCAG AA),
    `LoginPage` sin landmark `<main>` (todo su contenido quedaba fuera de cualquier región para
    lectores de pantalla), y el `<select>` de cambio de estado por fila en `FacilitiesPage` sin
    nombre accesible (`aria-label="Status for {name}"` — antes un lector de pantalla solo anunciaba
    "combobox" sin decir de qué instalación). 0 violaciones tras corregir los tres.
  - Ninguno de estos dos problemas era visible leyendo el código o los tests — ambos necesitaban una
    verificación *renderizada* (visual para responsive, con `axe-core` para accesibilidad) contra la
    app real corriendo.
- **2026-07-22** — **Despliegue inicial en vivo**: frontend en Vercel
  (https://aquatrack-frontend-iota.vercel.app), backend en Render, base de datos en Neon Postgres —
  el stack exacto que ya estaba decidido en §2. Rafael proveyó las cuentas/credenciales (token de
  Vercel, connection string de Neon, API key de Render) directamente en el chat de esta sesión; no
  se guardaron en ningún archivo del repo ni se commitearon — solo se usaron como variables de
  entorno puntuales en los comandos de deploy, y las de Render/Vercel quedaron guardadas del lado de
  esas plataformas (no en este repo).
  - **Credenciales demo públicas, a propósito**: `admin@aquatrack.dev`/`Admin123!` (y las otras dos)
    están en el código fuente (`DbInitializer.cs`) y ahora también en el README — decisión consciente
    de Rafael para que cualquiera que revise el portfolio pueda loguearse sin pedir acceso. Implica
    que cualquier visitante puede actuar como Admin sobre la base de datos pública (crear/editar
    instalaciones, umbrales, etc.) — aceptable porque son datos ficticios, sin PII ni información de
    negocio real. Sembrada con el mismo set de datos demo usado para las capturas del README (3
    instalaciones, 2 umbrales globales, una alerta activa) para que la demo no se vea vacía al entrar.
  - **Bug real encontrado al verificar el deploy con un navegador real**: Nginx (usado en
    `docker compose`) ya tenía fallback SPA (`try_files ... /index.html`) para que las rutas de React
    Router funcionaran; **Vercel no lo tiene por defecto** — sin `frontend/vercel.json`
    (`rewrites: [{source: "/(.*)", destination: "/index.html"}]`), cualquier ruta que no fuera `/`
    devolvía 404 real en producción. Encontrado con el mismo navegador headless (Playwright) usado
    para el resto del pulido de esta sesión, apuntado a la URL pública en vez de a Docker local —
    otra vez: `curl` a la raíz (`/`) daba 200 y no lo hubiera detectado.
  - Orden de deploy real: 1) backend a Render con el connection string de Neon (las migraciones y el
    seed corren solos al arrancar, mismo mecanismo que en Docker — ver entrada de Program.cs en esta
    sección) — 2) frontend a Vercel con `VITE_API_URL` apuntando a la URL de Render — 3) volver a
    Render para setear `Cors__AllowedOrigin` con la URL final de Vercel (no se puede saber antes de
    que Vercel asigne el dominio) y redeployar. Sin este último paso, el deploy en vivo tendría el
    mismo bug de CORS que se encontró y arregló en local.
  - JWT secret de producción generado nuevo (no reutilizado del `.env` local) — 64 bytes aleatorios,
    solo vive como variable de entorno en Render.
- **2026-07-22** — **UI del frontend traducida al español**, a pedido explícito de Rafael. Alcance:
  todo el texto visible (labels, botones, mensajes de carga/error, encabezados de tabla) en los
  componentes React. **No** se tradujo el código (nombres de variables/funciones/componentes,
  comentarios — sigue la convención de §4). En su momento los mensajes que devuelve el backend
  (FluentValidation, `DomainException`, `Result.Failure`) quedaron sin traducir — ver la entrada
  posterior en esta misma sección, donde Rafael pidió explícitamente cerrar ese hueco y quedó hecho.
  - Los valores de los enums que vienen de la API (`FacilityStatus`, `FacilityType`, `Role`,
    `AlertStatus`, `EnvironmentalParameter`) **no cambiaron** — siguen siendo `'Active'`, `'Admin'`,
    etc., tal como los espera/devuelve el backend. Cada tipo tiene un mapa `*_LABELS` al lado de su
    definición (`FACILITY_STATUS_LABELS`, `ROLE_LABELS`, `ALERT_STATUS_LABELS`, ampliando el patrón
    que ya existía para `ENVIRONMENTAL_PARAMETER_LABELS`) que traduce el valor solo para mostrarlo;
    el dato que viaja a la API nunca se toca.
  - Verificado con navegador real (Playwright) contra el stack dockerizado completo, no solo tests —
    en particular para confirmar que palabras en español más largas que sus equivalentes en inglés
    (p. ej. "Instalaciones" vs. "Facilities") no rompieran el layout responsive que se arregló en la
    sesión anterior. Sin problemas: 0 errores de consola, capturas limpias en desktop y en 390px.
  - Los 17 tests de frontend que hacían `getByLabelText`/`getByRole(name: ...)` sobre texto en inglés
    se actualizaron para buscar el texto en español nuevo — es la razón por la que estos tests son
    tests de integración de UI y no solo de lógica: verifican lo que el usuario realmente ve.
  - Capturas del README (`docs/screenshots/*.png`) regeneradas contra la UI en español para que la
    documentación no quede desactualizada respecto a la app real.
  - Al desplegar esta rama a Vercel apareció un problema de infraestructura no relacionado con la
    traducción: el proyecto de Vercel tenía la integración de GitHub activa (se activó sola al
    correr `vercel link` en la sesión de despliegue anterior) pero **sin `Root Directory`
    configurado** — los deploys manuales por CLI funcionaban porque el comando corría parado dentro
    de `frontend/`, pero los deploys automáticos por PR clonan el monorepo completo y sin
    `rootDirectory: "frontend"` intentaban buildear desde la raíz, donde no hay ningún `package.json`
    (`vite: command not found`, exit 127). Corregido seteando `rootDirectory` vía la API de Vercel
    (`PATCH /v9/projects/aquatrack-frontend`) — queda arreglado para cualquier PR futuro, no solo
    para este.
- **2026-07-22** — **Mensajes del backend traducidos al español**, a pedido explícito de Rafael tras
  la traducción del frontend (quedaban en inglés y podían aparecer mezclados con la UI en español).
  Cubre las tres fuentes de mensajes que le llegan al cliente:
  - **FluentValidation**: en vez de `ValidatorOptions.Global.LanguageManager.Enabled = false`
    (forzaba inglés para no depender del locale del SO — ver entrada de sesión anterior), ahora
    `Enabled = true` + `Culture = new CultureInfo("es")` — **explícito**, no auto-detectado, así que
    mantiene la misma garantía de determinismo que tenía la versión anterior (no depende del locale
    del SO) pero en español en vez de en inglés. Cada `RuleFor` que antes mostraba el nombre de la
    propiedad en inglés (ej. `'PH' must be between 0 and 14.`) ahora tiene `.WithName("...")` en
    español — verificado con `curl` que `.WithName()` se aplica a toda la cadena de validadores de
    esa propiedad, no solo al último.
  - **`DomainException`** (invariantes de las entidades) y **`Result.Failure`** (fallos de negocio
    en los servicios de Application, ej. "instalación no encontrada", "ya existe una instalación
    llamada X") — traducidos directamente, son literales de C#.
  - Mensaje genérico de error 500 en `ExceptionHandlingMiddleware`.
  - **Ningún test dependía del texto exacto de estos mensajes** (solo de `IsSuccess`/status codes),
    así que no hubo que tocar ningún test backend — buen diseño de tests previo, no suerte.
  - Verificado con `curl` contra el backend real para cada una de las tres fuentes (NotEmpty,
    InclusiveBetween, GreaterThanOrEqualTo, `Result.Failure` de login inválido y de nombre
    duplicado) y con Playwright disparando el error real de "instalación duplicada" desde el
    formulario de verdad, confirmando que el mensaje llega intacto hasta el banner de error del
    frontend.
  - Al redesplegar el backend a Render para llevar esto a producción, el trigger de deploy por API
    devolvía `500 internal server error` sin más detalle. Causa real: el servicio de Render seguía
    apuntando a la rama `worktree-frontend-auth-slice` (la del PR #1), que se borró — local y
    remota — al mergear ese PR (ver entrada de esa sesión). Render nunca avisó de esto en el
    dashboard, solo fallaba al intentar un deploy nuevo. Corregido apuntando el servicio a
    `worktree-i18n-spanish-ui` (la rama del PR #2 actual) vía `PATCH /v1/services/{id}` con
    `{"branch": "..."}`. **Lección:** si se borra una rama que un servicio de Render/Vercel tenía
    configurada, hay que actualizar la config del servicio a mano — no se detecta ni se avisa solo.

## 8. Estado actual

**Última sesión:** 2026-07-22

**Resumen del proyecto a día de hoy:** Fase 0 (setup) completa. **Fase 1 MVP está terminada por
completo**: funcionalidad (backend + frontend), pulido (README, responsive, accesibilidad) y
**desplegada en vivo** — https://aquatrack-frontend-iota.vercel.app — ver `PROGRESS.md`. La UI del
frontend está en español (código y mensajes del backend siguen en inglés — ver §7). La app se
verificó por primera vez esta sesión contra un **navegador real** (Playwright headless, no solo
`curl`), tanto en local como contra las URLs públicas ya desplegadas, lo que encontró y corrigió dos
bugs reales que ningún test anterior había detectado: **CORS no estaba configurado** (la app nunca
había funcionado en un navegador real desde que existe frontend) y **Vercel no tenía fallback SPA**
(cualquier ruta que no fuera `/` daba 404 en producción) — ver §7 para ambos. El detalle línea a
línea de qué se tocó en cada sesión vive en el historial de git (`git log --oneline`), no hace falta
repetirlo aquí — esta sección solo recoge el estado y lo que no es obvio a partir del código.

**Hecho hasta ahora (por área):**
- **Repo y CI:** repo público en https://github.com/RafaGadSan/aquatrack, monorepo, `.gitignore`,
  Docker Compose (postgres + backend + frontend) y dos workflows de GitHub Actions
  (`backend-ci.yml`, `frontend-ci.yml`) — ambos verificados en verde contra pushes reales, no solo
  localmente.
- **Frontend — Auth (primera rebanada real):** login (`features/auth/LoginPage.tsx`) contra
  `POST /api/auth/login`, sesión en `AuthContext` respaldada por `localStorage`, cliente axios
  compartido con interceptors de token/401 (`lib/httpClient.ts`), rutas protegidas
  (`routes/ProtectedRoute.tsx`) y layout autenticado con botón de cierre de sesión
  (`layouts/AuthenticatedLayout.tsx`). Ver §7 para las decisiones (por qué Context+localStorage y no
  una librería de estado global, por qué el redirect en 401 es duro y no vía router). Verificado
  contra el backend real: build de producción (`docker compose up`) + `curl` al login confirmando que
  la forma de la respuesta (`token`, `expiresAt`, `userId`, `email`, `fullName`, `role`) coincide con
  los tipos TS, y que `/` y `/login` resuelven bien vía el fallback SPA de Nginx. Verificado más
  tarde en esta misma sesión contra un navegador real (Playwright headless) durante el pulido de
  Fase 1 — ver la entrada del bug de CORS en §7, que hasta ese momento rompía el login para
  cualquier navegador real pese a que esta verificación por `curl` ya daba todo por bueno.
- **Frontend — Facilities (segunda rebanada real, ruta índice `/`):** lista de instalaciones
  (`features/facilities/FacilitiesPage.tsx`) contra `GET /api/facilities`, alta
  (`CreateFacilityForm.tsx`, solo visible para Admin) contra `POST /api/facilities`, y cambio de
  estado inline (select por fila, visible para Admin/ShiftLead) contra
  `PUT /api/facilities/{id}/status` — con invalidación de cache de TanStack Query tras cada mutación.
  Verificado contra el backend real (`docker compose up`): alta, listado, cambio de estado y el
  choque de nombre duplicado (409, forma `{error}`) via `curl`, confirmando que coincide con los
  tipos TS y con `getApiErrorMessage` (ver §7).
- **Frontend — Environmental readings/alerts/thresholds (tercera rebanada real):** página de detalle
  de instalación (`features/facilities/FacilityDetailPage.tsx`, ruta `/facilities/:id`) con
  formulario de registro de lectura, lista de lecturas recientes y lista de alertas de esa
  instalación; pantalla de umbrales (`features/environmental-params/ThresholdsPage.tsx`, ruta
  `/thresholds`) con alta restringida a Admin. Verificado contra el backend real (`docker compose
  up`): flujo completo umbral→lectura→alerta vía `curl`, incluyendo el caso que dispara la alerta.
  Esta verificación encontró y corrigió un bug real antes de mergear — ver la entrada sobre `ph` en
  minúsculas en §7. Verificado contra navegador real en el pulido de Fase 1 (ver CORS en §7).
- **Frontend — Dashboard (cuarta rebanada real, nueva ruta índice `/`):** stat tiles (instalaciones
  por estado, total, alertas activas) y lista de alertas activas de todas las instalaciones con link
  a cada una (`features/dashboard/DashboardPage.tsx`) contra `GET /api/dashboard/summary`. `Facilities`
  se movió a `/facilities` explícito; navegación real en `AuthenticatedLayout` (Dashboard/Facilities/
  Thresholds). Ver §7 para la decisión de stat tiles en vez de gráficos. Con esto el frontend
  funcionalmente ya no tiene nada pendiente de Fase 1 por conectar.
- **Backend — Dashboard:** `GET /api/dashboard/summary` (cualquier rol autenticado) — ver §7.
- **Pulido de Fase 1 (README, CORS, responsive, accesibilidad):** primera verificación de la app
  completa contra un navegador real (Playwright headless, instalado sobre la marcha — ver §7),
  usada para tres cosas: (1) capturas reales para `README.md` (`docs/screenshots/`, no maquetas),
  (2) encontrar y corregir el bug de CORS que rompía el login en cualquier navegador real desde el
  principio del frontend, (3) encontrar y corregir un desborde real de header en mobile y 3
  violaciones de accesibilidad reales vía `axe-core` (contraste, landmark `<main>` faltante,
  `<select>` sin nombre accesible) — 0 violaciones tras corregir. `README.md` reescrito completo
  (problema, capturas, stack, diagrama Mermaid de arquitectura, decisiones técnicas curadas,
  roadmap). Ver §7 para el detalle de cada hallazgo.
- **Backend — Dominio (Fase 1):** entidades `User`, `Facility`, `EnvironmentalReading`,
  `ParameterThreshold`, `Alert` + servicio `AlertEvaluator` (cálculo de alertas). Ver §7 para las
  decisiones de diseño (Facility-only, umbrales por instalación vs. globales, etc.).
- **Backend — Auth:** `POST /api/auth/login` — JWT (HS256, 8h, sin refresh tokens), hashing PBKDF2
  vía ASP.NET Core Identity, seed de 3 usuarios demo (uno por rol — credenciales en
  `DbInitializer.cs`), middleware de errores centralizado.
- **Backend — Facility:** `POST/GET /api/facilities`, `GET /api/facilities/{id}`,
  `PUT /api/facilities/{id}/status`, con autorización por rol (`[Authorize(Roles=...)]` — matriz en §7).
- **Backend — EnvironmentalReading/ParameterThreshold/Alert (rebanada vertical completa, cierra
  Fase 1 MVP del backend):** `POST/GET /api/facilities/{id}/readings`,
  `GET /api/facilities/{id}/alerts`, `POST/GET /api/parameter-thresholds`. Registrar una lectura
  evalúa automáticamente los umbrales aplicables (`AlertEvaluator`, ya existente desde el modelado
  de dominio) y persiste cualquier alerta disparada en la misma transacción
  (`IUnitOfWork` — ver §7). Cualquier rol autenticado puede registrar lecturas (coincide con cómo se
  hace en la operación real); solo Admin configura umbrales.
- **Persistencia:** `AquaTrackDbContext` mapea las 5 entidades de dominio (3 migraciones:
  `InitialCreate`, `AddFacility`, `AddEnvironmentalReadingsThresholdsAndAlerts`).
- **Tests backend:** 75 en total (30 Domain + 26 Application + 19 Api.IntegrationTests — incluye
  `CorsTests.cs`), todos en verde en local y en el `Backend CI` de GitHub tras cada push de esta
  sesión.
- **Tests frontend:** 17 en total — `lib/authStorage.test.ts` y `features/auth/LoginPage.test.tsx`
  (login), `features/facilities/FacilitiesPage.test.tsx` (permisos por rol),
  `features/environmental-params/RecordReadingForm.test.tsx` +
  `features/environmental-params/ThresholdsPage.test.tsx` (alertas disparadas en la respuesta,
  permisos por rol), `features/facilities/FacilityDetailPage.test.tsx` (regresión: el banner de
  alertas no debe persistir al navegar a otra instalación ya cacheada),
  `features/environmental-params/queryKeyIsolation.test.tsx` (regresión: crear una instalación no
  debe refetchear las lecturas de otra — ver §7 para ambos hallazgos), y
  `features/dashboard/DashboardPage.test.tsx` (conteos, link de cada alerta a su instalación, estado
  vacío), con `httpClient` mockeado en todos.
- Las cuatro rebanadas de backend (Auth, Facility, EnvironmentalReading, Dashboard) se verificaron no
  solo con `dotnet test`, sino también contra el stack 100% dockerizado (`docker compose up`)
  hablando con Postgres real — incluyendo el flujo completo umbral→lectura→alerta→dashboard. Lo mismo
  del lado frontend, contra ese mismo backend real (ver entradas de arriba).

**En qué se está trabajando ahora mismo:** nada en curso.

**Próximos pasos inmediatos:**
1. Rafael le da un vistazo manual de todos modos, tanto al PR sin mergear como a la app ya
   desplegada en vivo — esta sesión verificó con un navegador automatizado (Playwright headless:
   login, las 5 pantallas, mobile, `axe-core`, y luego contra las URLs públicas reales), pero eso no
   reemplaza un ojo humano real, sobre todo para cosas de gusto/detalle visual que un test no juzga.
2. Mergear el PR #1 a `main` en algún momento — el deploy en vivo se hizo desde la rama del PR
   (tenía todo el frontend), así que `main` todavía no refleja el estado desplegado. No bloqueante
   para seguir trabajando, pero sí para que el repo y lo que está en producción cuenten la misma
   historia.
3. Fase 1 está completa de punta a punta (funcionalidad, pulido, despliegue). Queda decidir: saltar
   a Fase 2 (turnos/personal, incidencias, alimentación, trazabilidad de lote), o los stretch goals
   de Fase 3.

**Bloqueos/problemas conocidos:** ninguno. Ver §7 para varios gotchas de entorno ya resueltos y
documentados (puerto de Postgres, longitud mínima del secreto JWT, timing de `IOptions`) para que no
se repitan si se tocan esas zonas otra vez.
