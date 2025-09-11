# Guide d'installation complet - Projet PetBoarding

---

## Table des matières

1. [Prérequis techniques](#prérequis-techniques)
2. [Guide d'installation Windows](#guide-dinstallation-windows)
   - [Installation de Git](#1-installation-de-git)
   - [Installation du .NET 9.0 SDK](#2-installation-du-net-90-sdk)
   - [Installation de Node.js](#3-installation-de-nodejs)
   - [Installation de Docker Desktop](#4-installation-de-docker-desktop)
   - [Installation d'Angular CLI](#5-installation-dangular-cli)
   - [Configuration des certificats HTTPS](#6-configuration-des-certificats-https-windows)
3. [Guide d'installation macOS](#guide-dinstallation-macos)
   - [Installation de Git](#1-installation-de-git-1)
   - [Installation du .NET 9.0 SDK](#2-installation-du-net-90-sdk-1)
   - [Installation de Node.js](#3-installation-de-nodejs-1)
   - [Installation de Docker Desktop](#4-installation-de-docker-desktop-1)
   - [Installation d'Angular CLI](#5-installation-dangular-cli-1)
   - [Configuration des certificats HTTPS](#6-configuration-des-certificats-https-macos)
4. [Démarrage du projet](#démarrage-du-projet)
   - [Clonage du repository](#1-clonage-du-repository)
   - [Démarrage du Backend avec Docker](#2-démarrage-du-backend-avec-docker)
   - [Démarrage du Frontend Angular](#3-démarrage-du-frontend-angular-nouveau-terminal)
   - [Vérification finale](#4-vérification-finale)
5. [Commandes de développement](#commandes-de-développement)
   - [Backend](#backend)
   - [Frontend](#frontend)
6. [Dépannage courant](#dépannage-courant)
7. [Informations complémentaires](#informations-complémentaires)

---

## PRÉREQUIS TECHNIQUES

**Backend :**

- .NET 9.0 SDK
- Docker Desktop
- Git

**Frontend :**

- Node.js 20.x LTS ou 22.x LTS
- npm (inclus avec Node.js)
- Angular CLI 19.1.0

**Services (via Docker) :**

- PostgreSQL 17 (base de données)
- RabbitMQ 3.13.7 (messaging)
- Memcached 1.6.32 (cache)
- Jaeger (tracing)
- Seq (logging)

---

## GUIDE D'INSTALLATION WINDOWS

### 1. Installation de Git

- Téléchargez : https://git-scm.com/download/win
- Installez avec les paramètres par défaut
- **Vérification :** Ouvrez PowerShell et tapez : `git --version`

### 2. Installation du .NET 9.0 SDK

- Téléchargez : https://dotnet.microsoft.com/download/dotnet/9.0
- Choisissez "SDK x64" pour Windows
- **Vérification :** Dans PowerShell : `dotnet --version` (doit afficher 9.x.x)

### 3. Installation de Node.js

- Téléchargez Node.js 20.x LTS : https://nodejs.org/
- Installez avec les paramètres par défaut (npm sera inclus)
- **Vérification :** Dans PowerShell :
  - `node --version` (doit afficher v20.x.x ou v22.x.x)
  - `npm --version` (doit afficher 10.x.x ou plus)

### 4. Installation de Docker Desktop

- Téléchargez : https://www.docker.com/products/docker-desktop/
- **IMPORTANT :** Activez WSL 2 si demandé pendant l'installation
- Redémarrez l'ordinateur après installation
- Lancez Docker Desktop et attendez qu'il soit complètement démarré
- **Vérification :** Dans PowerShell : `docker --version`

### 5. Installation d'Angular CLI

```powershell
npm install -g @angular/cli@19
```

- **Vérification :** `ng version` (doit afficher Angular CLI 19.x.x)

### 6. Configuration des certificats HTTPS (Windows)

```powershell
dotnet dev-certs https --trust
```

- Cliquez "Oui" quand Windows demande d'installer le certificat

---

## GUIDE D'INSTALLATION macOS

### 1. Installation de Git

```bash
# Via Homebrew (recommandé)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
brew install git
```

- **Alternative :** Téléchargez depuis https://git-scm.com/download/mac
- **Vérification :** `git --version`

### 2. Installation du .NET 9.0 SDK

- Téléchargez : https://dotnet.microsoft.com/download/dotnet/9.0
- Choisissez "SDK x64" pour macOS
- **Alternative via Homebrew :**

```bash
brew install --cask dotnet-sdk
```

- **Vérification :** `dotnet --version`

### 3. Installation de Node.js

```bash
# Via Homebrew
brew install node@20
```

- **Alternative :** Téléchargez depuis https://nodejs.org/
- **Vérification :**
  - `node --version`
  - `npm --version`

### 4. Installation de Docker Desktop

- Téléchargez : https://www.docker.com/products/docker-desktop/
- Installez le .dmg
- Lancez Docker Desktop et attendez qu'il soit complètement démarré
- **Vérification :** `docker --version`

### 5. Installation d'Angular CLI

```bash
npm install -g @angular/cli@19
```

- **Vérification :** `ng version`

### 6. Configuration des certificats HTTPS (macOS)

```bash
dotnet dev-certs https --trust
```

---

## DÉMARRAGE DU PROJET

### 1. Clonage du repository

```bash
git clone [URL_DU_REPOSITORY]
cd PetBoarding
```

### 2. Démarrage du Backend avec Docker

```bash
cd Core_PetBoarding_Backend
docker-compose up --build
```

**Temps d'attente :** 5-10 minutes pour le premier build complet

**URLs Backend :**

- API HTTPS : https://localhost:5001
- Swagger UI : https://localhost:5001/swagger
- Base PostgreSQL : localhost:5432
- RabbitMQ Management : http://localhost:15672 (user: `petboarding`, pass: `petboarding123`)
- Jaeger UI : http://localhost:16686
- Seq UI : http://localhost:5341 (user: `admin`, pass: `petboarding123`)

Au démarrage de l'api, il y a un seeding de données pour pouvoir tester directement.
Il y a un utilisateur enregistré (test@petboarding.com / TestPetboarding123\*) qui a deux animaux.
Il y a aussi des prestations et des planning pour ces prestations.

### 3. Démarrage du Frontend Angular (nouveau terminal)

```bash
cd ng_PetBoarding_app
npm install
npm start
```

**URL Frontend :**

- Application Angular : http://localhost:4200

### 4. Vérification finale

- Backend : Ouvrez https://localhost:5001/swagger (doit afficher l'API)
- Frontend : Ouvrez http://localhost:4200 (doit afficher l'application)
- Base de données : Les migrations s'appliquent automatiquement au démarrage

---

## DÉPANNAGE COURANT

**Problème :** Docker ne démarre pas

- **Solution :** Redémarrez Docker Desktop, vérifiez que les ports 5000, 5001, 5432 ne sont pas occupés

**Problème :** Certificats HTTPS non reconnus

- **Solution :** Relancez `dotnet dev-certs https --trust`

**Problème :** npm install échoue

- **Solution :** Videz le cache npm : `npm cache clean --force`

**Problème :** Ports déjà occupés

- **Solution :** Vérifiez avec `netstat -an | findstr :5000` (Windows) ou `lsof -i :5000` (macOS)

**Problème :** Angular CLI non reconnu

- **Solution :** Redémarrez le terminal après installation

---

## INFORMATIONS COMPLÉMENTAIRES

**Architecture du projet :**

- Backend : Clean Architecture avec CQRS, minimal APIs, JWT Auth
- Frontend : Angular 19 standalone components, Angular Material + Bootstrap
- Base de données : PostgreSQL avec Entity Framework Core
- Containerisation : Docker Compose avec services complets

**Versions critiques à respecter :**

- .NET : 9.0.x
- Node.js : 20.x LTS minimum
- Angular : 19.1.x
- Docker : Version récente (2025)

Pour toute question technique, n'hésitez pas à me contacter.

Cordialement,
Kévin GUENET
