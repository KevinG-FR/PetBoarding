using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<User>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // Récupérer l'utilisateur avec son adresse
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result.Fail(UserErrors.NotFound(request.UserId.Value));

            // Valider et créer les value objects
            var firstnameResult = Firstname.Create(request.Firstname);
            if (firstnameResult.IsFailed)
                return Result.Fail(firstnameResult.Errors);

            var lastnameResult = Lastname.Create(request.Lastname);
            if (lastnameResult.IsFailed)
                return Result.Fail(lastnameResult.Errors);

            var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
            if (phoneNumberResult.IsFailed)
                return Result.Fail(phoneNumberResult.Errors);

            // Gérer l'adresse si fournie
            Address? address = null;
            if (request.Address != null)
            {
                // Valider et créer les value objects pour l'adresse
                var streetNumberResult = StreetNumber.Create(request.Address.StreetNumber);
                if (streetNumberResult.IsFailed)
                    return Result.Fail(streetNumberResult.Errors);

                var streetNameResult = StreetName.Create(request.Address.StreetName);
                if (streetNameResult.IsFailed)
                    return Result.Fail(streetNameResult.Errors);

                var cityResult = City.Create(request.Address.City);
                if (cityResult.IsFailed)
                    return Result.Fail(cityResult.Errors);

                var postalCodeResult = PostalCode.Create(request.Address.PostalCode);
                if (postalCodeResult.IsFailed)
                    return Result.Fail(postalCodeResult.Errors);

                var countryResult = Country.Create(request.Address.Country);
                if (countryResult.IsFailed)
                    return Result.Fail(countryResult.Errors);

                var complementResult = Complement.Create(request.Address.Complement);
                if (complementResult.IsFailed)
                    return Result.Fail(complementResult.Errors);

                if (user.Address is not null)
                {
                    // Mise à jour de l'adresse existante
                    user.Address.UpdateAddress(
                        streetNumberResult.Value,
                        streetNameResult.Value,
                        cityResult.Value,
                        postalCodeResult.Value,
                        countryResult.Value,
                        complementResult.Value);
                    address = user.Address;
                }
                else
                {
                    // Création d'une nouvelle adresse
                    address = new Address(
                        streetNumberResult.Value,
                        streetNameResult.Value,
                        cityResult.Value,
                        postalCodeResult.Value,
                        countryResult.Value,
                        complementResult.Value);
                }
            }

            try
            {
                // Mettre à jour le profil de l'utilisateur
                var updateResult = user.UpdateProfile(
                    firstnameResult.Value,
                    lastnameResult.Value,
                    phoneNumberResult.Value,
                    address);

                if (updateResult.IsFailed)
                    return Result.Fail(updateResult.Errors);

                var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
                if (updatedUser is null)
                    return Result.Fail("Error occurred while updating the user");

                // Update cache
                await _cacheService.SetAsync(CacheKeys.Users.ById(request.UserId.Value), updatedUser, null, cancellationToken);

                // Also refresh by email cache if user has email
                if (!string.IsNullOrEmpty(updatedUser.Email.Value))
                {
                    await _cacheService.SetAsync(CacheKeys.Users.ByEmail(updatedUser.Email.Value), updatedUser, null, cancellationToken);
                }

                return Result.Ok(updatedUser);
            
            }
            catch (Exception ex)
            {
                return Result.Fail($"Unexpected error updating user profile: {ex.Message}");
            }   
        }
    }
}
