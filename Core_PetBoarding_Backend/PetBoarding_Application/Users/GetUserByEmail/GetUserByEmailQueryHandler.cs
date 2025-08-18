using FluentResults;

using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserByEmail
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, User>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailed)
                return Result.Fail(emailResult.Errors);

            var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

            if (user is null)
                return Result.Fail(UserErrors.NotFound());

            return Result.Ok(user);
        }
    }
}
