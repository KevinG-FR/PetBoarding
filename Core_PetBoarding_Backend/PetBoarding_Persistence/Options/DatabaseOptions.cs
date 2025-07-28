namespace PetBoarding_Persistence.Options
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int MaxRetry { get; set; }
        public int CommandTimeout { get; set; }
        public bool EnableDetailedErrors { get; set; }
        public bool EnableSensitiveDataLogging {get; set; }
    }
}