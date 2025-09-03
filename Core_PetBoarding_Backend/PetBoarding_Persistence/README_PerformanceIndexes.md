# Index de Performance - PetBoarding Database

Ce document décrit tous les index de performance mis en place dans la base de données PetBoarding pour optimiser les requêtes les plus fréquentes.

## 📋 Vue d'ensemble

La migration `AddPerformanceIndexes` (20250903205040) ajoute **19 index optimisés** sur les tables principales de l'application. Ces index sont **déclarés dans les fichiers de configuration Entity Framework** puis générés automatiquement par EF Core.

## 🏗️ Architecture d'implémentation

### ✅ Approche déclarative (recommandée)

Les index sont déclarés dans les fichiers `*Configuration.cs` avec la syntaxe Entity Framework :

```csharp
// Dans UserConfiguration.cs
builder.HasIndex(x => new { x.Email, x.PasswordHash })
    .HasDatabaseName("idx_users_email_password");

// Index avec filtre (partiel)
builder.HasIndex(x => x.EstDisponible)
    .HasDatabaseName("idx_prestations_disponible")
    .HasFilter("\"EstDisponible\" = true");

// Index avec ordre de tri
builder.HasIndex(x => new { x.UserId, x.CreatedAt })
    .HasDatabaseName("idx_reservations_userid_createdat")
    .IsDescending(false, true); // UserId ASC, CreatedAt DESC
```

### Avantages de cette approche :

- ✅ **Code-first** : Index versionnés avec la logique métier
- ✅ **Type-safe** : Validation au compile-time avec IntelliSense
- ✅ **Maintenable** : Cohérent avec Clean Architecture
- ✅ **Automatique** : EF génère le SQL optimal pour PostgreSQL
- ✅ **Rollback intégré** : Méthode Down() auto-générée et complète

## 📊 Index par table et fichier de configuration

### 🔐 UserConfiguration.cs (4 index)

#### `idx_users_email_password`

```csharp
builder.HasIndex(x => new { x.Email, x.PasswordHash })
    .HasDatabaseName("idx_users_email_password");
```

**Priorité :** 🔴 CRITIQUE  
**Justification :** Optimise l'authentification dans `UserRepository.GetUserForAuthentification()`. Cette requête est exécutée à chaque connexion utilisateur.  
**Impact :** Améliore drastiquement les temps de connexion.

#### `idx_users_email`

```csharp
builder.HasIndex(x => x.Email)
    .HasDatabaseName("idx_users_email");
```

**Priorité :** 🟠 HAUTE  
**Justification :**

- Optimise `UserRepository.UserEmailAlreadyUsed()` lors de l'inscription
- Optimise `UserRepository.GetByEmailAsync()`
- Utilisé pour la validation d'unicité des emails

#### `idx_users_status_profiletype`

```csharp
builder.HasIndex(x => new { x.Status, x.ProfileType })
    .HasDatabaseName("idx_users_status_profiletype");
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Permet de filtrer efficacement les utilisateurs actifs par type (client/admin).

#### `idx_users_created_at`

```csharp
builder.HasIndex(x => x.CreatedAt)
    .HasDatabaseName("idx_users_created_at");
```

**Priorité :** 🟢 BASSE  
**Justification :** Optimise les tris chronologiques et les statistiques d'inscription.

---

### 🐾 PrestationConfiguration.cs (4 index)

#### `idx_prestations_disponible_categorie`

```csharp
builder.HasIndex(p => new { p.EstDisponible, p.CategorieAnimal })
    .HasDatabaseName("idx_prestations_disponible_categorie");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise `PrestationRepository.GetAllAsync()` avec filtres multiples. Couvre les cas d'usage les plus fréquents de recherche de prestations.

#### `idx_prestations_categorie_libelle`

```csharp
builder.HasIndex(p => new { p.CategorieAnimal, p.Libelle })
    .HasDatabaseName("idx_prestations_categorie_libelle");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise `PrestationRepository.GetByCategorieAnimalAsync()` avec tri par libellé.

#### `idx_prestations_date_creation`

```csharp
builder.HasIndex(p => p.DateCreation)
    .IsDescending()
    .HasDatabaseName("idx_prestations_date_creation");
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Optimise le tri descendant utilisé dans `GetAllAsync()` pour afficher les prestations les plus récentes.

#### `idx_prestations_disponible` (Index partiel)

```csharp
builder.HasIndex(p => p.EstDisponible)
    .HasDatabaseName("idx_prestations_disponible")
    .HasFilter("\"EstDisponible\" = true");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Index partiel très efficace pour `PrestationRepository.GetDisponiblesAsync()`. Ne stocke que les prestations disponibles.

---

### 📅 ReservationConfiguration.cs (5 index)

#### `idx_reservations_userid_createdat`

```csharp
builder.HasIndex(r => new { r.UserId, r.CreatedAt })
    .HasDatabaseName("idx_reservations_userid_createdat")
    .IsDescending(false, true); // UserId ASC, CreatedAt DESC
