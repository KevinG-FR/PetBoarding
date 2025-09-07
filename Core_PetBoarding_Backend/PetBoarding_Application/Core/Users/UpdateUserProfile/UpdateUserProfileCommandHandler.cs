using FluentResults;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<User>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // Get user from repository
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Fail(UserErrors.NotFound(request.UserId.Value));
        }

        // Create value objects for user profile
        var firstnameResult = Firstname.Create(request.Firstname);
        if (firstnameResult.IsFailed)
            return Result.Fail(firstnameResult.Errors);

        var lastnameResult = Lastname.Create(request.Lastname);
        if (lastnameResult.IsFailed)
            return Result.Fail(lastnameResult.Errors);

        var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneNumberResult.IsFailed)
            return Result.Fail(phoneNumberResult.Errors);

        // Create address if provided
        Address? address = null;
        if (request.AddressData is not null)
        {
            var streetNumberResult = StreetNumber.Create(request.AddressData.StreetNumber);
            if (streetNumberResult.IsFailed)
                return Result.Fail(streetNumberResult.Errors);

            var streetNameResult = StreetName.Create(request.AddressData.StreetName);
            if (streetNameResult.IsFailed)
                return Result.Fail(streetNameResult.Errors);

            var cityResult = City.Create(request.AddressData.City);
            if (cityResult.IsFailed)
                return Result.Fail(cityResult.Errors);

            var postalCodeResult = PostalCode.Create(request.AddressData.PostalCode);
            if (postalCodeResult.IsFailed)
                return Result.Fail(postalCodeResult.Errors);

            var countryResult = Country.Create(request.AddressData.Country);
            if (countryResult.IsFailed)
                return Result.Fail(countryResult.Errors);

            Complement? complement = null;
            if (!string.IsNullOrWhiteSpace(request.AddressData.Complement))
            {
                var complementResult = Complement.Create(request.AddressData.Complement);
                if (complementResult.IsFailed)
                    return Result.Fail(complementResult.Errors);
                complement = complementResult.Value;
            }

            address = new Address(
                streetNumberResult.Value,
                streetNameResult.Value,
                cityResult.Value,
                postalCodeResult.Value,
                countryResult.Value,
                complement);
        }

        // Update user profile using domain method
        var updateResult = user.UpdateProfile(
            firstnameResult.Value,
            lastnameResult.Value,
            phoneNumberResult.Value,
            address);

        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        var cacheKeyById = CacheKeys.Users.ById(user.Id.Value);
        var cacheKeyByEmail = CacheKeys.Users.ByEmail(user.Email.Value);
        await _cacheService.RemoveAsync(cacheKeyById, cancellationToken);
        await _cacheService.RemoveAsync(cacheKeyByEmail, cancellationToken);

        return Result.Ok(user);
    }
}