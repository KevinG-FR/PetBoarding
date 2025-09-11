using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.GetAdminAndEmployeeUsers
{
    public class GetAdminAndEmployeeUsersQueryHandler : IQueryHandler<GetAdminAndEmployeeUsersQuery, List<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetAdminAndEmployeeUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<List<User>>> Handle(GetAdminAndEmployeeUsersQuery request, CancellationToken cancellationToken)
        {
            // Récupérer tous les utilisateurs
            var allUsers = await _userRepository.GetAllAsync(cancellationToken);
            
            // Vérifier que l'utilisateur actuel est administrateur
            var currentUser = allUsers.FirstOrDefault(u => u.Id == request.CurrentUserId);
            if (currentUser == null || currentUser.ProfileType != UserProfileType.Administrator)
            {
                return Result.Fail("Access denied. Administrator privileges required.");
            }

            // Filtrer pour ne garder que les Admin et Employee, en excluant l'utilisateur actuel
            var filteredUsers = allUsers
                .Where(user => 
                    // Garder seulement Admin et Employee
                    (user.ProfileType == UserProfileType.Administrator || user.ProfileType == UserProfileType.Employee)
                    // Exclure l'utilisateur actuel
                    && user.Id != request.CurrentUserId
                    // Appliquer le filtre de profil si spécifié
                    && (request.ProfileTypeFilter == null || (int)user.ProfileType == request.ProfileTypeFilter))
                .ToList();

            return Result.Ok(filteredUsers);
        }
    }
}