using FluentResults;

using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Fail(UserErrors.NotFound(request.UserId.Value));

            return Result.Ok(user);
        }
    }
}
