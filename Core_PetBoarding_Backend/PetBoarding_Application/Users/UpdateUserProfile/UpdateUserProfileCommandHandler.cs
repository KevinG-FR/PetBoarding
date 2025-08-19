using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, User>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // Récupérer l'utilisateur
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

            // Mettre à jour le profil
            var updateResult = user.UpdateProfile(
                firstnameResult.Value,
                lastnameResult.Value,
                phoneNumberResult.Value);

            if (updateResult.IsFailed)
                return Result.Fail(updateResult.Errors);

            // Sauvegarder les modifications
            var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
            if (updatedUser is null)
                return Result.Fail("Erreur lors de la mise à jour de l'utilisateur");

            return Result.Ok(updatedUser);
        }
    }
}
