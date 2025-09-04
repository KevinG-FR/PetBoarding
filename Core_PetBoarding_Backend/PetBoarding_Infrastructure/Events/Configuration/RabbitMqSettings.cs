namespace PetBoarding_Infrastructure.Events.Configuration;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMQ";
    
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = string.Empty;
}