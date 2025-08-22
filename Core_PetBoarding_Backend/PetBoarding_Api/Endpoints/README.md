# Architecture des Endpoints - PetBoarding API

Cette architecture utilise des **classes partielles** pour organiser les endpoints de manière modulaire et maintenable.

## 🏗️ Structure générale

```
Endpoints/
├── ApiVersion.cs                 # Configuration des versions d'API
├── IEndpoint.cs                  # Interface commune (si applicable)
├── Tags.cs                       # Tags Swagger pour la documentation
├── Prestations/                  # 📁 Endpoints pour les prestations
│   ├── PrestationsEndpoints.cs
│   ├── PrestationsEndpoints.GetPrestations.cs
│   ├── PrestationsEndpoints.GetPrestationById.cs
│   ├── PrestationsEndpoints.CreatePrestation.cs
│   ├── PrestationsEndpoints.UpdatePrestation.cs
│   ├── PrestationsEndpoints.DeletePrestation.cs
│   └── README.md
├── Reservations/                 # 📁 Endpoints pour les réservations
│   ├── ReservationsEndpoints.cs
│   ├── ReservationsEndpoints.GetReservations.cs
│   ├── ReservationsEndpoints.GetReservationById.cs
│   ├── ReservationsEndpoints.CreateReservation.cs
│   ├── ReservationsEndpoints.UpdateReservation.cs
│   ├── ReservationsEndpoints.CancelReservation.cs
│   └── README.md
└── Users/                        # 📁 Endpoints pour les utilisateurs
    ├── UsersEndpoints.cs
    ├── UsersEndpoints.GetAllUsers.cs
    ├── UsersEndpoints.GetUserById.cs
    ├── UsersEndpoints.CreateUser.cs
    ├── UsersEndpoints.UpdateUserProfile.cs
    ├── UsersEndpoints.Authentification.cs
    └── README.md
```

## 🎯 Principes de l'architecture

### ✅ **Avantages des classes partielles**

1. **Séparation des responsabilités** : Un fichier = une responsabilité
2. **Lisibilité** : Code plus facile à naviguer et comprendre
3. **Maintenabilité** : Modifications isolées par endpoint
4. **Collaboration** : Moins de conflits Git, travail en parallèle facilité
5. **Documentation** : README par domaine pour contexte métier

### 📋 **Conventions appliquées**

#### **Structure des fichiers**

- **Principal** : `{Entity}Endpoints.cs` - Configuration des routes
- **Actions** : `{Entity}Endpoints.{Action}.cs` - Logique métier

#### **Namespaces**

- Format : `PetBoarding_Api.Endpoints.{Entity}`
- Exemples :
  - `PetBoarding_Api.Endpoints.Users`
  - `PetBoarding_Api.Endpoints.Prestations`
  - `PetBoarding_Api.Endpoints.Reservations`

#### **Classes et méthodes**

```csharp
public static partial class {Entity}Endpoints
{
    private static async Task<IResult> {Action}(...)
}
```

### 🔧 **Standards techniques**

#### **Status codes cohérents**

- **200 OK** : Lectures, modifications réussies
- **201 Created** : Créations avec location header
- **204 No Content** : Suppression sans retour
- **400 Bad Request** : Erreurs de validation
- **401 Unauthorized** : Authentification requise
- **404 Not Found** : Ressource introuvable

#### **Extensions utilisées**

- `GetHttpResult()` : Gestion des réponses avec mappeurs
- `SuccessStatusCode` : Enum pour codes de succès personnalisés
- Documentation Swagger complète avec attributs

#### **Patterns CQRS**

- **Commands** : Création, modification, suppression
- **Queries** : Lectures avec filtres optionnels
- **MediatR** : Orchestration des handlers

## 🔐 Gestion de l'autorisation

### **Par endpoint**

- `RequireAuthorization()` : Authentification requise
- `[HasPermission(Permission.{Right})]` : Permissions spécifiques

### **Exemples d'application**

- **Users.GetAllUsers** : Permission `ReadMember`
- **Users.UpdateUserProfile** : Authentification requise
- **Reservations/Prestations** : Actuellement ouvert (à adapter)

## 📊 **Comparaison avant/après**

| Aspect                  | Avant (Monolithique)          | Après (Classes partielles)   |
| ----------------------- | ----------------------------- | ---------------------------- |
| **Fichiers par entité** | 1 fichier (150+ lignes)       | 6-7 fichiers (20-40 lignes)  |
| **Navigation**          | Scroll dans un gros fichier   | Accès direct par endpoint    |
| **Conflits Git**        | Fréquents sur le même fichier | Rares, modifications isolées |
| **Lisibilité**          | Difficile avec la complexité  | Claire et ciblée             |
| **Maintenance**         | Modifications impactantes     | Changements localisés        |

## 🚀 **Guidelines pour l'ajout d'endpoints**

### **1. Créer le dossier entité**

```bash
mkdir Endpoints/{NewEntity}
```

### **2. Fichier principal**

```csharp
// {Entity}Endpoints.cs
namespace PetBoarding_Api.Endpoints.{Entity};

public static partial class {Entity}Endpoints
{
    public static void Map{Entity}Endpoints(this IEndpointRouteBuilder app)
    {
        // Configuration des routes
    }
}
```

### **3. Fichiers d'actions**

```csharp
// {Entity}Endpoints.{Action}.cs
namespace PetBoarding_Api.Endpoints.{Entity};

public static partial class {Entity}Endpoints
{
    private static async Task<IResult> {Action}(...)
    {
        // Logique métier
    }
}
```

### **4. Mise à jour du Program.cs**

```csharp
using PetBoarding_Api.Endpoints.{Entity};

// Dans la configuration
app.Map{Entity}Endpoints();
```

### **5. Documentation**

Créer un `README.md` dans le dossier avec :

- Description des endpoints
- Spécificités métier
- Exemples d'utilisation
- Évolutions possibles

## 🎉 **Résultat**

Cette architecture offre une base solide et évolutive pour l'API PetBoarding, respectant les principes de la Clean Architecture et facilitant la collaboration en équipe.
