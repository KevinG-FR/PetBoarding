-- Script SQL amélioré pour insérer/mettre à jour les données des plannings et créneaux disponibles
-- Gère les réservations existantes, les week-ends et évite les doublons

-- Vérification que les prestations existent avant d'insérer les plannings
DO $$
BEGIN
    -- Vérifier que toutes les prestations nécessaires existent
    IF NOT EXISTS (SELECT 1 FROM "PetBoarding"."Prestations" WHERE "Id" IN (
        '018f4b4a-6789-7000-8000-123456789abc', -- Pension complète
        '018f4b4a-6789-7001-8000-123456789abd', -- Garderie journée
        '018f4b4a-6789-7002-8000-123456789abe', -- Toilettage complet
        '018f4b4a-6789-7003-8000-123456789abf', -- Promenade
        '018f4b4a-6789-7004-8000-123456789ac0', -- Garde à domicile
        '018f4b4a-6789-7005-8000-123456789ac1', -- Pension chat
        '018f4b4a-6789-7006-8000-123456789ac2'  -- Toilettage chat
    )) THEN
        RAISE EXCEPTION 'Les prestations requises n''existent pas. Veuillez d''abord exécuter SeedPrestationsData.sql';
    END IF;
END $$;

-- Insertion des plannings (avec gestion des doublons)
INSERT INTO "PetBoarding"."Plannings" 
(
    "Id", 
    "PrestationId", 
    "Label", 
    "Description", 
    "IsActive", 
    "DateCreation", 
    "DateModification"
)
VALUES
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde1',
    '018f4b4a-6789-7000-8000-123456789abc', -- Pension complète
    'Planning Pension Complète Chiens',
    'Planification des places de pension complète pour chiens avec hébergement de nuit',
    TRUE,
    NOW(),
    NULL
),
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde2',
    '018f4b4a-6789-7001-8000-123456789abd', -- Garderie journée
    'Planning Garderie Journée',
    'Planification de la garderie de jour avec activités',
    TRUE,
    NOW(),
    NULL
),
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde3',
    '018f4b4a-6789-7002-8000-123456789abe', -- Toilettage complet
    'Planning Toilettage Chiens',
    'Créneaux disponibles pour le toilettage complet des chiens',
    TRUE,
    NOW(),
    NULL
),
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde4',
    '018f4b4a-6789-7003-8000-123456789abf', -- Promenade
    'Planning Promenades',
    'Organisation des créneaux de promenades individuelles et collectives',
    TRUE,
    NOW(),
    NULL
),
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde5',
    '018f4b4a-6789-7004-8000-123456789ac0', -- Garde à domicile
    'Planning Gardes Domicile',
    'Planification des visites à domicile',
    TRUE,
    NOW(),
    NULL
),
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde6',
    '018f4b4a-6789-7005-8000-123456789ac1', -- Pension chat
    'Planning Pension Chats',
    'Planification des places de pension pour chats en chatterie',
    TRUE,
    NOW(),
    NULL
),
(
    'a1b2c3d4-e5f6-7890-1234-567890abcde7',
    '018f4b4a-6789-7006-8000-123456789ac2', -- Toilettage chat
    'Planning Toilettage Chats',
    'Créneaux de toilettage spécialisés pour chats',
    TRUE,
    NOW(),
    NULL
)
ON CONFLICT ("Id") DO UPDATE SET
    "Label" = EXCLUDED."Label",
    "Description" = EXCLUDED."Description",
    "IsActive" = EXCLUDED."IsActive",
    "DateModification" = NOW();

-- Génération/Mise à jour intelligente des créneaux disponibles pour les 3 prochains mois
DO $$
DECLARE
    date_courante DATE;
    date_fin DATE;
    planning_record RECORD;
    capacite_max INT;
    capacite_base INT;
    jour_semaine INT;
    existing_slot_record RECORD;
