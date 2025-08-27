-- Script SQL pour ajouter des animaux à la table Pets pour l'utilisateur spécifié
-- Remplacez les valeurs des animaux selon vos besoins

INSERT INTO "Pets" (
    "Id",
    "Name",
    "Type",
    "Breed",
    "Age",
    "Color",
    "Gender",
    "IsNeutered",
    "OwnerId",
    "Weight",
    "MicrochipNumber",
    "MedicalNotes",
    "SpecialNeeds",
    "PhotoUrl",
    "EmergencyContact_Name",
    "EmergencyContact_Phone",
    "CreatedAt",
    "UpdatedAt"
) VALUES
    ('a1b2c3d4-e5f6-7890-1234-56789abcdef0', 'Rex', 0, 'Labrador', 5, 'Noir', 0, true, '01f46c24-9d13-459a-9454-2c40d9ea80c2', 30.5, '123456789', 'RAS', NULL, NULL, 'Julie Martin', '0612345678', NOW(), NOW()),
    ('b2c3d4e5-f6a1-2345-6789-0abcdef12345', 'Mia', 1, 'Siamois', 2, 'Blanc', 1, false, '01f46c24-9d13-459a-9454-2c40d9ea80c2', 4.2, NULL, NULL, 'Besoin de soins', NULL, 'Paul Dupont', '0698765432', NOW(), NOW()),
    ('c3d4e5f6-a1b2-3456-7890-abcdef123456', 'Nemo', 2, 'Poisson rouge', 1, 'Orange', 0, false, '01f46c24-9d13-459a-9454-2c40d9ea80c2', 0.1, NULL, NULL, NULL, NULL, 'Sophie Bernard', '0676543210', NOW(), NOW());

-- Remarques :
-- Type : 0=Chien, 1=Chat, 2=Poisson, etc. (adapter selon l'enum PetType)
-- Gender : 0=Male, 1=Femelle (adapter selon l'enum PetGender)
-- Les champs EmergencyContact sont laissés à NULL ici, à adapter si besoin
-- Les champs CreatedAt et UpdatedAt sont remplis avec NOW() (PostgreSQL)
-- Adaptez les valeurs selon vos besoins et la structure exacte de la table
