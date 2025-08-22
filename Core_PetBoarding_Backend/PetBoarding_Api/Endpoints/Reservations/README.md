# Endpoints Reservations - PetBoarding API

Gestion des **réservations** pour les services de pension d'animaux.

## 📋 **Endpoints disponibles**

| Endpoint                    | Méthode  | Description                          | Autorisation |
| --------------------------- | -------- | ------------------------------------ | ------------ |
| `/api/v1/reservations`      | `GET`    | Lister les réservations avec filtres | Ouvert       |
| `/api/v1/reservations/{id}` | `GET`    | Détail d'une réservation             | Ouvert       |
| `/api/v1/reservations`      | `POST`   | Créer une nouvelle réservation       | Ouvert       |
| `/api/v1/reservations/{id}` | `PUT`    | Modifier une réservation             | Ouvert       |
| `/api/v1/reservations/{id}` | `DELETE` | Annuler une réservation              | Ouvert       |

## 🏗️ **Structure des fichiers**

```
Reservations/
├── ReservationsEndpoints.cs                    # Configuration routes + tags Swagger
├── ReservationsEndpoints.GetReservations.cs    # Liste avec filtres
├── ReservationsEndpoints.GetReservationById.cs # Détail par ID
├── ReservationsEndpoints.CreateReservation.cs  # Création avec validation
├── ReservationsEndpoints.UpdateReservation.cs  # Modification complète
└── ReservationsEndpoints.CancelReservation.cs  # Annulation (soft delete)
```

## 🔍 **Spécificités métier**

### **Filtres disponibles (GetReservations)**

- **UserId** : Réservations d'un utilisateur spécifique
- **StartDate** : Réservations à partir d'une date
- **EndDate** : Réservations jusqu'à une date
- **Status** : Filtrer par statut (En attente, Confirmée, Annulée, etc.)

### **États des réservations**

- **Pending** : En attente de confirmation
- **Confirmed** : Confirmée par l'établissement
- **InProgress** : En cours (animal hébergé)
- **Completed** : Terminée avec succès
- **Cancelled** : Annulée (par client ou établissement)

### **Règles de validation**

- **Dates** : StartDate < EndDate et futures uniquement
- **Durée** : Minimum 1 jour, maximum configurable
- **Conflits** : Vérification de disponibilité des prestations
- **Utilisateur** : Doit exister et être actif

## � **Exemples d'utilisation**

### **Créer une réservation**

```json
POST /api/v1/reservations
{
  "userId": "guid-user-id",
  "startDate": "2025-09-01T10:00:00Z",
  "endDate": "2025-09-05T12:00:00Z",
  "prestationIds": ["guid-prestation-1", "guid-prestation-2"],
  "notes": "Chat sensible, préfère le calme"
}
```

### **Filtrer les réservations**

```
GET /api/v1/reservations?userId=guid&startDate=2025-09-01&status=Confirmed
```

### **Mise à jour du statut**

```json
PUT /api/v1/reservations/{id}
{
  "status": "Confirmed",
  "notes": "Confirmation avec instructions spéciales"
}
```

## ⚠️ **Points d'attention**

### **Gestion des conflits**

- Vérifier la disponibilité avant confirmation
- Gérer les chevauchements de dates
- Limites de capacité par type de prestation

### **Notifications** (à implémenter)

- Confirmation de réservation → Email client
- Changement de statut → Notification push
- Rappel J-1 → SMS/Email

### **Audit et traçabilité**

- Log des changements de statut
- Historique des modifications
- Identification des auteurs

## � **Évolutions prévues**

### **Court terme**

- [ ] Gestion des disponibilités en temps réel
- [ ] Calcul automatique des prix
- [ ] Validation des créneaux horaires

### **Moyen terme**

- [ ] Système de notifications
- [ ] Gestion des acomptes/paiements
- [ ] Planning visuel intégré

### **Long terme**

- [ ] IA pour optimisation des créneaux
- [ ] Intégration calendrier externe
- [ ] Système de recommandations
