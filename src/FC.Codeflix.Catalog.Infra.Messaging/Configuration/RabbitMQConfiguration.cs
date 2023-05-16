namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;
public class RabbitMQConfiguration
{
    public const string ConfigurationSection = "RabbitMQ";
    public string? Hostname { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Exchange { get; set; }
}
