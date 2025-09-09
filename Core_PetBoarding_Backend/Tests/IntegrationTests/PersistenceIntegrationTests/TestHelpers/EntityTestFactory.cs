using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Prestations;

namespace PersistenceIntegrationTests.TestHelpers;

public static class EntityTestFactory
{
    public static User CreateUser(
        string firstname = "John",
        string lastname = "Doe",
        string email = "john.doe@test.com",
        string phoneNumber = "+33123456789",
        string passwordHash = "hashedpassword",
        UserProfileType profileType = UserProfileType.Customer)
    {
        return User.Create(
            Firstname.Create(firstname).Value,
            Lastname.Create(lastname).Value,
            Email.Create(email).Value,
            PhoneNumber.Create(phoneNumber).Value,
            passwordHash,
            profileType
        );
    }

    public static Address CreateAddress(
        string streetNumber = "123",
        string streetName = "Test Street",
        string city = "Test City",
        string postalCode = "12345",
        string country = "Test Country",
        string? complement = null)
    {
        return Address.Create(
            StreetNumber.Create(streetNumber).Value,
            StreetName.Create(streetName).Value,
            City.Create(city).Value,
            PostalCode.Create(postalCode).Value,
            Country.Create(country).Value,
            complement != null ? Complement.Create(complement).Value : null
        );
    }

    public static Prestation CreatePrestation(
        string libelle = "Test Prestation",
        string description = "Test Description",
        TypeAnimal categorieAnimal = TypeAnimal.Chien,
        decimal prix = 25.00m,
        int dureeEnMinutes = 480)
    {
        return Prestation.Create(libelle, description, categorieAnimal, prix, dureeEnMinutes);
    }

    public static void AssignAddressToUser(User user, Address address)
    {
        user.AddressId = address.Id;
        user.Address = address;
    }
}