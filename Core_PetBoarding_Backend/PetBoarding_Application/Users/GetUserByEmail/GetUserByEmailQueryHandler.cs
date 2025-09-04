using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserByEmail;

public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, User>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;

    public GetUserByEmailQueryHandler(IUserRepository userRepository, ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<User>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var emailResult = PetBoarding_Domain.Users.Email.Create(request.Email);
        if (emailResult.IsFailed)
            return Result.Fail(emailResult.Errors);


        var cacheKey = CacheKeys.Users.ByEmail(request.Email);

        // Get from cache or create if not exists.
        var user = await _cacheService.GetOrCreateAsync<User>(cacheKey, async () =>
        {
            var userFromDb = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

            return userFromDb;

        }, TimeSpan.FromMinutes(15), cancellationToken);

        var test = await _cacheService.ExistsAsync<User>(cacheKey, cancellationToken);

        if (user is null)
        {
            return Result.Fail(UserErrors.NotFound());
        }

        return Result.Ok(user);
    }
}
