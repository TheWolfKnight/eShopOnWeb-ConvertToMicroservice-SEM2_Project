using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using CatalogMicroservice.RabbitMQ.Model;
using CatalogMicroservice.RabbitMQ.Interfaces;

namespace CatalogMicroservice.RabbitMQ.Sender;

public class CatalogSender : ICatalogSender
{
    private readonly IChannel _ch;
    private readonly string _exchange;

    //Det styrer, hvordan egenskabsnavne (property names)  navngives i JSON.
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // IChannel leveres udefra (håndteres højere oppe i systemet)
    public CatalogSender(IChannel channel, string exchange = "catalog.router")
    {
        _ch = channel ?? throw new ArgumentNullException(nameof(channel));
        _exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
    }

    // CRUD-facader
    public Task<string> CreateAsync(Command command, string claimKey)
        => SendCoreAsync(command, claimKey, "create");

    public Task<string> GetAsync(Command command, string claimKey)
        => SendCoreAsync(command, claimKey, "get.one");

    public Task<string> GetAllAsync(Command command, string claimKey)
        => SendCoreAsync(command, claimKey, "get.all");

    public Task<string> UpdateAsync(Command command, string claimKey)
        => SendCoreAsync(command, claimKey, "update");

    public Task<string> DeleteAsync(Command command, string claimKey)
        => SendCoreAsync(command, claimKey, "delete");


    // Core publisher — body = JSON(command), headers = redis claim + type hints
    private async Task<string> SendCoreAsync(Command command, string claimKey, string routingKey)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        if (string.IsNullOrWhiteSpace(claimKey)) throw new ArgumentException("claimKey required", nameof(claimKey));

        var corrId = Guid.NewGuid().ToString("N");
        var type = command.GetType();

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command, type, _json));

        var props = new BasicProperties
        {
            CorrelationId = corrId,
            ContentType = "application/json",
            Headers = new Dictionary<string, object>
            {
                ["x-claim-key"] = Encoding.UTF8.GetBytes(claimKey),
                ["x-command-type"] = Encoding.UTF8.GetBytes(type.FullName ?? type.Name),
                ["x-command-simple"] = Encoding.UTF8.GetBytes(type.Name)
            }
        };

        await _ch.BasicPublishAsync(
            exchange: _exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );

        return corrId;
    }
}
