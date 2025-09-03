using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        
        public GetUserByIdQueryHandler(IUserRepository userRepository, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.Users.ById(request.UserId.Value);

            // Get from cache or create if not exists
            var user = await _cacheService.GetOrCreateAsync<User>(cacheKey, async () =>
            {
                var userFromDb = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                return userFromDb;
            }, TimeSpan.FromHours(1), cancellationToken);

            if (user is null)
                return Result.Fail(UserErrors.NotFound(request.UserId.Value));

            return Result.Ok(user);
        }
    }
}
