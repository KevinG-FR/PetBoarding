# Composants de Gestion du Profil Utilisateur

Ce dossier contient les composants pour la gestion du profil utilisateur dans l'application PetBoarding.

## Composants Créés

### 1. ProfileEditComponent
**Fichiers :**
- `profile-edit.component.ts` - Logique du composant
- `profile-edit.component.html` - Template HTML
- `profile-edit.component.scss` - Styles SCSS

**Fonctionnalités :**
- Modification des informations personnelles (prénom, nom, email, téléphone)
- Validation en temps réel des champs
- Détection des modifications non sauvegardées
- Intégration avec l'API backend (via ProfileApiService)
- Gestion des erreurs et affichage des messages de succès
- Bouton pour ouvrir le dialogue de changement de mot de passe

**Route :** `/profile/edit`

### 2. ChangePasswordDialogComponent
**Fichiers :**
- `change-password-dialog.component.ts` - Logique du composant
- `change-password-dialog.component.html` - Template HTML
- `change-password-dialog.component.scss` - Styles SCSS

**Fonctionnalités :**
- Interface modale pour changer le mot de passe
- Validation de la force du mot de passe avec critères visuels
- Confirmation du nouveau mot de passe
- Masquage/affichage des mots de passe
- Design responsive

## Services

### ProfileApiService
**Fichier :** `services/profile-api.service.ts`

**Fonctionnalités :**
- Gestion des appels API pour mettre à jour le profil
- Mapper les réponses API vers les modèles TypeScript
- Gestion centralisée des erreurs HTTP

## Contrats/DTOs

### UpdateProfileDto
**Fichier :** `contracts/update-profile.dto.ts`

**Contenu :**
- `UpdateProfileRequestDto` - Structure des données pour la mise à jour
- `UpdateProfileResponseDto` - Structure de la réponse API

## Intégration

### Routes
La route `/profile/edit` a été ajoutée dans `app.routes.ts` pour accéder au composant de modification du profil.

### AuthService
Méthode `updateCurrentUser()` ajoutée pour mettre à jour les données de l'utilisateur connecté après modification du profil.

### Navigation
Le composant `ProfileComponent` existant a été mis à jour pour naviguer vers la page d'édition via le bouton "Modifier le profil".

## Fonctionnalités Techniques

### Validation
- Validation des champs en temps réel avec Angular Reactive Forms
- Messages d'erreur personnalisés
- Validation de la force du mot de passe avec critères visuels

### UX/UI
- Design cohérent avec Material Design
- Animations et transitions fluides
- Interface responsive pour mobile et desktop
- Indicateurs de chargement et états disabled appropriés

### Sécurité
- Critères de mot de passe stricts (majuscule, minuscule, chiffre, caractère spécial, minimum 8 caractères)
- Confirmation des modifications avant navigation
- Gestion sécurisée des erreurs API

## Utilisation

1. L'utilisateur accède à son profil via `/profile`
2. Clique sur "Modifier le profil" pour accéder à `/profile/edit`
3. Peut modifier ses informations personnelles
4. Peut cliquer sur "Changer le mot de passe" pour ouvrir le dialogue modal
5. Les modifications sont sauvegardées via l'API backend

## À Implémenter

- [ ] Intégration réelle avec l'API backend (.NET)
- [ ] Service pour le changement de mot de passe
- [ ] Tests unitaires pour les composants
- [ ] Validation côté serveur
- [ ] Upload d'avatar utilisateur
- [ ] Historique des modifications
