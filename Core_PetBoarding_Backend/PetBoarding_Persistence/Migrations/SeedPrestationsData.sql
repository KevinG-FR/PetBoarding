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
(
    '018f4b4a-6789-7000-8000-123456789abc',
    'Pension complète',
    'Garde de jour et nuit avec promenades et soins',
    0, 
    35.00,
    1440,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7001-8000-123456789abd',
    'Garderie journée',
    'Garde en journée avec activités et socialisation',
    0,
    25.00,
    480,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7002-8000-123456789abe',
    'Toilettage complet',
    'Bain, coupe, griffes et soins esthétiques',
    0,
    45.00,
    120,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7003-8000-123456789abf',
    'Promenade',
    'Sortie individuelle ou en groupe',
    0,
    15.00,
    60,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7004-8000-123456789ac0',
    'Garde à domicile',
    'Visite et soins au domicile du propriétaire',
    1,
    20.00,
    30,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7005-8000-123456789ac1',
    'Pension chat',
    'Hébergement en chatterie avec soins personnalisés',
    1,
    25.00,
    1440,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7006-8000-123456789ac2',
    'Toilettage chat',
    'Brossage, bain et coupe de griffes',
    1,
    35.00,
    90,
    TRUE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
),
(
    '018f4b4a-6789-7007-8000-123456789ac3',
    'Consultation comportementale',
    'Séance avec un spécialiste du comportement animal',
    0,
    60.00,
    60,
    FALSE,
    '2025-01-01 00:00:00+00'::timestamp with time zone,
    NULL
);


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
