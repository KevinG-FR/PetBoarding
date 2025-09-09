using FluentAssertions;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace DomainUnitTests;

public class PetTests
{
    private readonly UserId _validOwnerId;
    private readonly string _validName;
    private readonly PetType _validType;
    private readonly string _validBreed;
    private readonly int _validAge;
    private readonly string _validColor;
    private readonly PetGender _validGender;
    private readonly bool _validIsNeutered;

    public PetTests()
    {
        _validOwnerId = new UserId(Guid.NewGuid());
        _validName = "Buddy";
        _validType = PetType.Chien;
        _validBreed = "Golden Retriever";
        _validAge = 3;
        _validColor = "Golden";
        _validGender = PetGender.Male;
        _validIsNeutered = false;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePet()
    {
        // Act
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);

        // Assert
        pet.Should().NotBeNull();
        pet.Id.Should().NotBeNull();
        pet.Name.Should().Be(_validName);
        pet.Type.Should().Be(_validType);
        pet.Breed.Should().Be(_validBreed);
        pet.Age.Should().Be(_validAge);
        pet.Color.Should().Be(_validColor);
        pet.Gender.Should().Be(_validGender);
        pet.IsNeutered.Should().Be(_validIsNeutered);
        pet.OwnerId.Should().Be(_validOwnerId);
        pet.Weight.Should().BeNull();
        pet.MicrochipNumber.Should().BeNull();
        pet.MedicalNotes.Should().BeNull();
        pet.SpecialNeeds.Should().BeNull();
        pet.PhotoUrl.Should().BeNull();
        pet.EmergencyContact.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithAllOptionalParameters_ShouldCreatePet()
    {
        // Arrange
        var weight = 25.5m;
        var microchipNumber = "123456789012345";
        var medicalNotes = "Vaccinated";
        var specialNeeds = "Needs daily medication";
        var photoUrl = "https://example.com/photo.jpg";
        var emergencyContact = new EmergencyContact("John Doe", "0123456789", "john@example.com");

        // Act
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, 
                         _validOwnerId, weight, microchipNumber, medicalNotes, specialNeeds, photoUrl, emergencyContact);

        // Assert
        pet.Weight.Should().Be(weight);
        pet.MicrochipNumber.Should().Be(microchipNumber);
        pet.MedicalNotes.Should().Be(medicalNotes);
        pet.SpecialNeeds.Should().Be(specialNeeds);
        pet.PhotoUrl.Should().Be(photoUrl);
        pet.EmergencyContact.Should().Be(emergencyContact);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Pet.Create(invalidName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId));
        
        exception.Message.Should().Contain("Le nom de l'animal ne peut pas être vide");
        exception.ParamName.Should().Be("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidBreed_ShouldThrowArgumentException(string invalidBreed)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Pet.Create(_validName, _validType, invalidBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId));
        
        exception.Message.Should().Contain("La race de l'animal ne peut pas être vide");
        exception.ParamName.Should().Be("breed");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_WithNegativeAge_ShouldThrowArgumentException(int negativeAge)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Pet.Create(_validName, _validType, _validBreed, negativeAge, _validColor, _validGender, _validIsNeutered, _validOwnerId));
        
