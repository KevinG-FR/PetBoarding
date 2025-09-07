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
            var resultCheckEmailAlreadyUsed = await _userRepository.UserEmailAlreadyUsed(request.Email, cancellationToken);
            if (resultCheckEmailAlreadyUsed)
                return Result.Fail(UserErrors.EmailAlreadyUsed(request.Email));

            var passwordHash = _accountService.GetHashPassword(request.Password);

            User newUser = new User(
               Firstname.Create(request.Firstname).Value,
               Lastname.Create(request.Lastname).Value,
               PetBoarding_Domain.Users.Email.Create(request.Email).Value,
               PhoneNumber.Create(request.PhoneNumber).Value,
               passwordHash,
               request.ProfileType);

            var result = await _userRepository.AddAsync(newUser, cancellationToken);

            return result;
        }
    }
}
