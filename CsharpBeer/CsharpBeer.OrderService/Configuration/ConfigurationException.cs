namespace CsharpBeer.OrderService.Configuration;

public class ConfigurationException(string message) : Exception(message)
{
    public ConfigurationException() : this("Occured error on configuring startup of service")
    {
    }
}