```

**Priorité :** 🔴 CRITIQUE  
**Justification :** Optimise `ReservationRepository.GetByUserIdAsync()`. Requête extrêmement fréquente pour afficher l'historique utilisateur.

#### `idx_reservations_serviceid_createdat`

```csharp
builder.HasIndex(r => new { r.ServiceId, r.CreatedAt })
    .HasDatabaseName("idx_reservations_serviceid_createdat")
    .IsDescending(false, true); // ServiceId ASC, CreatedAt DESC
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise `ReservationRepository.GetByServiceIdAsync()` pour les statistiques par prestation.

#### `idx_reservations_status_startdate`

```csharp
builder.HasIndex(r => new { r.Status, r.StartDate })
    .HasDatabaseName("idx_reservations_status_startdate");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise les filtres multiples dans `GetAllAsync()` et les requêtes d'administration.

#### `idx_reservations_date_range`

```csharp
builder.HasIndex(r => new { r.StartDate, r.EndDate })
    .HasDatabaseName("idx_reservations_date_range");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise spécifiquement `ReservationRepository.GetReservationsBetweenDatesAsync()` pour les plannings.

#### `idx_reservations_user_displayed` (Index partiel)

```csharp
builder.HasIndex(r => new { r.UserId, r.Status, r.CreatedAt })
    .HasDatabaseName("idx_reservations_user_displayed")
    .HasFilter("\"Status\" IN ('Validated', 'InProgress', 'Completed')")
    .IsDescending(false, false, true); // UserId ASC, Status ASC, CreatedAt DESC
```

**Priorité :** 🟠 HAUTE  
**Justification :** Index partiel très efficace pour `GetUserDisplayedReservationsAsync()`. Ne stocke que les réservations visibles (Validated, InProgress, Completed).

---

### 🎯 ReservationSlotConfiguration.cs (3 index)

#### `idx_reservation_slots_reservation_available` (Unique)

```csharp
builder.HasIndex(rs => new { rs.ReservationId, rs.AvailableSlotId })
    .IsUnique()
    .HasDatabaseName("idx_reservation_slots_reservation_available");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise les jointures entre réservations et créneaux. Utilisé dans `GetByIdAsync()` avec Include. Contrainte d'unicité préservée.

#### `idx_reservation_slots_available_active`

```csharp
builder.HasIndex(rs => new { rs.AvailableSlotId, rs.ReleasedAt })
    .HasDatabaseName("idx_reservation_slots_available_active");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Optimise la recherche des réservations par créneau disponible avec statut.

#### `idx_reservation_slots_active` (Index partiel)

```csharp
builder.HasIndex(rs => rs.ReleasedAt)
    .HasDatabaseName("idx_reservation_slots_active")
    .HasFilter("\"ReleasedAt\" IS NULL");
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Index partiel pour les créneaux actifs (non libérés). Très efficace car ne stocke que les créneaux avec ReleasedAt IS NULL.

---

### 🛒 BasketConfiguration.cs (2 index)

#### `idx_baskets_userid_status`

```csharp
builder.HasIndex(x => new { x.UserId, x.Status })
    .HasDatabaseName("idx_baskets_userid_status");
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Optimise la recherche de paniers utilisateur par statut (Active, Completed, etc.).

#### `idx_baskets_payment_id`

```csharp
builder.HasIndex(x => x.PaymentId)
    .HasDatabaseName("idx_baskets_payment_id");
```

**Priorité :** 🟢 BASSE  
**Justification :** Optimise les requêtes de recherche de panier par paiement associé.

---

### 💳 PaymentConfiguration.cs (2 index)

#### `idx_payments_status_createdat`

```csharp
builder.HasIndex(x => new { x.Status, x.CreatedAt })
    .HasDatabaseName("idx_payments_status_createdat")
    .IsDescending(false, true); // Status ASC, CreatedAt DESC
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Optimise l'historique des paiements par statut avec tri chronologique descendant.

#### `idx_payments_external_transaction` (Unique avec filtre)

```csharp
builder.HasIndex(x => x.ExternalTransactionId)
    .IsUnique()
    .HasDatabaseName("idx_payments_external_transaction")
    .HasFilter("\"ExternalTransactionId\" IS NOT NULL");
```

**Priorité :** 🟠 HAUTE  
**Justification :** Index unique partiel pour les IDs de transactions externes. Assure l'unicité tout en permettant les valeurs NULL.

---

### 🐕 PetConfiguration.cs (3 index)

#### `idx_pets_owner_type`

```csharp
builder.HasIndex(p => new { p.OwnerId, p.Type })
    .HasDatabaseName("idx_pets_owner_type");
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Optimise la recherche des animaux par propriétaire et type (chien, chat, etc.).

