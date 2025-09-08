# PetBoarding - Application de Pension pour Animaux

## Vue d'ensemble métier

PetBoarding est une plateforme complète de gestion de pension pour animaux de compagnie qui permet :

- **Gestion des propriétaires** : Inscription, authentification et gestion des profils utilisateurs
- **Gestion des animaux** : Enregistrement des informations détaillées (race, âge, vaccinations, besoins spéciaux)
- **Catalogue de prestations** : Services de pension, toilettage, promenades, soins vétérinaires
- **Système de réservation** : Booking en ligne avec gestion des disponibilités et calendrier
- **Suivi des vaccinations** : Vérification et rappels automatiques pour les vaccinations obligatoires
- **Panier et facturation** : Système de commande avec gestion des prix et facturation

L'application s'adresse aux propriétaires d'animaux souhaitant confier leurs compagnons pendant leurs absences, ainsi qu'aux professionnels gérant des établissements de pension.

## Architecture technique

### Stack technologique

**Backend (.NET 9)**

- Architecture Clean Architecture avec CQRS
- APIs minimales avec pattern d'endpoint mapping
- Entity Framework Core avec PostgreSQL
- Authentification JWT avec refresh tokens
- Tests d'architecture avec NetArchTest
- Logging structuré avec Serilog et Seq
- Tracing distribué avec OpenTelemetry et Jaeger
- TaskWorker avec Quartz.NET pour les tâches planifiées

**Frontend (Angular 19)**

- Architecture standalone components (sans NgModules)
- Gestion d'état avec Angular Signals
- Angular Material + Bootstrap 5.3.7
- TypeScript strict mode

**Base de données**

- PostgreSQL avec conteneurisation Docker
- Migrations Entity Framework automatiques
- Seeding de données de test

### Structure du projet

```
PetBoarding/
├── Core_PetBoarding_Backend/          # Backend .NET
│   ├── PetBoarding_Api/               # Couche présentation (APIs)
│   ├── PetBoarding_Domain/            # Logique métier core
│   ├── PetBoarding_Application/       # Couche application (CQRS)
│   ├── PetBoarding_Infrastructure/    # Services externes
│   ├── PetBoarding_Persistence/       # Accès aux données
│   ├── PetBoarding_TaskWorker/        # Service de tâches planifiées
│   └── Tests/                         # Tests organisés par type
│       ├── ArchitectureTests/         # Tests d'architecture (NetArchTest)
│       ├── UnitTests/                 # Tests unitaires par couche
│       │   ├── DomainUnitTests/       # Tests unitaires domaine
│       │   └── InfrastructureUnitTests/ # Tests unitaires infrastructure
│       └── IntegrationTests/          # Tests d'intégration
│           └── PersistenceIntegrationTests/ # Tests d'intégration persistance
├── ng_PetBoarding_app/                # Frontend Angular
│   ├── src/app/features/              # Modules métier
│   ├── src/app/shared/                # Composants partagés
│   └── src/app/core/                  # Services core
└── docker-compose.yml                 # Configuration Docker
```

## Démarrage avec Docker (Recommandé)

### Prérequis

- [Docker]
- [Git]

### Installation rapide

1. **Cloner le projet**

```bash
git clone <repository-url>
cd PetBoarding
```

2. **Lancer l'environnement backend complet**

```bash
cd Core_PetBoarding_Backend
docker-compose up --build
```

Cette commande démarre :

- API Backend sur `https://localhost:5001`
- Base de données PostgreSQL sur `localhost:5432`
- Documentation Swagger sur `https://localhost:5001/swagger`
- TaskWorker pour les tâches planifiées (service en arrière-plan)
- Memcached sur `localhost:11211`
- RabbitMQ sur `localhost:5672` (Management UI: `localhost:15672`)
- Jaeger UI pour les traces sur `localhost:16686`
- Seq pour les logs structurés sur `localhost:5341`

### Démarrage du frontend Angular

```bash
cd ng_PetBoarding_app
npm install
npm start
```

L'application sera accessible sur `http://localhost:4200`

### Configuration de la base de données

Les paramètres de connexion par défaut :

- **Host** : `localhost:5432`
- **Database** : `petboarding`
- **Username** : `postgres`
- **Password** : `postgres`

## Développement local (sans Docker)

### Backend (.NET 9)

```bash
cd Core_PetBoarding_Backend
dotnet build                    # Build solution
cd PetBoarding_Api
dotnet run                      # Démarrer l'API
```

### Frontend (Angular 19)

```bash
cd ng_PetBoarding_app
npm install                     # Installer dépendances
ng serve                        # Serveur de développement
npm run build                   # Build production
```

### Base de données (Entity Framework comme ORM)

```bash
# Créer une migration
dotnet ef migrations add <NomMigration> --project PetBoarding_Persistence --startup-project PetBoarding_Api

# Appliquer les migrations
dotnet ef database update --project PetBoarding_Persistence --startup-project PetBoarding_Api
```

## Endpoints principaux

- **Authentication** : `/api/auth/*` (login, refresh)
- **Users** : `/api/users/*`
- **Pets** : `/api/pets/*`
- **Prestations** : `/api/prestations/*`
- **Réservations** : `/api/reservations/*`
- **Documentation** : `/swagger` (environnement dev)

## Tests

### Tests Backend (.NET)

```bash
# Tous les tests backend
cd Core_PetBoarding_Backend
dotnet test

# Tests par catégorie
dotnet test Tests/ArchitectureTests/                        # Tests d'architecture (NetArchTest)
dotnet test Tests/UnitTests/DomainUnitTests/                # Tests unitaires domaine
dotnet test Tests/UnitTests/InfrastructureUnitTests/        # Tests unitaires infrastructure
dotnet test Tests/IntegrationTests/PersistenceIntegrationTests/  # Tests d'intégration persistance
```

## Observabilité (Logs et Traces)

### Logging structuré avec Serilog et Seq

- **Serilog** : Library de logging structuré avec enrichisseurs (assemblage, environnement, machine, processus)
- **Seq** : Interface web pour consulter et analyser les logs en temps réel
- **Configuration** : Logs console + Seq via variable d'environnement `SEQ_URL`
- **Accès Seq** : http://localhost:5341 (admin/petboarding123)

### Tracing distribué avec OpenTelemetry et Jaeger

- **OpenTelemetry** : Instrumentation automatique ASP.NET Core, HttpClient, Entity Framework
- **Jaeger** : Interface de visualisation des traces distribuées
- **Configuration** : Export OTLP via variable d'environnement `JAEGER_ENDPOINT`
- **Accès Jaeger** : http://localhost:16686
- **Services tracés** : PetBoarding.Api, PetBoarding.TaskWorker

### Variables d'environnement

```bash
JAEGER_ENDPOINT=http://jaeger:4317    # Endpoint Jaeger OTLP
SEQ_URL=http://seq:5341               # URL Seq pour logs
OTEL_SERVICE_NAME=PetBoarding.Api     # Nom du service pour les traces
```

## Authentification

Le système utilise JWT avec refresh tokens :

- Tokens d'accès (1h d'expiration)
- Refresh automatique via interceptors
- Autorisation basée sur les permissions/claims
