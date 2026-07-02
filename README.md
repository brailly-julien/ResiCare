# ResiCare

[![CI](https://github.com/brailly-julien/ResiCare/actions/workflows/ci.yml/badge.svg)](https://github.com/brailly-julien/ResiCare/actions/workflows/ci.yml)

Mini-système de gestion pour établissement médico-social (EMS) : suivi des
résidents, transmissions/observations et tâches de soin.

> Projet portfolio full-stack **Angular + .NET 10** en **Clean Architecture**,
> conçu pour démontrer concrètement chaque compétence d'une offre ciblée.

## Démo

**En local, en une commande** — une seule image Docker auto-suffisante : l'API sert le front
Angular (même origine, aucun CORS) avec une base **SQLite semée au démarrage** (rien à installer).

```bash
docker build -t resicare .
docker run --rm -p 8080:8080 -e Jwt__Key="une-cle-d-au-moins-32-caracteres" resicare
# puis http://localhost:8080  — comptes de démo affichés sur la page de login
```

Comptes : `marie.curie@resicare.local` / `Manager123!` (responsable) ·
`paul.durand@resicare.local` / `Soignant123!` (soignant).

## Stack technique

| Domaine            | Choix                                                      |
| ------------------ | ---------------------------------------------------------- |
| Back-end           | .NET 10, ASP.NET Core Web API, C# 14                       |
| Accès aux données  | Entity Framework Core 10, Microsoft SQL Server             |
| Architecture       | Clean Architecture (4 couches) + CQRS                      |
| Validation         | FluentValidation                                           |
| Authentification   | JWT (Bearer) · rôles soignant/responsable · hachage PBKDF2 |
| Tests              | Back : xUnit + FluentAssertions · Front : Vitest           |
| Front              | Angular (standalone, signals, formulaires typés), Material |
| Outillage          | Swagger/OpenAPI, Docker Compose, GitHub Actions (CI)       |

## Architecture

Découpage en 4 couches. **Règle d'or : les dépendances pointent vers
l'intérieur.** Le Domaine ne connaît personne ; l'API connaît tout le monde.

```
            ┌──────────────────────────────────────────┐
            │                  Api                     │  ASP.NET Core, endpoints,
            │   (point d'entrée + composition root)    │  injection de dépendances
            │   ┌──────────────────────────────────┐   │
            │   │          Infrastructure          │   │  EF Core, SQL Server,
            │   │   (EF Core, repositories, SQL)   │   │  migrations, repositories
            │   │   ┌──────────────────────────┐   │   │
            │   │   │       Application        │   │   │  CQRS (commandes/requêtes),
            │   │   │   (cas d'usage, CQRS)    │   │   │  DTOs, interfaces, validation
            │   │   │   ┌──────────────────┐   │   │   │
            │   │   │   │     Domain       │   │   │   │  entités, enums, règles
            │   │   │   │  (cœur métier)   │   │   │   │  métier — AUCUNE dépendance
            │   │   │   └──────────────────┘   │   │   │
            │   │   └──────────────────────────┘   │   │
            │   └──────────────────────────────────┘   │
            └──────────────────────────────────────────┘
                  Les flèches de dépendance vont
                  TOUJOURS vers le centre (Domain).
```

| Projet                   | Rôle                                                          | Dépend de            |
| ------------------------ | ------------------------------------------------------------ | -------------------- |
| `ResiCare.Domain`        | Entités, enums, règles & exceptions métier                   | *(rien)*             |
| `ResiCare.Application`   | CQRS (commandes/requêtes + handlers), DTOs, abstractions     | Domain               |
| `ResiCare.Infrastructure`| EF Core, SQL Server, migrations, repositories                | Application, Domain  |
| `ResiCare.Api`           | Web API, endpoints, injection de dépendances                 | Application, Infra.  |

## État d'avancement

- [x] **Jalon 0** — Solution à 4 projets, références entre couches
- [x] **Jalon 1** — Domaine ✅ · EF Core (DbContext, Fluent API, migration `InitialCreate`) ✅ · seed de démo ✅ · SQL Server via Docker ✅
- [x] **Jalon 2** — CQRS résidents complet : dispatcher maison, validation, erreurs (ProblemDetails), 5 endpoints (CRUD + archive + tableau de bord), UI Scalar
- [x] **Jalon 3** — Observations & tâches : ajout/liste d'observations (règle « archivé » → 409), planification/complétion de tâches (règle « complétion unique » → 409)
- [x] **Jalon 4** — Tests xUnit + FluentAssertions : 27 tests verts (19 Domaine + 8 handlers via SQLite in-memory)
- [x] **Jalon 5** — Front **Angular 22** (standalone, signals, zoneless), Material M3, **mobile-first** : liste+recherche, tableau de bord (complétion de tâche + ajout d'observation), formulaires create/edit typés, routes lazy
- [x] **Bonus** (entre Jalon 5 et 6) — Enrichissement métier ✅ · statistiques ✅ · tests d'intégration API (WebApplicationFactory) ✅ · pagination + filtrage des observations ✅ · export PDF (QuestPDF) ✅ · **Auth JWT + rôles** ✅
- [x] **Niveau 2** (optionnel) — Pointage des soignants ✅ · Planning / affectations ✅ · Prescriptions + posologie ✅
- [x] **Jalon 6** — Dépôt Git ✅ · secrets en `user-secrets` ✅ · CI GitHub Actions (build + tests back & front) ✅

## Authentification (comptes de démo)

L'API est **sécurisée par défaut** : tout endpoint exige un jeton JWT, sauf `POST /api/auth/login`.
Connecte-toi sur `/login` avec un compte injecté par le seed :

| Rôle        | Email                          | Mot de passe   | Peut en plus…                        |
| ----------- | ------------------------------ | -------------- | ------------------------------------ |
| Responsable | `marie.curie@resicare.local`   | `Manager123!`  | créer/modifier/archiver, planifier   |
| Soignant    | `paul.durand@resicare.local`   | `Soignant123!` | consulter, transmettre, valider tâches |

> ⚠️ Le schéma a changé (colonnes `Email`/`PasswordHash` + index unique). Avant le prochain
> `dotnet run`, **réinitialiser la base** : `docker compose down -v && docker compose up -d`.

## Démarrer

Prérequis : [SDK .NET 10](https://dotnet.microsoft.com/download), [Node ≥ 20](https://nodejs.org/),
Docker (pour SQL Server).

**1. Secrets** (chaîne de connexion + clé JWT — volontairement absentes de `appsettings.json`) :

```bash
dotnet user-secrets init --project src/ResiCare.Api
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1433;Database=ResiCare;User Id=sa;Password=ResiCare!Dev2026;TrustServerCertificate=True" --project src/ResiCare.Api
dotnet user-secrets set "Jwt:Key" "une-cle-d-au-moins-32-caracteres-pour-hmac-sha256" --project src/ResiCare.Api
```

**2. Base de données + back-end :**

```bash
docker compose up -d                    # démarre SQL Server (mot de passe = celui fourni ci-dessus)
dotnet build ResiCare.slnx              # compiler toute la solution
dotnet run --project src/ResiCare.Api   # http://localhost:5045 (auto-migrate + seed en dev)
```

**3. Front-end :**

```bash
npm --prefix resicare-web install
npm --prefix resicare-web start         # http://localhost:4200 (proxy /api -> :5045)
```

Comptes de démo : voir la section [Authentification](#authentification-comptes-de-démo) ci-dessus.
