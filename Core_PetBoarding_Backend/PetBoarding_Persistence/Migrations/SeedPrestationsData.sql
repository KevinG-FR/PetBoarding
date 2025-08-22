-- Script SQL pour insérer les données initiales des prestations
-- À exécuter après la migration AddPrestationsTable

-- Insertion des prestations dans la table Prestations
INSERT INTO "PetBoarding"."Prestations" 
(
    "Id", 
    "Libelle", 
    "Description", 
    "CategorieAnimal", 
    "Prix", 
    "DureeEnMinutes", 
    "EstDisponible", 
    "DateCreation", 
    "DateModification"
)
VALUES
-- Pension complète pour chiens
(
    'a1b2c3d4-e5f6-7890-1234-56789abcdef0',
    'Pension complète',
    'Garde de jour et nuit avec promenades et soins',
    0, -- TypeAnimal.Chien
    35.00,
    1440, -- 24 heures en minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Garderie journée pour chiens
(
    'b2c3d4e5-f6g7-8901-2345-6789abcdef01',
    'Garderie journée',
    'Garde en journée avec activités et socialisation',
    0, -- TypeAnimal.Chien
    25.00,
    480, -- 8 heures en minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Toilettage complet pour chiens
(
    'c3d4e5f6-g7h8-9012-3456-789abcdef012',
    'Toilettage complet',
    'Bain, coupe, griffes et soins esthétiques',
    0, -- TypeAnimal.Chien
    45.00,
    120, -- 2 heures en minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Promenade pour chiens
(
    'd4e5f6g7-h8i9-0123-4567-89abcdef0123',
    'Promenade',
    'Sortie individuelle ou en groupe',
    0, -- TypeAnimal.Chien
    15.00,
    60, -- 1 heure en minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Garde à domicile pour chats
(
    'e5f6g7h8-i9j0-1234-5678-9abcdef01234',
    'Garde à domicile',
    'Visite et soins au domicile du propriétaire',
    1, -- TypeAnimal.Chat
    20.00,
    30, -- 30 minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Pension chat
(
    'f6g7h8i9-j0k1-2345-6789-abcdef012345',
    'Pension chat',
    'Hébergement en chatterie avec soins personnalisés',
    1, -- TypeAnimal.Chat
    25.00,
    1440, -- 24 heures en minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Toilettage chat
(
    'g7h8i9j0-k1l2-3456-789a-bcdef0123456',
    'Toilettage chat',
    'Brossage, bain et coupe de griffes',
    1, -- TypeAnimal.Chat
    35.00,
    90, -- 1h30 en minutes
    true,
    '2025-01-01 00:00:00'::timestamp,
    NULL
),

-- Consultation comportementale (non disponible)
(
    'h8i9j0k1-l2m3-4567-89ab-cdef01234567',
    'Consultation comportementale',
    'Séance avec un spécialiste du comportement animal',
    0, -- TypeAnimal.Chien
    60.00,
    60, -- 1 heure en minutes
    false, -- Non disponible
    '2025-01-01 00:00:00'::timestamp,
    NULL
);

-- Vérification des données insérées
SELECT 
    "Id",
    "Libelle",
    "Description",
    CASE 
        WHEN "CategorieAnimal" = 0 THEN 'Chien'
        WHEN "CategorieAnimal" = 1 THEN 'Chat'
        ELSE 'Autre'
    END as "TypeAnimal",
    "Prix",
    "DureeEnMinutes",
    "EstDisponible",
    "DateCreation"
FROM "PetBoarding"."Prestations"
ORDER BY "CategorieAnimal", "Libelle";
