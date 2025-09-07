using FluentResults;

using PetBoarding_Application.Core.Abstractions;

using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.GetAllUsers
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<User>>
    {
        private readonly IUserRepository _userRepository;
        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<List<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return Result.Ok(await _userRepository.GetAllAsync(cancellationToken));
        }
    }
}
