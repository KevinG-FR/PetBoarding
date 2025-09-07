namespace PetBoarding_Application.Core.Caching;

public static class CacheKeys
{
    public static class Users
    {
        public static string ByEmail(string email) => $"user:email:{email}";
        public static string ById(Guid userId) => $"user:id:{userId}";
        public static string AllUsers() => "users:all";
    }

    public static class Prestations
    {
        public static string ById(Guid prestationId) => $"prestation:id:{prestationId}";
        public static string AllPrestations() => "prestations:all";
        public static string ByTypeAnimal(string typeAnimal) => $"prestations:type:{typeAnimal}";
    }

    public static class Planning
    {
        public static string ByPrestationId(Guid prestationId) => $"planning:prestation:{prestationId}";
        public static string AllPlannings() => "plannings:all";
    }

    public static class Pets
    {
        public static string ById(Guid petId) => $"pet:id:{petId}";
        public static string ByOwner(Guid ownerId) => $"pets:owner:{ownerId}";
    }

    public static class Baskets
    {
        public static string ByUser(Guid userId) => $"basket:user:{userId}";
    }

    public static class Sessions
    {
        public static string UserSession(Guid userId) => $"session:user:{userId}";
        public static string RefreshToken(string tokenId) => $"refresh:token:{tokenId}";
    }
}