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
- [ ] Despliegue inicial (backend + frontend + DB) con datos de ejemplo — requiere cuentas reales en
      Railway/Render/Vercel/Neon (ver stack en §2 de `CLAUDE.md`) que esta sesión no tiene; el resto
      de Fase 1 está listo para cuando eso se resuelva.

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
- [x] README profesional (problema, capturas reales de la app corriendo, stack, diagrama Mermaid,
      decisiones técnicas, roadmap) — capturas tomadas con Playwright headless contra el stack
      dockerizado real, no maquetas.
- [x] Diseño responsive (especial cuidado en dashboard móvil) — verificado con capturas reales en
      viewport 390px (no solo revisión de código). Encontró y corrigió un desborde horizontal real
      en el header (`AuthenticatedLayout`) que cortaba el nombre de usuario y el botón de cerrar
      sesión; tablas envueltas en `overflow-x-auto`. Ver `CLAUDE.md` §7.
- [x] Accesibilidad básica (contraste, labels, navegación por teclado) — auditado con `axe-core`
      contra las 5 pantallas principales (no solo revisión visual): 0 violaciones tras corregir
      3 hallazgos reales (contraste insuficiente en `sky-600`, falta de landmark `<main>` en login,
      `<select>` de cambio de estado sin nombre accesible). Ver `CLAUDE.md` §7.
- [x] CI en GitHub Actions corriendo lint + tests en cada push/PR
