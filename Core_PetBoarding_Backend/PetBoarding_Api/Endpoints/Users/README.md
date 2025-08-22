# Structure des Endpoints - Users

Cette structure utilise des **classes partielles** pour améliorer la lisibilité et la maintenabilité du code.

## 📁 Organisation des fichiers

```
Users/
├── UsersEndpoints.cs                    # Configuration des routes principales
├── UsersEndpoints.GetAllUsers.cs        # Logique GET /users (avec autorisation)
├── UsersEndpoints.GetUserById.cs        # Logique GET /users/{userId}
├── UsersEndpoints.CreateUser.cs         # Logique POST /users
├── UsersEndpoints.UpdateUserProfile.cs  # Logique PUT /users/{userId}/profile
└── UsersEndpoints.Authentification.cs   # Logique POST /users/login
```

## 🔐 Endpoints et leurs fonctionnalités

### **GET /users** (GetAllUsers)

- **Permission** : `ReadMember` requise
- **Fonction** : Récupère la liste de tous les utilisateurs
- **Autorisation** : Oui (`RequireAuthorization()`)

### **GET /users/{userId}** (GetUserById)

- **Fonction** : Récupère les détails d'un utilisateur spécifique
- **Autorisation** : Non

### **POST /users** (CreateUser)

- **Fonction** : Crée un nouveau compte utilisateur
- **Retour** : 201 Created avec l'utilisateur créé
- **Autorisation** : Non

### **POST /users/login** (Authentification)

- **Fonction** : Authentifie un utilisateur et retourne un JWT
- **Retour** : Token JWT + informations utilisateur
- **Autorisation** : Non

### **PUT /users/{userId}/profile** (UpdateUserProfile)

- **Fonction** : Met à jour le profil d'un utilisateur
- **Champs** : Nom, prénom, téléphone, adresse
- **Autorisation** : Oui (`RequireAuthorization()`)

## 🎯 Spécificités techniques

### **Authentification complexe**

Le fichier `UsersEndpoints.Authentification.cs` contient la logique la plus complexe :

- Vérification des credentials
- Génération du token JWT
- Récupération et mapping des données utilisateur
- Construction manuelle du DTO avec adresse

### **Autorisation**

- Utilise l'attribut `[HasPermission(Permission.ReadMember)]`
- Gère les permissions au niveau des endpoints
- Intégration avec le système d'autorisation du domaine

### **Mapping d'adresse**

- Conversion entre `Address` (domaine) et `AddressDto` (API)
- Gestion des adresses optionnelles (nullable)

## 🔧 Dépendances principales

- **MediatR** : Pour les commandes et requêtes CQRS
- **PetBoarding_Domain.Accounts** : Permissions et services métier
- **PetBoarding_Infrastructure.Authentication** : Attributs et services d'auth
- **UserMapper/UserResponseMapper** : Conversion entités/DTOs

## 📋 Conventions respectées

- **Namespace** : `PetBoarding_Api.Endpoints.Users`
- **Classe** : `public static partial class UsersEndpoints`
- **Méthodes** : `private static async Task<IResult>`
- **Extensions** : Utilisation de `GetHttpResult()` avec status codes
- **Documentation** : Attributs Swagger complets

Cette structure facilite la maintenance et l'évolution des fonctionnalités utilisateur tout en respectant les principes SOLID et la Clean Architecture.
