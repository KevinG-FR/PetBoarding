# Script SQL pour les données initiales des Prestations

## Description

Ce script SQL `SeedPrestationsData.sql` contient les données initiales pour la table Prestations. Il a été créé pour remplacer le seeding automatique d'Entity Framework et vous donne plus de contrôle sur quand et comment les données sont insérées.

## Prérequis

- La migration `AddPrestationsTable` doit avoir été appliquée
- La base de données PostgreSQL doit être accessible
- L'utilisateur de base de données doit avoir les permissions d'écriture sur le schéma "PetBoarding"

## Utilisation

### Option 1 : Via psql (ligne de commande PostgreSQL)

```bash
psql -U <username> -d <database_name> -f SeedPrestationsData.sql
```

### Option 2 : Via pgAdmin ou un autre outil graphique

1. Ouvrez pgAdmin ou votre outil de gestion PostgreSQL préféré
2. Connectez-vous à votre base de données
3. Ouvrez le fichier `SeedPrestationsData.sql`
4. Exécutez le script

### Option 3 : Via Entity Framework (pour les développeurs)

Vous pouvez également exécuter ce script après avoir appliqué les migrations :

```bash
# Appliquer les migrations
dotnet ef database update --project PetBoarding_Persistence --startup-project PetBoarding_Api

# Puis exécuter le script SQL manuellement dans votre outil de base de données
```

## Données incluses

Le script insère 8 prestations :

### Pour les chiens (CategorieAnimal = 0)

1. **Pension complète** - 35€ - 24h - Garde de jour et nuit avec promenades et soins
2. **Garderie journée** - 25€ - 8h - Garde en journée avec activités et socialisation
3. **Toilettage complet** - 45€ - 2h - Bain, coupe, griffes et soins esthétiques
4. **Promenade** - 15€ - 1h - Sortie individuelle ou en groupe
5. **Consultation comportementale** - 60€ - 1h - ⚠️ NON DISPONIBLE

### Pour les chats (CategorieAnimal = 1)

6. **Garde à domicile** - 20€ - 30min - Visite et soins au domicile du propriétaire
7. **Pension chat** - 25€ - 24h - Hébergement en chatterie avec soins personnalisés
8. **Toilettage chat** - 35€ - 1h30 - Brossage, bain et coupe de griffes

## Vérification

Le script inclut une requête de vérification à la fin qui affiche toutes les prestations insérées avec un formatage lisible des types d'animaux.

## Notes importantes

- Tous les IDs sont des UUIDs fixes pour assurer la cohérence
- La consultation comportementale est marquée comme non disponible (`EstDisponible = false`)
- Toutes les prestations ont une date de création fixée au 1er janvier 2025
- Les prix sont en décimal avec 2 décimales
- Les durées sont en minutes

## En cas de problème

Si vous obtenez des erreurs lors de l'exécution :

1. Vérifiez que la migration a bien été appliquée
2. Vérifiez les permissions de votre utilisateur de base de données
3. Assurez-vous que le schéma "PetBoarding" existe
4. Vérifiez que la table "Prestations" n'est pas déjà peuplée (le script n'inclut pas de vérification de doublons)

Pour vider la table avant de ré-exécuter le script :

```sql
DELETE FROM "PetBoarding"."Prestations";
```
