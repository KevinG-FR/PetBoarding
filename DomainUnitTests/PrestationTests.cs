using FluentAssertions;
using PetBoarding_Domain.Prestations;

namespace DomainUnitTests;

public class PrestationTests
{
    private readonly string _validLibelle;
    private readonly string _validDescription;
    private readonly TypeAnimal _validTypeAnimal;
    private readonly decimal _validPrix;
    private readonly int _validDureeEnMinutes;

    public PrestationTests()
    {
        _validLibelle = "Garde à domicile";
        _validDescription = "Service de garde d'animaux à domicile";
        _validTypeAnimal = TypeAnimal.Chien;
        _validPrix = 25.50m;
        _validDureeEnMinutes = 60;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePrestation()
    {
        // Act
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Assert
        prestation.Should().NotBeNull();
        prestation.Id.Should().NotBeNull();
        prestation.Libelle.Should().Be(_validLibelle);
        prestation.Description.Should().Be(_validDescription);
        prestation.CategorieAnimal.Should().Be(_validTypeAnimal);
        prestation.Prix.Should().Be(_validPrix);
        prestation.DureeEnMinutes.Should().Be(_validDureeEnMinutes);
        prestation.EstDisponible.Should().BeTrue();
        prestation.DateCreation.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidLibelle_ShouldThrowArgumentException(string invalidLibelle)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Prestation(invalidLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes));
        
        exception.Message.Should().Contain("Le libellé ne peut pas être vide");
        exception.ParamName.Should().Be("libelle");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Constructor_WithNegativePrice_ShouldThrowArgumentException(decimal negativePrice)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Prestation(_validLibelle, _validDescription, _validTypeAnimal, negativePrice, _validDureeEnMinutes));
        
        exception.Message.Should().Contain("Le prix ne peut pas être négatif");
        exception.ParamName.Should().Be("prix");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-60)]
    public void Constructor_WithInvalidDuration_ShouldThrowArgumentException(int invalidDuration)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, invalidDuration));
        
        exception.Message.Should().Contain("La durée doit être supérieure à 0");
        exception.ParamName.Should().Be("dureeEnMinutes");
    }

    [Fact]
    public void ModifierLibelle_WithValidLibelle_ShouldUpdateLibelleAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var nouveauLibelle = "Garde de nuit";
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.ModifierLibelle(nouveauLibelle);

        // Assert
        prestation.Libelle.Should().Be(nouveauLibelle);
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ModifierLibelle_WithInvalidLibelle_ShouldThrowArgumentException(string invalidLibelle)
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => prestation.ModifierLibelle(invalidLibelle));
        exception.Message.Should().Contain("Le libellé ne peut pas être vide");
        exception.ParamName.Should().Be("nouveauLibelle");
    }

    [Fact]
    public void ModifierDescription_WithValidDescription_ShouldUpdateDescriptionAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var nouvelleDescription = "Service de garde d'animaux à domicile premium";
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.ModifierDescription(nouvelleDescription);

        // Assert
        prestation.Description.Should().Be(nouvelleDescription);
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Fact]
    public void ModifierPrix_WithValidPrice_ShouldUpdatePriceAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var nouveauPrix = 30.00m;
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.ModifierPrix(nouveauPrix);

        // Assert
        prestation.Prix.Should().Be(nouveauPrix);
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void ModifierPrix_WithNegativePrice_ShouldThrowArgumentException(decimal negativePrice)
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => prestation.ModifierPrix(negativePrice));
        exception.Message.Should().Contain("Le prix ne peut pas être négatif");
        exception.ParamName.Should().Be("nouveauPrix");
    }

    [Fact]
    public void ModifierDuree_WithValidDuration_ShouldUpdateDurationAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var nouvelleDuree = 90;
        var dateBefore = DateTime.UtcNow;
        
        // Wait to ensure time difference
        Thread.Sleep(10);

        // Act
        prestation.ModifierDuree(nouvelleDuree);

        // Assert
        prestation.DureeEnMinutes.Should().Be(nouvelleDuree);
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-60)]
    public void ModifierDuree_WithInvalidDuration_ShouldThrowArgumentException(int invalidDuration)
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => prestation.ModifierDuree(invalidDuration));
        exception.Message.Should().Contain("La durée doit être supérieure à 0");
        exception.ParamName.Should().Be("nouvelleDureeEnMinutes");
    }

    [Fact]
    public void ModifierCategorieAnimal_ShouldUpdateCategoryAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var nouvelleCategorie = TypeAnimal.Chat;
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.ModifierCategorieAnimal(nouvelleCategorie);

        // Assert
        prestation.CategorieAnimal.Should().Be(nouvelleCategorie);
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Fact]
    public void RendreDisponible_WhenIndisponible_ShouldUpdateAvailabilityAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        prestation.RendreIndisponible(); // First make it unavailable
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.RendreDisponible();

        // Assert
        prestation.EstDisponible.Should().BeTrue();
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Fact]
    public void RendreIndisponible_WhenDisponible_ShouldUpdateAvailabilityAndModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.RendreIndisponible();

        // Assert
        prestation.EstDisponible.Should().BeFalse();
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Fact]
    public void Activer_ShouldSetDisponibleToTrueAndUpdateModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        prestation.Desactiver(); // First deactivate
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.Activer();

        // Assert
        prestation.EstDisponible.Should().BeTrue();
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Fact]
    public void Desactiver_ShouldSetDisponibleToFalseAndUpdateModificationDate()
    {
        // Arrange
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);
        var dateBefore = DateTime.UtcNow;

        // Act
        prestation.Desactiver();

        // Assert
        prestation.EstDisponible.Should().BeFalse();
        prestation.DateModification.Should().NotBeNull();
        prestation.DateModification.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prestation.DateModification.Should().BeAfter(dateBefore);
    }

    [Fact]
    public void Constructor_ShouldSetEstDisponibleToTrueByDefault()
    {
        // Act
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Assert
        prestation.EstDisponible.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldSetDateCreationToCurrentUtcTime()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Assert
        var after = DateTime.UtcNow;
        prestation.DateCreation.Should().BeOnOrAfter(before);
        prestation.DateCreation.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Constructor_ShouldSetDateModificationToNull()
    {
        // Act
        var prestation = new Prestation(_validLibelle, _validDescription, _validTypeAnimal, _validPrix, _validDureeEnMinutes);

        // Assert
        prestation.DateModification.Should().BeNull();
    }
}