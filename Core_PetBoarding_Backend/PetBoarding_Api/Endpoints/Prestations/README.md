# Structure des Endpoints - Prestations

Cette structure utilise des **classes partielles** pour améliorer la lisibilité et la maintenabilité du code.

## 📁 Organisation des fichiers

```
Prestations/
├── PrestationsEndpoints.cs                    # Configuration des routes principales
├── PrestationsEndpoints.GetPrestations.cs     # Logique GET /prestations (avec filtres)
├── PrestationsEndpoints.GetPrestationById.cs  # Logique GET /prestations/{id}
├── PrestationsEndpoints.CreatePrestation.cs   # Logique POST /prestations
├── PrestationsEndpoints.UpdatePrestation.cs   # Logique PUT /prestations/{id}
└── PrestationsEndpoints.DeletePrestation.cs   # Logique DELETE /prestations/{id}
```

## 🔧 Endpoints et leurs fonctionnalités

### **GET /prestations** (GetPrestations)

- **Filtres disponibles** :
  - `categorieAnimal` : Filtrer par type d'animal (Chien/Chat)
  - `estDisponible` : Filtrer par disponibilité (true/false)
  - `searchText` : Recherche textuelle dans libellé/description
- **Retour** : Liste paginée des prestations avec compte total
- **Autorisation** : Non

### **GET /prestations/{id}** (GetPrestationById)

- **Fonction** : Récupère les détails d'une prestation spécifique
- **Retour** : Prestation complète ou 404 si introuvable
- **Autorisation** : Non

### **POST /prestations** (CreatePrestation)

- **Fonction** : Crée une nouvelle prestation de service
- **Retour** : 201 Created avec la prestation créée + Location header
- **Autorisation** : Non (à adapter selon besoins métier)

### **PUT /prestations/{id}** (UpdatePrestation)

- **Fonction** : Met à jour tous les champs d'une prestation
- **Champs modifiables** : Libellé, description, catégorie, prix, durée, disponibilité
- **Retour** : 200 OK avec la prestation mise à jour
- **Autorisation** : Non (à adapter selon besoins métier)

### **DELETE /prestations/{id}** (DeletePrestation)

- **Fonction** : Supprime logiquement une prestation
- **Retour** : 200 OK avec message de confirmation
- **Autorisation** : Non (à adapter selon besoins métier)

## 🎯 Spécificités techniques

### **Filtrage avancé**

Le fichier `PrestationsEndpoints.GetPrestations.cs` implémente un système de filtrage flexible :

- **Par type d'animal** : Enum `TypeAnimal` (Chien = 0, Chat = 1)
- **Par disponibilité** : Boolean `estDisponible`
- **Par recherche** : Texte libre dans `searchText`

### **Gestion des status codes**

- **201 Created** : Pour `CreatePrestation` avec location URI
- **200 OK** : Pour les lectures, modifications et suppressions
- **404 Not Found** : Pour les ressources introuvables
- **400 Bad Request** : Pour les erreurs de validation

### **Response DTOs spécialisés**

- `GetAllPrestationsResponse` : Collection avec pagination
- `GetPrestationResponse` : Prestation individuelle
- `CreatePrestationResponse` : Prestation créée
- `UpdatePrestationResponse` : Prestation mise à jour
- `DeletePrestationResponse` : Confirmation de suppression

### **Mapping entité/DTO**

- `PrestationMapper.ToDto()` : Conversion entité → DTO
- `PrestationResponseMapper.*` : Conversion DTO → Response

## 🔄 Flux de données typiques

### **Création d'une prestation**

```
Request → CreatePrestationCommand → Domain.Prestation → Repository → DTO → Response (201)
```

### **Mise à jour d'une prestation**

```
Request → UpdatePrestationCommand → Domain.Prestation → Repository → DTO → Response (200)
```

### **Filtrage des prestations**

```
Query params → GetPrestationsQuery → Repository avec filtres → List<DTO> → Response (200)
```

## 📋 Conventions respectées

- **Namespace** : `PetBoarding_Api.Endpoints.Prestations`
- **Classe** : `public static partial class PrestationsEndpoints`
- **Méthodes** : `private static async Task<IResult>`
- **Extensions** : Utilisation de `GetHttpResult()` avec status codes appropriés
- **Documentation** : Attributs Swagger complets avec descriptions

## 🔮 Évolutions possibles

- **Autorisation** : Ajouter des permissions selon les rôles utilisateur
- **Validation** : Enrichir les validations métier (prix, durée, etc.)
- **Audit** : Traçabilité des modifications des prestations
- **Cache** : Mise en cache des prestations fréquemment consultées
- **Pagination** : Améliorer la pagination pour de gros volumes

Cette structure facilite la maintenance des fonctionnalités prestations et respecte les principes de la Clean Architecture.
