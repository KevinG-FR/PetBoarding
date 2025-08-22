namespace PetBoarding_Persistence.Seeders;

using PetBoarding_Domain.Prestations;
using Microsoft.EntityFrameworkCore;

public static class PrestationSeeder
{
    public static void SeedPrestations(ModelBuilder modelBuilder)
    {
        var prestations = new[]
        {
            new
            {
                Id = new PrestationId(Guid.Parse("a1b2c3d4-e5f6-7890-1234-56789abcdef0")).Value,
                Libelle = "Pension complète",
                Description = "Garde de jour et nuit avec promenades et soins",
                CategorieAnimal = TypeAnimal.Chien,
                Prix = 35.00m,
                DureeEnMinutes = 1440, // 24 heures
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("b2c3d4e5-f6g7-8901-2345-6789abcdef01")).Value,
                Libelle = "Garderie journée",
                Description = "Garde en journée avec activités et socialisation",
                CategorieAnimal = TypeAnimal.Chien,
                Prix = 25.00m,
                DureeEnMinutes = 480, // 8 heures
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("c3d4e5f6-g7h8-9012-3456-789abcdef012")).Value,
                Libelle = "Toilettage complet",
                Description = "Bain, coupe, griffes et soins esthétiques",
                CategorieAnimal = TypeAnimal.Chien,
                Prix = 45.00m,
                DureeEnMinutes = 120, // 2 heures
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("d4e5f6g7-h8i9-0123-4567-89abcdef0123")).Value,
                Libelle = "Promenade",
                Description = "Sortie individuelle ou en groupe",
                CategorieAnimal = TypeAnimal.Chien,
                Prix = 15.00m,
                DureeEnMinutes = 60, // 1 heure
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("e5f6g7h8-i9j0-1234-5678-9abcdef01234")).Value,
                Libelle = "Garde à domicile",
                Description = "Visite et soins au domicile du propriétaire",
                CategorieAnimal = TypeAnimal.Chat,
                Prix = 20.00m,
                DureeEnMinutes = 30, // 30 minutes
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("f6g7h8i9-j0k1-2345-6789-abcdef012345")).Value,
                Libelle = "Pension chat",
                Description = "Hébergement en chatterie avec soins personnalisés",
                CategorieAnimal = TypeAnimal.Chat,
                Prix = 25.00m,
                DureeEnMinutes = 1440, // 24 heures
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("g7h8i9j0-k1l2-3456-789a-bcdef0123456")).Value,
                Libelle = "Toilettage chat",
                Description = "Brossage, bain et coupe de griffes",
                CategorieAnimal = TypeAnimal.Chat,
                Prix = 35.00m,
                DureeEnMinutes = 90, // 1h30
                EstDisponible = true,
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            },
            new
            {
                Id = new PrestationId(Guid.Parse("h8i9j0k1-l2m3-4567-89ab-cdef01234567")).Value,
                Libelle = "Consultation comportementale",
                Description = "Séance avec un spécialiste du comportement animal",
                CategorieAnimal = TypeAnimal.Chien,
                Prix = 60.00m,
                DureeEnMinutes = 60, // 1 heure
                EstDisponible = false, // Non disponible comme dans Angular
                DateCreation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DateModification = (DateTime?)null
            }
        };

        modelBuilder.Entity<Prestation>().HasData(prestations);
    }
}
