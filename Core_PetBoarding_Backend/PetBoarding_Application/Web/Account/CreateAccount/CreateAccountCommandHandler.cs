using FluentResults;

using PetBoarding_Application.Core.Abstractions;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Web.Account.CreateAccount
{
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountService _accountService;

        public CreateAccountCommandHandler(
            IUserRepository userRepository,
            IAccountService accountService)
        {
            _userRepository = userRepository;
            _accountService = accountService;
        }
        public async Task<Result<User>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Validation des value objects avant la création
            var firstnameResult = Firstname.Create(request.Firstname);
            if (!firstnameResult.IsSuccess)
                return Result.Fail(firstnameResult.Errors);

            var lastnameResult = Lastname.Create(request.Lastname);
            if (!lastnameResult.IsSuccess)
                return Result.Fail(lastnameResult.Errors);

            var emailResult = PetBoarding_Domain.Users.Email.Create(request.Email);
            if (!emailResult.IsSuccess)
                return Result.Fail(emailResult.Errors);

            var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
            if (!phoneNumberResult.IsSuccess)
                return Result.Fail(phoneNumberResult.Errors);

            var resultCheckEmailAlreadyUsed = await _userRepository.UserEmailAlreadyUsed(request.Email, cancellationToken);
            if (resultCheckEmailAlreadyUsed)
                return Result.Fail(UserErrors.EmailAlreadyUsed(request.Email));

            var passwordHash = _accountService.GetHashPassword(request.Password);

            User newUser = User.Create(
               firstnameResult.Value,
               lastnameResult.Value,
               emailResult.Value,
               phoneNumberResult.Value,
               passwordHash,
               request.ProfileType);

            var result = await _userRepository.AddAsync(newUser, cancellationToken);

            return result;
        }
    }
}