BEGIN
    -- Définir la période (3 mois à partir d'aujourd'hui)
    date_courante := CURRENT_DATE;
    date_fin := CURRENT_DATE + INTERVAL '3 months';
    
    RAISE NOTICE 'Génération des créneaux du % au %', date_courante, date_fin;
    
    -- Pour chaque planning actif
    FOR planning_record IN 
        SELECT "Id", "PrestationId", "Label" FROM "PetBoarding"."Plannings" WHERE "IsActive" = TRUE
    LOOP
        RAISE NOTICE 'Traitement du planning: %', planning_record."Label";
        
        -- Réinitialiser la date pour chaque planning
        date_courante := CURRENT_DATE;
        
        -- Pour chaque jour de la période
        WHILE date_courante <= date_fin LOOP
            jour_semaine := EXTRACT(DOW FROM date_courante); -- 0=Dimanche, 6=Samedi
            
            -- Définir la capacité de base selon le type de prestation
            IF planning_record."PrestationId" = '018f4b4a-6789-7000-8000-123456789abc' THEN -- Pension complète
                capacite_base := 10;
            ELSIF planning_record."PrestationId" = '018f4b4a-6789-7001-8000-123456789abd' THEN -- Garderie journée
                capacite_base := 15;
            ELSIF planning_record."PrestationId" = '018f4b4a-6789-7002-8000-123456789abe' THEN -- Toilettage complet
                capacite_base := 4;
            ELSIF planning_record."PrestationId" = '018f4b4a-6789-7003-8000-123456789abf' THEN -- Promenade
                capacite_base := 8;
            ELSIF planning_record."PrestationId" = '018f4b4a-6789-7004-8000-123456789ac0' THEN -- Garde à domicile
                capacite_base := 6;
            ELSIF planning_record."PrestationId" = '018f4b4a-6789-7005-8000-123456789ac1' THEN -- Pension chat
                capacite_base := 8;
            ELSIF planning_record."PrestationId" = '018f4b4a-6789-7006-8000-123456789ac2' THEN -- Toilettage chat
                capacite_base := 3;
            ELSE
                capacite_base := 5; -- Capacité par défaut
            END IF;
            
            -- Appliquer les réductions weekend selon le type de service
            capacite_max := capacite_base;
            
            IF jour_semaine IN (0, 6) THEN -- Weekend
                IF planning_record."PrestationId" = '018f4b4a-6789-7002-8000-123456789abe' THEN -- Toilettage complet
                    capacite_max := GREATEST(1, ROUND(capacite_base * 0.5)); -- -50%
                ELSIF planning_record."PrestationId" = '018f4b4a-6789-7006-8000-123456789ac2' THEN -- Toilettage chat
                    capacite_max := GREATEST(1, ROUND(capacite_base * 0.5)); -- -50%
                ELSIF planning_record."PrestationId" = '018f4b4a-6789-7001-8000-123456789abd' THEN -- Garderie journée
                    capacite_max := GREATEST(8, ROUND(capacite_base * 0.6)); -- -40%
                ELSIF planning_record."PrestationId" = '018f4b4a-6789-7004-8000-123456789ac0' THEN -- Garde à domicile
                    capacite_max := GREATEST(3, ROUND(capacite_base * 0.7)); -- -30%
                -- Pension et promenades gardent leur capacité normale le weekend
                END IF;
            END IF;
            
            -- Vérifier si un créneau existe déjà pour cette date et ce planning
            SELECT "Id", "MaxCapacity", "CapaciteReservee" 
            INTO existing_slot_record
            FROM "PetBoarding"."AvailableSlots" 
            WHERE "PlanningId" = planning_record."Id" AND "Date" = date_courante;
            
            IF existing_slot_record."Id" IS NOT NULL THEN
                -- Le créneau existe déjà : mise à jour uniquement si la capacité max a changé
                -- ET qu'elle ne risque pas de créer une incohérence avec les réservations
                IF existing_slot_record."MaxCapacity" != capacite_max 
                   AND existing_slot_record."CapaciteReservee" <= capacite_max THEN
                    
                    UPDATE "PetBoarding"."AvailableSlots" 
                    SET 
                        "MaxCapacity" = capacite_max,
                        "ModifiedAt" = NOW()
                    WHERE "Id" = existing_slot_record."Id";
                    
                    RAISE NOTICE 'Mis à jour créneau % pour %: capacité % -> %', 
                        date_courante, planning_record."Label", 
                        existing_slot_record."MaxCapacity", capacite_max;
                        
                ELSIF existing_slot_record."CapaciteReservee" > capacite_max THEN
                    -- Problème : plus de réservations que la nouvelle capacité
                    RAISE WARNING 'Conflit pour % le %: % réservations mais capacité proposée de %', 
                        planning_record."Label", date_courante, 
                        existing_slot_record."CapaciteReservee", capacite_max;
                END IF;
            ELSE
                -- Nouveau créneau : insertion
                INSERT INTO "PetBoarding"."AvailableSlots" 
                (
                    "Id",
                    "PlanningId",
                    "Date",
                    "MaxCapacity",
                    "CapaciteReservee",
                    "CreatedAt",
                    "ModifiedAt"
                )
                VALUES (
                    gen_random_uuid(),
                    planning_record."Id",
                    date_courante,
                    capacite_max,
                    0, -- Aucune réservation initialement
                    NOW(),
                    NULL
                );
            END IF;
            
            -- Passer au jour suivant
            date_courante := date_courante + INTERVAL '1 day';
        END LOOP;
    END LOOP;
    
    RAISE NOTICE 'Génération des créneaux terminée avec succès';
