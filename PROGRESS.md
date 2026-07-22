# PROGRESS.md — AquaTrack

Checklist de avance por fase. Se actualiza en cada sesión (ver `CLAUDE.md` sección "Cómo trabajamos").

## Fase 0 — Setup del proyecto

- [x] Inicializar repositorio git (monorepo)
- [x] Crear `CLAUDE.md` y `PROGRESS.md`
- [x] Definir estructura de carpetas frontend/backend
- [x] Scaffold del proyecto .NET (solution + proyectos por capa + proyectos de test)
- [x] Scaffold del proyecto Vite + React + TS + Tailwind
- [x] `docker-compose.yml` con Postgres para desarrollo local
- [x] Pipeline base de GitHub Actions (lint + build, sin despliegue todavía)

## Fase 1 — MVP

- [x] Modelo de dominio inicial (entidades `User`, `Facility`, `EnvironmentalReading`,
      `ParameterThreshold`, `Alert` + lógica de cálculo de alertas `AlertEvaluator`, con 30 unit tests)
- [x] Autenticación JWT (solo Login; sin registro público — ver `CLAUDE.md` §7) + roles (Admin, ShiftLead, Operator)
- [x] CRUD de instalaciones (jaulas/tanques), con estado (activo, en cosecha, vacío) — API con roles
      (crear: Admin; cambiar estado: Admin/ShiftLead; leer: cualquiera). Frontend en `/facilities`.
- [x] Registro de parámetros ambientales (temperatura, oxígeno disuelto, salinidad, pH) por instalación
      — API `POST/GET /api/facilities/{id}/readings`, cualquier rol autenticado. Frontend en la página
      de detalle de instalación (`/facilities/:id`).
- [x] Alertas automáticas cuando un parámetro sale de rango configurable — `POST /api/parameter-thresholds`
      (Admin) + `GET /api/facilities/{id}/alerts`. Verificado extremo a extremo con Postgres real.
      Frontend: umbrales en `/thresholds`, alertas en la página de detalle de instalación.
- [x] Dashboard mínimo funcional (alertas activas, resumen básico) — `GET /api/dashboard/summary`
      (instalaciones por estado, alertas activas con nombre de instalación resuelto) + frontend en `/`
      (nueva página de inicio; Facilities se movió a `/facilities`).
- [x] Seed data con datos de ejemplo realistas (3 usuarios demo, uno por rol — se irá ampliando por slice)
- [ ] Despliegue inicial (backend + frontend + DB) con datos de ejemplo

## Fase 2 — Funcionalidad completa

- [ ] Gestión de turnos y personal (asignación de turnos, roles/responsables)
- [ ] Gestión de incidencias (prioridad, estado abierta/en curso/resuelta, asignación a usuario)
- [ ] Registro de alimentación por lote/día + cálculo de consumo acumulado
- [ ] Trazabilidad completa del lote (siembra → cosecha/venta, eventos históricos)
- [ ] Gráficos avanzados en el dashboard (series temporales de parámetros, producción)
- [ ] Gestión de usuarios (solo Admin)

## Fase 3 — Pulido (stretch goals, opcional)

- [ ] Exportación de informes en PDF
- [ ] Simulación de datos en tiempo real vía WebSockets (sensores IoT simulados)
- [ ] Notificaciones por email ante alertas críticas
- [ ] Modo demo público con datos ficticios, sin necesidad de registro

## Calidad / no funcionales (transversal, revisar en cada fase)

- [x] Arquitectura en capas en backend (Domain/Application/Infrastructure/Api)
- [x] Validación de inputs en frontend y backend (FluentValidation + validación nativa de formularios,
      `getApiErrorMessage` para mostrar errores del backend)
- [x] Middleware de manejo de errores centralizado
- [x] Unit tests en lógica de negocio crítica (alertas, `AlertEvaluator`)
- [x] Integration tests en endpoints principales
- [ ] README profesional (problema, capturas, stack, instrucciones locales, diagrama Mermaid, decisiones técnicas, roadmap)
- [ ] Diseño responsive (especial cuidado en dashboard móvil) — no verificado en viewport móvil todavía
- [ ] Accesibilidad básica (contraste, labels, navegación por teclado) — labels/htmlFor sí, resto sin auditar
- [x] CI en GitHub Actions corriendo lint + tests en cada push/PR
