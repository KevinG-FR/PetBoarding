# Composant Pet Details

## Description

Le composant `pet-details` permet d'afficher et de modifier les informations détaillées d'un animal. Il offre deux modes :

- **Mode lecture** : Affichage des informations en lecture seule
- **Mode édition** : Formulaire permettant de modifier les informations

## Fonctionnalités

### Navigation

- **Route de consultation** : `/profile/pets/:id` - Mode lecture seule
- **Route d'édition** : `/profile/pets/:id/edit` - Mode édition direct

### Modes d'affichage

1. **Mode lecture** :
   - Affichage formaté des informations
   - Bouton "Modifier" pour passer en mode édition
   - Bouton "Retour" pour revenir au profil

2. **Mode édition** :
   - Formulaire avec validation
   - Bouton "Sauvegarder" (activé uniquement si des modifications sont détectées)
   - Bouton "Annuler" avec confirmation si des modifications non sauvegardées existent
   - Indicateur visuel des modifications non sauvegardées

### Intégration avec pet-card

Le composant `pet-card` navigue automatiquement vers les bonnes routes :

- **Bouton "Détails"** → `/profile/pets/:id` (mode lecture)
- **Bouton "Modifier"** → `/profile/pets/:id/edit` (mode édition)

## Structure des données

### Informations affichées

- **Informations générales** : nom, type, race, âge, poids, couleur, sexe
- **Statut de stérilisation**
- **Numéro de puce électronique**
- **Notes médicales**
- **Besoins spéciaux**
- **Contact d'urgence** : nom, téléphone, relation
- **Vaccinations** : liste avec dates d'expiration

### Validation du formulaire

- **Champs obligatoires** : nom, type, race, âge, couleur, sexe
- **Contraintes** :
  - Nom : minimum 2 caractères
  - Âge : entre 0 et 30 ans
  - Poids : entre 0.1 et 100 kg (optionnel)

## Fonctionnalités avancées

### Détection des modifications

- Le système détecte automatiquement les modifications dans le formulaire
- Un indicateur visuel apparaît en bas de page si des modifications ne sont pas sauvegardées
- Les boutons sont activés/désactivés selon l'état des modifications

### Confirmations utilisateur

- **Annulation avec modifications** : Dialog de confirmation
- **Navigation avec modifications** : Dialog de confirmation avant de quitter la page

### Gestion des erreurs

- Validation en temps réel des champs du formulaire
- Messages d'erreur contextuels
- Gestion des erreurs de chargement et de sauvegarde

## Utilisation

### Navigation programmatique

```typescript
// Vers le mode lecture
this.router.navigate(['/profile/pets', petId]);

// Vers le mode édition
this.router.navigate(['/profile/pets', petId, 'edit']);
```

### Service de données

Le composant utilise le `PetService` pour :

- Charger les données de l'animal
- Sauvegarder les modifications
- Gérer les erreurs

## Architecture technique

### Composants

- **PetDetailsComponent** : Composant principal
- **ConfirmCancelDialog** : Dialog de confirmation pour l'annulation

### Services utilisés

- **PetService** : Gestion des données des animaux
- **Router** : Navigation
- **MatDialog** : Dialogs de confirmation
- **MatSnackBar** : Notifications utilisateur

### Technologies

- Angular Signals pour la gestion d'état
- Reactive Forms pour le formulaire
- Angular Material pour l'UI
- Bootstrap pour la mise en page responsive

## Responsive Design

Le composant est entièrement responsive :

- **Desktop** : Layout en 2 colonnes (informations principales + sidebar)
- **Mobile** : Layout en 1 colonne avec adaptation des boutons d'action
- **Tablet** : Adaptation intermédiaire

## Améliorations possibles

1. **Gestion des photos** : Ajout/modification de photos d'animaux
2. **Historique des modifications** : Suivi des changements
3. **Partage** : Possibilité de partager les informations
4. **Export** : Export des données en PDF
5. **Synchronisation offline** : Fonctionnement hors ligne
