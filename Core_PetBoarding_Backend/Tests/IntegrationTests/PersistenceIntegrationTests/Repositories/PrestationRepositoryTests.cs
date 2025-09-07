using FluentAssertions;
using PetBoarding_Domain.Prestations;
using PetBoarding_Persistence.Repositories;

namespace PersistenceIntegrationTests.Repositories;

public class PrestationRepositoryTests : PostgreSqlTestBase
{
    private PrestationRepository _repository => new PrestationRepository(Context);

    [Fact]
    public async Task GetByIdAsync_Should_ReturnPrestation_When_PrestationExists()
    {
        // Arrange
        var prestation = new Prestation(
            "Garde de chien",
            "Garde complète pour chien à domicile",
            TypeAnimal.Chien,
            25.50m,
            480 // 8 heures
        );

        Context.Prestations.Add(prestation);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(prestation.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(prestation.Id);
        result.Libelle.Should().Be("Garde de chien");
        result.Prix.Should().Be(25.50m);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnAllPrestations()
    {
        // Arrange
        var prestations = new[]
        {
            new Prestation("Garde chien", "Description 1", TypeAnimal.Chien, 25.00m, 480),
            new Prestation("Garde chat", "Description 2", TypeAnimal.Chat, 20.00m, 360),
            new Prestation("Garde oiseau", "Description 3", TypeAnimal.Oiseau, 15.00m, 240)
        };

        Context.Prestations.AddRange(prestations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Libelle == "Garde chien");
        result.Should().Contain(p => p.Libelle == "Garde chat");
        result.Should().Contain(p => p.Libelle == "Garde oiseau");
    }

    [Fact]
    public async Task GetAllWithFilterAsync_Should_FilterByCategorieAnimal()
    {
        // Arrange
        var prestations = new[]
        {
            new Prestation("Garde chien 1", "Description 1", TypeAnimal.Chien, 25.00m, 480),
            new Prestation("Garde chien 2", "Description 2", TypeAnimal.Chien, 30.00m, 600),
            new Prestation("Garde chat", "Description 3", TypeAnimal.Chat, 20.00m, 360)
        };

        Context.Prestations.AddRange(prestations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWithFilterAsync(
            p => p.CategorieAnimal == TypeAnimal.Chien,
            CancellationToken.None
        );

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.CategorieAnimal == TypeAnimal.Chien);
        result.Should().Contain(p => p.Libelle == "Garde chien 1");
        result.Should().Contain(p => p.Libelle == "Garde chien 2");
    }

    [Fact]
    public async Task GetAllWithFilterAsync_Should_FilterByPriceRange()
    {
        // Arrange
        var prestations = new[]
        {
            new Prestation("Prestation économique", "Description 1", TypeAnimal.Chien, 15.00m, 240),
            new Prestation("Prestation standard", "Description 2", TypeAnimal.Chat, 25.00m, 360),
            new Prestation("Prestation premium", "Description 3", TypeAnimal.Chien, 45.00m, 600)
        };

        Context.Prestations.AddRange(prestations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWithFilterAsync(
            p => p.Prix >= 20.00m && p.Prix <= 30.00m,
            CancellationToken.None
        );

        // Assert
        result.Should().HaveCount(1);
        result.First().Libelle.Should().Be("Prestation standard");
        result.First().Prix.Should().Be(25.00m);
    }

    [Fact]
    public async Task AddAsync_Should_AddPrestation_And_SaveChanges()
    {
        // Arrange
        var prestation = new Prestation(
            "Nouvelle prestation",
            "Description de la nouvelle prestation",
            TypeAnimal.Lapin,
            18.50m,
            300
        );

        // Act
        var result = await _repository.AddAsync(prestation, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Libelle.Should().Be("Nouvelle prestation");
        result.CategorieAnimal.Should().Be(TypeAnimal.Lapin);

        var prestationInDb = await Context.Prestations.FindAsync(prestation.Id);
        prestationInDb.Should().NotBeNull();
        prestationInDb!.Prix.Should().Be(18.50m);
    }

    [Fact]
    public async Task UpdateAsync_Should_UpdateExistingPrestation()
    {
        // Arrange
        var prestation = new Prestation(
            "Prestation originale",
            "Description originale",
            TypeAnimal.Chien,
            20.00m,
            360
        );

        Context.Prestations.Add(prestation);
        await Context.SaveChangesAsync();

        // Modifier la prestation via reflection car pas de méthodes publiques
        var prixProperty = typeof(Prestation).GetProperty(nameof(Prestation.Prix));
        prixProperty?.SetValue(prestation, 25.00m);

        // Act
        var result = await _repository.UpdateAsync(prestation, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Prix.Should().Be(25.00m);
    }

    [Fact]
    public async Task DeleteAsync_Should_DeletePrestation()
    {
        // Arrange
        var prestation = new Prestation(
            "Prestation à supprimer",
            "Description",
            TypeAnimal.Chat,
            20.00m,
            360
        );

        Context.Prestations.Add(prestation);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(prestation, CancellationToken.None);

        // Assert
        result.Should().Be(1);

        var deletedPrestation = await Context.Prestations.FindAsync(prestation.Id);
        deletedPrestation.Should().BeNull();
    }

    [Theory]
    [InlineData(TypeAnimal.Chien)]
    [InlineData(TypeAnimal.Chat)]
    [InlineData(TypeAnimal.Oiseau)]
    [InlineData(TypeAnimal.Lapin)]
    [InlineData(TypeAnimal.Hamster)]
    [InlineData(TypeAnimal.Autre)]
    public async Task Repository_Should_HandleAllAnimalTypes(TypeAnimal typeAnimal)
    {
        // Arrange
        var prestation = new Prestation(
            $"Prestation {typeAnimal}",
            $"Description pour {typeAnimal}",
            typeAnimal,
            20.00m,
            360
        );

        // Act
        var result = await _repository.AddAsync(prestation, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CategorieAnimal.Should().Be(typeAnimal);

        var prestationInDb = await _repository.GetByIdAsync(prestation.Id);
        prestationInDb.Should().NotBeNull();
        prestationInDb!.CategorieAnimal.Should().Be(typeAnimal);
    }

    [Fact]
    public async Task Repository_Should_HandlePrestationsWithDifferentDurations()
    {
        // Arrange
        var prestations = new[]
        {
            new Prestation("Courte", "30 min", TypeAnimal.Hamster, 10.00m, 30),
            new Prestation("Moyenne", "2 heures", TypeAnimal.Chat, 20.00m, 120),
            new Prestation("Longue", "8 heures", TypeAnimal.Chien, 50.00m, 480),
            new Prestation("Très longue", "24 heures", TypeAnimal.Chien, 100.00m, 1440)
        };

        Context.Prestations.AddRange(prestations);
        await Context.SaveChangesAsync();

        // Act
        var shortDuration = await _repository.GetAllWithFilterAsync(
            p => p.DureeEnMinutes <= 120,
            CancellationToken.None
        );

        var longDuration = await _repository.GetAllWithFilterAsync(
            p => p.DureeEnMinutes >= 480,
            CancellationToken.None
        );

        // Assert
        shortDuration.Should().HaveCount(2);
        shortDuration.Should().Contain(p => p.Libelle == "Courte");
        shortDuration.Should().Contain(p => p.Libelle == "Moyenne");

        longDuration.Should().HaveCount(2);
        longDuration.Should().Contain(p => p.Libelle == "Longue");
        longDuration.Should().Contain(p => p.Libelle == "Très longue");
    }
}