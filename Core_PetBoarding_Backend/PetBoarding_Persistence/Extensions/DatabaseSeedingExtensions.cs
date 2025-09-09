using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence.Extensions;

public static class DatabaseSeedingExtensions
{
    public static void SeedTestData(this IApplicationBuilder app)
    {
        try
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            logger.LogInformation("Starting test data seeding process...");
            
            using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            
            logger.LogInformation("Services resolved successfully, proceeding with seeding...");
            
            SeedTestDataWithRetry(dbContext, accountService, logger);
        }
        catch (Exception ex)
        {
            // Log l'erreur mais ne pas faire planter l'app
            Console.WriteLine($"SEEDING ERROR: {ex.Message}");
            Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
            throw; // Re-throw pour que l'erreur soit visible
        }
    }

    private static void SeedTestDataWithRetry(ApplicationDbContext dbContext, IAccountService accountService, ILogger logger, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                logger.LogInformation("Attempting to seed test data (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
                SeedTestDataInternal(dbContext, accountService, logger);
                logger.LogInformation("Test data seeded successfully");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                logger.LogWarning(ex, "Failed to seed test data on attempt {Attempt}/{MaxRetries}. Retrying in {Delay} seconds...", 
                    i + 1, maxRetries, 2);
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed test data after {MaxRetries} attempts", maxRetries);
                throw;
            }
        }
    }

    private static void SeedTestDataInternal(ApplicationDbContext dbContext, IAccountService accountService, ILogger logger)
    {
        // Vérifier si les données de test existent déjà
        var testUserEmail = Email.Create("test@petboarding.com").Value;
        if (dbContext.Users.Any(u => u.Email == testUserEmail))
        {
            logger.LogInformation("Test data already exists, skipping seeding");
            return;
        }

        logger.LogInformation("Seeding test data...");

        // 1. Créer l'utilisateur de test
        var testUser = CreateTestUser(accountService);
        var testAddress = CreateTestAddress();
        testUser.Address = testAddress;
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();

        // 2. Créer les animaux de test
        var pets = CreateTestPets(testUser.Id);
        dbContext.Pets.AddRange(pets);
        dbContext.SaveChanges();

        // 3. Créer les prestations
        var prestations = CreateTestPrestations();
        dbContext.Prestations.AddRange(prestations);
        dbContext.SaveChanges();

        // 4. Créer les plannings et créneaux
        var (plannings, availableSlots) = CreateTestPlanningsAndSlots(prestations);
        dbContext.Plannings.AddRange(plannings);
        dbContext.SaveChanges();

        dbContext.AvailableSlots.AddRange(availableSlots);
        dbContext.SaveChanges();

        logger.LogInformation("Test data seeding completed successfully:");
        logger.LogInformation("- Test user: test@petboarding.com (password: TestPetboarding123*)");
        logger.LogInformation("- 2 pets added (Rex the dog and Minou the cat)");
        logger.LogInformation("- 5 prestations created with plannings");
        logger.LogInformation("- Available slots from 2025-09-09 to 2025-10-30");
    }

    private static User CreateTestUser(IAccountService accountService)
    {
        var firstname = Firstname.Create("Test").Value;
        var lastname = Lastname.Create("PetBoarding").Value;
        var email = Email.Create("test@petboarding.com").Value;
        var phoneNumber = PhoneNumber.Create("+33123456789").Value;
        var passwordHash = accountService.GetHashPassword("TestPetboarding123*");
        var profileType = UserProfileType.Customer;

        return new User(firstname, lastname, email, phoneNumber, passwordHash, profileType);
    }
    
    private static Address CreateTestAddress()
    {
        var streetNumber = StreetNumber.Create("123").Value;
        var streetName = StreetName.Create("Rue de l'Exemple").Value;
        var city = City.Create("Paris").Value;
        var postalCode = PostalCode.Create("75001").Value;
        var country = Country.Create("France").Value;

        return new Address(streetNumber, streetName, city, postalCode, country);
    }

    private static List<Pet> CreateTestPets(UserId ownerId)
    {
        var pets = new List<Pet>();

        // Rex le chien
        var rex = new Pet(
            name: "Rex",
            type: PetType.Chien,
            breed: "Labrador",
            age: 5,
            color: "Doré",
            gender: PetGender.Male,
            isNeutered: true,
            ownerId: ownerId,
            weight: 25.5m,
            medicalNotes: "Vaccins à jour"
        );
        pets.Add(rex);

        // Minou le chat
        var minou = new Pet(
            name: "Minou",
            type: PetType.Chat,
            breed: "Siamois",
            age: 3,
            color: "Blanc et noir",
            gender: PetGender.Female,
            isNeutered: true,
            ownerId: ownerId,
            weight: 4.2m,
            medicalNotes: "Allergie aux puces",
            specialNeeds: "Médicament quotidien"
        );
        pets.Add(minou);

        return pets;
    }

    private static List<Prestation> CreateTestPrestations()
    {
        var prestations = new List<Prestation>
        {
            new Prestation("Pension pour chien", "Garde de votre chien dans nos locaux", TypeAnimal.Chien, 45.00m, 1440),
            new Prestation("Pension pour chat", "Garde de votre chat dans nos locaux", TypeAnimal.Chat, 35.00m, 1440),
            new Prestation("Promenade chien (1h)", "Promenade individuelle d'une heure avec votre chien", TypeAnimal.Chien, 20.00m, 60),
            new Prestation("Garde à domicile (journée)", "Garde de votre animal à domicile pendant une journée complète", TypeAnimal.Autre, 80.00m, 480),
            new Prestation("Soins vétérinaires basiques", "Consultation, vaccination et soins de base", TypeAnimal.Chien, 60.00m, 30)
        };

        return prestations;
    }

    private static (List<Planning> plannings, List<AvailableSlot> slots) CreateTestPlanningsAndSlots(List<Prestation> prestations)
    {
        var plannings = new List<Planning>();
        var slots = new List<AvailableSlot>();
        var random = new Random(42); // Seed fixe pour des résultats reproductibles

        foreach (var prestation in prestations)
        {
            var planningId = new PlanningId(Guid.CreateVersion7());
            var planning = new Planning(planningId, prestation.Id, $"Planning {prestation.Libelle}", $"Planning automatique pour {prestation.Libelle}");
            plannings.Add(planning);

            // Créer des créneaux du 09 septembre au 30 octobre 2025
            var startDate = new DateTime(2025, 9, 9);
            var endDate = new DateTime(2025, 10, 30);
            
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Ne créer que les créneaux futurs
                if (date >= DateTime.Today)
                {
                    var maxCapacity = GetMaxCapacityForPrestation(prestation.Libelle);
                    var reservedCapacity = GenerateRealisticReservedCapacity(random, maxCapacity, date);
                    
                    var slot = AvailableSlot.Create(planningId, date, maxCapacity, reservedCapacity);
                    slots.Add(slot);
                }
            }
        }

        return (plannings, slots);
    }

    private static int GenerateRealisticReservedCapacity(Random random, int maxCapacity, DateTime slotDate)
    {
        var today = DateTime.Today;
        var daysFromNow = (slotDate - today).Days;
        
        // Plus la date est proche, plus il y a de chances d'avoir des réservations
        var reservationProbability = daysFromNow switch
        {
            <= 7 => 0.8,    // 80% de chance d'avoir des réservations dans les 7 prochains jours
            <= 14 => 0.6,   // 60% pour les 2 prochaines semaines
            <= 30 => 0.4,   // 40% pour le mois prochain
            _ => 0.2         // 20% pour plus tard
        };

        // Décider s'il y a des réservations
        if (random.NextDouble() > reservationProbability)
        {
            return 0; // Aucune réservation
        }

        // Générer le nombre de réservations avec une distribution réaliste
        var reservationIntensity = random.NextDouble();
        
        return reservationIntensity switch
        {
            >= 0.9 => maxCapacity,                                    // 10% complet
            >= 0.7 => Math.Max(1, (int)(maxCapacity * 0.8)),         // 20% presque plein
            >= 0.4 => Math.Max(1, (int)(maxCapacity * 0.5)),         // 30% à moitié plein
            _ => Math.Max(1, random.Next(1, Math.Max(2, maxCapacity / 2))) // 40% peu rempli
        };
    }

    private static int GetMaxCapacityForPrestation(string prestationLibelle)
    {
        return prestationLibelle switch
        {
            var s when s.Contains("Pension") => 4,  // 4 créneaux par jour pour la pension
            var s when s.Contains("Promenade") => 8,   // 8 créneaux par jour pour les promenades
            var s when s.Contains("Garde") => 2,       // 2 créneaux par jour pour la garde
            var s when s.Contains("Soins") => 6,       // 6 créneaux par jour pour les soins
            _ => 3
        };
    }
}