        exception.Message.Should().Contain("L'âge de l'animal ne peut pas être négatif");
        exception.ParamName.Should().Be("age");
    }

    [Fact]
    public void Constructor_WithAgeOver50_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Pet.Create(_validName, _validType, _validBreed, 51, _validColor, _validGender, _validIsNeutered, _validOwnerId));
        
        exception.Message.Should().Contain("L'âge de l'animal ne peut pas dépasser 50 ans");
        exception.ParamName.Should().Be("age");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidColor_ShouldThrowArgumentException(string invalidColor)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Pet.Create(_validName, _validType, _validBreed, _validAge, invalidColor, _validGender, _validIsNeutered, _validOwnerId));
        
        exception.Message.Should().Contain("La couleur de l'animal ne peut pas être vide");
        exception.ParamName.Should().Be("color");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Constructor_WithNegativeWeight_ShouldThrowArgumentException(decimal negativeWeight)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId, negativeWeight));
        
        exception.Message.Should().Contain("Le poids de l'animal ne peut pas être négatif");
        exception.ParamName.Should().Be("weight");
    }

    [Fact]
    public void Constructor_ShouldTrimStringProperties()
    {
        // Arrange
        var nameWithSpaces = " Buddy ";
        var breedWithSpaces = " Golden Retriever ";
        var colorWithSpaces = " Golden ";
        var microchipWithSpaces = " 123456789012345 ";
        var medicalNotesWithSpaces = " Vaccinated ";
        var specialNeedsWithSpaces = " Needs daily medication ";
        var photoUrlWithSpaces = " https://example.com/photo.jpg ";

        // Act
        var pet = Pet.Create(nameWithSpaces, _validType, breedWithSpaces, _validAge, colorWithSpaces, _validGender, _validIsNeutered, 
                         _validOwnerId, null, microchipWithSpaces, medicalNotesWithSpaces, specialNeedsWithSpaces, photoUrlWithSpaces);

        // Assert
        pet.Name.Should().Be("Buddy");
        pet.Breed.Should().Be("Golden Retriever");
        pet.Color.Should().Be("Golden");
        pet.MicrochipNumber.Should().Be("123456789012345");
        pet.MedicalNotes.Should().Be("Vaccinated");
        pet.SpecialNeeds.Should().Be("Needs daily medication");
        pet.PhotoUrl.Should().Be("https://example.com/photo.jpg");
    }

    [Fact]
    public void UpdateBasicInfo_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var newName = "Max";
        var newBreed = "Labrador";
        var newAge = 5;
        var newColor = "Black";

        // Act
        pet.UpdateBasicInfo(newName, newBreed, newAge, newColor);

        // Assert
        pet.Name.Should().Be(newName);
        pet.Breed.Should().Be(newBreed);
        pet.Age.Should().Be(newAge);
        pet.Color.Should().Be(newColor);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateBasicInfo_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => pet.UpdateBasicInfo(invalidName, _validBreed, _validAge, _validColor));
        exception.Message.Should().Contain("Le nom de l'animal ne peut pas être vide");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(51)]
    public void UpdateBasicInfo_WithInvalidAge_ShouldThrowArgumentException(int invalidAge)
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => pet.UpdateBasicInfo(_validName, _validBreed, invalidAge, _validColor));
        exception.Message.Should().Contain("L'âge de l'animal doit être entre 0 et 50 ans");
    }

    [Fact]
    public void UpdateWeight_WithValidWeight_ShouldUpdateWeight()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var newWeight = 30.5m;

        // Act
        pet.UpdateWeight(newWeight);

        // Assert
        pet.Weight.Should().Be(newWeight);
    }

    [Fact]
    public void UpdateWeight_WithNull_ShouldSetWeightToNull()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId, 25.0m);

        // Act
        pet.UpdateWeight(null);

        // Assert
        pet.Weight.Should().BeNull();
    }

    [Fact]
    public void UpdateWeight_WithNegativeWeight_ShouldThrowArgumentException()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => pet.UpdateWeight(-1));
        exception.Message.Should().Contain("Le poids de l'animal ne peut pas être négatif");
    }

    [Fact]
    public void UpdateType_ShouldUpdateType()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var newType = PetType.Chat;

        // Act
        pet.UpdateType(newType);

        // Assert
        pet.Type.Should().Be(newType);
    }

    [Fact]
    public void UpdateGender_ShouldUpdateGender()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var newGender = PetGender.Female;

        // Act
        pet.UpdateGender(newGender);

        // Assert
        pet.Gender.Should().Be(newGender);
    }

    [Fact]
    public void UpdateNeuteredStatus_ShouldUpdateIsNeutered()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);

        // Act
        pet.UpdateNeuteredStatus(true);

        // Assert
        pet.IsNeutered.Should().BeTrue();
    }

    [Fact]
    public void UpdateMicrochipNumber_ShouldUpdateAndTrimMicrochipNumber()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var microchipNumber = " 123456789012345 ";

        // Act
        pet.UpdateMicrochipNumber(microchipNumber);

        // Assert
        pet.MicrochipNumber.Should().Be("123456789012345");
    }

    [Fact]
    public void UpdateMedicalNotes_ShouldUpdateAndTrimMedicalNotes()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var medicalNotes = " Vaccinated against rabies ";

        // Act
        pet.UpdateMedicalNotes(medicalNotes);

        // Assert
        pet.MedicalNotes.Should().Be("Vaccinated against rabies");
    }

    [Fact]
    public void UpdateSpecialNeeds_ShouldUpdateAndTrimSpecialNeeds()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var specialNeeds = " Needs daily medication ";

        // Act
        pet.UpdateSpecialNeeds(specialNeeds);

        // Assert
        pet.SpecialNeeds.Should().Be("Needs daily medication");
    }

    [Fact]
    public void UpdatePhotoUrl_ShouldUpdateAndTrimPhotoUrl()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var photoUrl = " https://example.com/photo.jpg ";

        // Act
        pet.UpdatePhotoUrl(photoUrl);

        // Assert
        pet.PhotoUrl.Should().Be("https://example.com/photo.jpg");
    }

    [Fact]
    public void UpdateEmergencyContact_ShouldUpdateEmergencyContact()
    {
        // Arrange
        var pet = Pet.Create(_validName, _validType, _validBreed, _validAge, _validColor, _validGender, _validIsNeutered, _validOwnerId);
        var emergencyContact = new EmergencyContact("Jane Doe", "0987654321", "jane@example.com");

        // Act
        pet.UpdateEmergencyContact(emergencyContact);

        // Assert
        pet.EmergencyContact.Should().Be(emergencyContact);
    }
}