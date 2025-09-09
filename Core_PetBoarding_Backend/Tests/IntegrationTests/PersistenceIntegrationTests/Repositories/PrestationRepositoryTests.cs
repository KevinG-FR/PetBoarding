using FluentAssertions;
using PersistenceIntegrationTests.TestHelpers;
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
        var prestation = EntityTestFactory.CreatePrestation();

        Context.Prestations.Add(prestation);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(prestation.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(prestation.Id);
        result.Libelle.Should().Be("Test Prestation");
        result.Prix.Should().Be(25.00m);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnAllPrestations()
    {
        // Arrange
        var prestations = new[]
        {
            Prestation.Create("Garde chien", "Description 1", TypeAnimal.Chien, 25.00m, 480),
            Prestation.Create("Garde chat", "Description 2", TypeAnimal.Chat, 20.00m, 360),
            Prestation.Create("Garde oiseau", "Description 3", TypeAnimal.Oiseau, 15.00m, 240)
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
            Prestation.Create("Garde chien 1", "Description 1", TypeAnimal.Chien, 25.00m, 480),
            Prestation.Create("Garde chien 2", "Description 2", TypeAnimal.Chien, 30.00m, 600),
            Prestation.Create("Garde chat", "Description 3", TypeAnimal.Chat, 20.00m, 360)
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
            Prestation.Create("Prestation économique", "Description 1", TypeAnimal.Chien, 15.00m, 240),
            Prestation.Create("Prestation standard", "Description 2", TypeAnimal.Chat, 25.00m, 360),
            Prestation.Create("Prestation premium", "Description 3", TypeAnimal.Chien, 45.00m, 600)
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
        var prestation = EntityTestFactory.CreatePrestation();

        // Act
        var result = await _repository.AddAsync(prestation, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Libelle.Should().Be("Test Prestation");
        result.CategorieAnimal.Should().Be(TypeAnimal.Chien);

        var prestationInDb = await Context.Prestations.FindAsync(prestation.Id);
        prestationInDb.Should().NotBeNull();
        prestationInDb!.Prix.Should().Be(25.00m);
    }

    [Fact]
    public async Task UpdateAsync_Should_UpdateExistingPrestation()
    {
        // Arrange
        var prestation = EntityTestFactory.CreatePrestation();

        Context.Prestations.Add(prestation);
        await Context.SaveChangesAsync();

        prestation.ModifierPrix(25.00m);

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
        var prestation = EntityTestFactory.CreatePrestation();

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
        var prestation = Prestation.Create(
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
            Prestation.Create("Courte", "30 min", TypeAnimal.Hamster, 10.00m, 30),
            Prestation.Create("Moyenne", "2 heures", TypeAnimal.Chat, 20.00m, 120),
            Prestation.Create("Longue", "8 heures", TypeAnimal.Chien, 50.00m, 480),
            Prestation.Create("Très longue", "24 heures", TypeAnimal.Chien, 100.00m, 1440)
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