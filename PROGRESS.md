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
- [ ] Autenticación JWT + roles (Admin, JefeDeTurno, Operario)
- [ ] CRUD de instalaciones/lotes de cultivo (jaulas/tanques), con estado (activo, en cosecha, vacío)
- [ ] Registro de parámetros ambientales (temperatura, oxígeno disuelto, salinidad, pH) por instalación
- [ ] Alertas automáticas cuando un parámetro sale de rango configurable
- [ ] Dashboard mínimo funcional (alertas activas, resumen básico)
- [ ] Seed data con datos de ejemplo realistas
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

- [ ] Arquitectura en capas en backend (Domain/Application/Infrastructure/Api)
- [ ] Validación de inputs en frontend y backend
- [ ] Middleware de manejo de errores centralizado
- [ ] Unit tests en lógica de negocio crítica (alertas, trazabilidad)
- [ ] Integration tests en endpoints principales
- [ ] README profesional (problema, capturas, stack, instrucciones locales, diagrama Mermaid, decisiones técnicas, roadmap)
- [ ] Diseño responsive (especial cuidado en dashboard móvil)
- [ ] Accesibilidad básica (contraste, labels, navegación por teclado)
- [ ] CI en GitHub Actions corriendo lint + tests en cada push/PR