#### `idx_pets_owner_id`

```csharp
builder.HasIndex(p => p.OwnerId)
    .HasDatabaseName("idx_pets_owner_id");
```

**Priorité :** 🟡 MOYENNE  
**Justification :** Optimise les requêtes de tous les animaux d'un propriétaire.

#### `idx_pets_name`

```csharp
builder.HasIndex(p => p.Name)
    .HasDatabaseName("idx_pets_name");
```

**Priorité :** 🟢 BASSE  
**Justification :** Optimise les recherches d'animaux par nom pour l'administration.

## 🚀 Mise en production

### Commandes de déploiement

```bash
# Navigation vers le projet
cd Core_PetBoarding_Backend

# Application de la migration (génère automatiquement CREATE INDEX)
dotnet ef database update --project PetBoarding_Persistence --startup-project PetBoarding_Api
```

### Points d'attention

#### ✅ Avantages de l'approche Entity Framework

- ✅ **Pas de verrouillage** des tables pendant la création (EF utilise les bonnes pratiques PostgreSQL)
- ✅ **L'application reste accessible** pendant la migration
- ✅ **Type-safe** et validé au compile-time
- ✅ **Rollback automatique** complet via la méthode Down()

#### ⚠️ Considérations importantes

- ⏱️ La création peut prendre du temps selon la taille des données
- 🔍 Surveillance recommandée des performances pendant la migration
- 📊 Exécuter idéalement pendant les heures creuses pour minimiser l'impact

#### 🛡️ Sécurité et rollback

- 🔄 Rollback possible avec `dotnet ef database update <previous-migration>`
- 🗑️ Les index sont automatiquement supprimés lors du rollback
- ✅ EF génère automatiquement `DROP INDEX IF EXISTS` pour éviter les erreurs

## 📈 Impact attendu

### Performances queries améliorées

| Repository            | Méthode                             | Index utilisé                       | Amélioration estimée |
| --------------------- | ----------------------------------- | ----------------------------------- | -------------------- |
| UserRepository        | `GetUserForAuthentification`        | `idx_users_email_password`          | 🔴 90%+              |
| ReservationRepository | `GetByUserIdAsync`                  | `idx_reservations_userid_createdat` | 🔴 80%+              |
| PrestationRepository  | `GetDisponiblesAsync`               | `idx_prestations_disponible`        | 🟠 70%+              |
| ReservationRepository | `GetUserDisplayedReservationsAsync` | `idx_reservations_user_displayed`   | 🟠 75%+              |

### Métriques à surveiller

- ✅ Temps de réponse des connexions utilisateur
- ✅ Performance de l'affichage des historiques de réservation
- ✅ Rapidité de chargement des prestations disponibles
- ✅ Efficacité des recherches par plages de dates

## 🔧 Maintenance et évolution

### Fichiers de configuration modifiés

```
PetBoarding_Persistence/Configurations/
├── UserConfiguration.cs          (4 index ajoutés)
├── PrestationConfiguration.cs    (4 index optimisés)
├── ReservationConfiguration.cs   (5 index ajoutés)
├── ReservationSlotConfiguration.cs (3 index optimisés)
├── BasketConfiguration.cs        (2 index optimisés)
├── PaymentConfiguration.cs       (2 index optimisés)
└── PetConfiguration.cs          (3 index optimisés)
```

### Pour ajouter de nouveaux index

1. **Modifier le fichier de configuration** approprié
2. **Ajouter les `.HasIndex()`** avec la syntaxe Entity Framework
3. **Générer une migration :** `dotnet ef migrations add <NomMigration>`
4. **Appliquer la migration :** `dotnet ef database update`

### Surveillance recommandée

- 📊 Surveiller l'utilisation des index via `pg_stat_user_indexes`
- 🔍 Analyser les plans d'exécution des requêtes lentes
- 📈 Mesurer l'impact sur les temps de réponse

### Commandes PostgreSQL utiles

```sql
-- Vérifier l'utilisation des index
SELECT schemaname, tablename, indexname, idx_scan, idx_tup_read, idx_tup_fetch
FROM pg_stat_user_indexes
WHERE schemaname = 'PetBoarding' AND indexname LIKE 'idx_%'
ORDER BY idx_scan DESC;

-- Taille des index
SELECT schemaname, tablename, indexname, pg_size_pretty(pg_relation_size(indexrelid)) as size
FROM pg_stat_user_indexes
WHERE schemaname = 'PetBoarding' AND indexname LIKE 'idx_%'
ORDER BY pg_relation_size(indexrelid) DESC;
```

---

_Document généré le 3 septembre 2025_  
_Migration : `20250903205040_AddPerformanceIndexes`_
