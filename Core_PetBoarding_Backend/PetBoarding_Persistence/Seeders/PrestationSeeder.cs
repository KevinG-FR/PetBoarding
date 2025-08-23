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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7000-8000-123456789abc")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7001-8000-123456789abd")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7002-8000-123456789abe")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7003-8000-123456789abf")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7004-8000-123456789ac0")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7005-8000-123456789ac1")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7006-8000-123456789ac2")).Value,
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
                Id = new PrestationId(Guid.Parse("018f4b4a-6789-7007-8000-123456789ac3")).Value,
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