END $$;

-- Requête de vérification : afficher un résumé des plannings et créneaux créés
SELECT 
    p."Label" as "Planning",
    prest."Libelle" as "Prestation",
    prest."Id" as "PrestationId",
    COUNT(s."Id") as "NombreCreneaux",
    MIN(s."Date") as "PremiereDate",
    MAX(s."Date") as "DerniereDate",
    AVG(s."MaxCapacity") as "CapaciteMoyenne",
    SUM(s."MaxCapacity") as "CapaciteTotale",
    SUM(s."CapaciteReservee") as "TotalReserve",
    SUM(s."MaxCapacity" - s."CapaciteReservee") as "TotalDisponible"
FROM "PetBoarding"."Plannings" p
JOIN "PetBoarding"."Prestations" prest ON p."PrestationId" = prest."Id"
LEFT JOIN "PetBoarding"."AvailableSlots" s ON p."Id" = s."PlanningId"
WHERE p."IsActive" = TRUE
GROUP BY p."Id", p."Label", prest."Libelle", prest."Id"
ORDER BY prest."Libelle";

-- Requête pour voir les créneaux disponibles des 7 prochains jours (avec distinction weekend)
SELECT 
    s."Date",
    CASE EXTRACT(DOW FROM s."Date")
        WHEN 0 THEN 'Dimanche'
        WHEN 1 THEN 'Lundi'
        WHEN 2 THEN 'Mardi' 
        WHEN 3 THEN 'Mercredi'
        WHEN 4 THEN 'Jeudi'
        WHEN 5 THEN 'Vendredi'
        WHEN 6 THEN 'Samedi'
    END as "JourSemaine",
    CASE WHEN EXTRACT(DOW FROM s."Date") IN (0,6) THEN 'Weekend' ELSE 'Semaine' END as "TypeJour",
    p."Label" as "Planning",
    prest."Libelle" as "Prestation",
    s."MaxCapacity" as "Capacité",
    s."CapaciteReservee" as "Réservé",
    (s."MaxCapacity" - s."CapaciteReservee") as "Disponible"
FROM "PetBoarding"."AvailableSlots" s
JOIN "PetBoarding"."Plannings" p ON s."PlanningId" = p."Id"
JOIN "PetBoarding"."Prestations" prest ON p."PrestationId" = prest."Id"
WHERE s."Date" BETWEEN CURRENT_DATE AND (CURRENT_DATE + INTERVAL '7 days')
  AND p."IsActive" = TRUE
ORDER BY s."Date", prest."Libelle";

-- Requête pour détecter d'éventuels problèmes de cohérence
SELECT 
    s."Date",
    p."Label" as "Planning",
    prest."Libelle" as "Prestation",
    s."MaxCapacity",
    s."CapaciteReservee",
    CASE 
        WHEN s."CapaciteReservee" > s."MaxCapacity" THEN 'SURRESERVATION'
        WHEN s."CapaciteReservee" < 0 THEN 'CAPACITE_NEGATIVE' 
        ELSE 'OK'
    END as "Statut"
FROM "PetBoarding"."AvailableSlots" s
JOIN "PetBoarding"."Plannings" p ON s."PlanningId" = p."Id"
JOIN "PetBoarding"."Prestations" prest ON p."PrestationId" = prest."Id"
WHERE s."CapaciteReservee" > s."MaxCapacity" OR s."CapaciteReservee" < 0
ORDER BY s."Date", prest."Libelle";