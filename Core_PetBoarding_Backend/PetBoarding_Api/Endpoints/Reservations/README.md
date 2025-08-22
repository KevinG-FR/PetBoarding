# Structure des Endpoints - Reservations

Cette structure utilise des **classes partielles** pour améliorer la lisibilité et la maintenabilité du code.

## 📁 Organisation des fichiers

```
Reservations/
├── ReservationsEndpoints.cs                    # Configuration des routes principales
├── ReservationsEndpoints.GetReservations.cs    # Logique GET /reservations
├── ReservationsEndpoints.GetReservationById.cs # Logique GET /reservations/{id}
├── ReservationsEndpoints.CreateReservation.cs  # Logique POST /reservations
├── ReservationsEndpoints.UpdateReservation.cs  # Logique PUT /reservations/{id}
└── ReservationsEndpoints.CancelReservation.cs  # Logique DELETE /reservations/{id}
```

## 🎯 Avantages de cette approche

### ✅ **Lisibilité**

- Chaque endpoint dans son propre fichier
- Code plus facile à naviguer
- Logique isolée par responsabilité

### ✅ **Maintenabilité**

- Modifications isolées par endpoint
- Moins de conflits Git sur le même fichier
- Facilite les revues de code

### ✅ **Collaboration**

- Développeurs peuvent travailler en parallèle
- Responsabilités clairement séparées
- Historique Git plus précis

## 📋 Conventions

### **Nommage des fichiers**

- Format : `{Entity}Endpoints.{Action}.cs`
- Exemple : `ReservationsEndpoints.GetReservations.cs`

### **Structure du code**

- Namespace : `PetBoarding_Api.Endpoints.{Entity}`
- Classe : `public static partial class {Entity}Endpoints`
- Méthode : `private static async Task<IResult> {Action}(...)`

### **Imports**

- Imports spécifiques dans chaque fichier partiel
- Imports communs dans le fichier principal si nécessaire

## 🚀 Prochaines étapes

Cette structure peut être appliquée aux autres entités :

- `Users/`
- `Prestations/`
- Futures entités

## 📝 Exemple d'utilisation

Pour ajouter un nouvel endpoint :

1. Créer `ReservationsEndpoints.{NouvelAction}.cs`
2. Ajouter la méthode privée avec la logique
3. Ajouter le mapping dans `ReservationsEndpoints.cs`
4. Respecter les conventions de nommage et structure
