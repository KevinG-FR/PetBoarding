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
│   └── Tests/ArchitectureTests/       # Tests d'architecture
├── ng_PetBoarding_app/                # Frontend Angular
│   ├── src/app/features/              # Modules métier
│   ├── src/app/shared/                # Composants partagés
│   └── src/app/core/                  # Services core
└── docker-compose.yml                 # Configuration Docker
```

## Démarrage avec Docker (Recommandé)

### Prérequis

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

### Installation rapide

1. **Cloner le projet**

```bash
git clone <repository-url>
cd PetBoarding
```

2. **Lancer l'environnement complet**

```bash
cd Core_PetBoarding_Backend
docker-compose up --build
```

Cette commande démarre :

- API Backend sur `https://localhost:5001`
- Base de données PostgreSQL sur `localhost:5432`
- Documentation Swagger sur `https://localhost:5001/swagger`

### Configuration de la base de données

Les paramètres de connexion par défaut :

- **Host** : `localhost:5432`
- **Database** : `petboarding`
- **Username** : `postgres`
- **Password** : `postgres`

### Démarrage du frontend Angular

```bash
cd ng_PetBoarding_app
npm install
npm start
```

L'application sera accessible sur `http://localhost:4200`

## Développement local (sans Docker)

### Backend (.NET)

```bash
cd Core_PetBoarding_Backend
dotnet build                    # Build solution
cd PetBoarding_Api
dotnet run                      # Démarrer l'API
```

### Frontend (Angular)

```bash
cd ng_PetBoarding_app
npm install                     # Installer dépendances
ng serve                        # Serveur de développement
npm run lint                    # Linter + Prettier
npm run build                   # Build production
```

### Base de données (Entity Framework)

```bash
# Créer une migration
dotnet ef migrations add <NomMigration> --project PetBoarding_Persistence --startup-project PetBoarding_Api

# Appliquer les migrations
dotnet ef database update --project PetBoarding_Persistence --startup-project PetBoarding_Api
```

## Endpoints principaux

- **Authentication** : `/api/auth/*` (login, refresh)
- **Users** : `/api/users/*`
- **Prestations** : `/api/prestations/*`
- **Réservations** : `/api/reservations/*`
- **Documentation** : `/swagger` (environnement dev)

## Tests

```bash
# Tests d'architecture
cd Core_PetBoarding_Backend
dotnet test Tests/ArchitectureTests/

# Tests frontend
cd ng_PetBoarding_app
ng test
```

## Authentification

Le système utilise JWT avec refresh tokens :

- Tokens d'accès (1h d'expiration)
- Refresh automatique via interceptors
- Autorisation basée sur les permissions/claims
