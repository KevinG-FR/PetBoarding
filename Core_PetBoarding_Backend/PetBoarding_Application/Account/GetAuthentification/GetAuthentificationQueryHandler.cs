using FluentResults;

using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Application.Account.GetAuthentification
{
    public class GetAuthentificationQueryHandler : IQueryHandler<GetAuthentificationQuery, AuthenticateTokens>
    {
        private readonly IAccountService _accountService;

        public GetAuthentificationQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<Result<AuthenticateTokens>> Handle(GetAuthentificationQuery request, CancellationToken cancellationToken)
        {
            var token = await _accountService.Authenticate(new AuthenticationRequest(request.Email, request.PasswordHash, request.RememberMe), cancellationToken);

            if (token is null)
                return Result.Fail(UserErrors.AuthentificationFailed());

            return Result.Ok(token);
        }
    }
}
