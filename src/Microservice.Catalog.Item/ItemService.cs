using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RMQ = RabbitMQ.Client;

namespace Microservice.Catalog.Item;

internal class ItemService(ILogger<ItemService> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken Token)
    {
        var factory = new RMQ.ConnectionFactory
        {
            HostName = "",
            UserName = "",
            Password = "",
            AutomaticRecoveryEnabled = true
        };

        const string queue = "Catalog.Item";

        while (!Token.IsCancellationRequested)
        {
            try
            {
                await using var conn = await factory.CreateConnectionAsync(Token);
                await using var channel = await conn.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false,
                                                    arguments: null, cancellationToken: Token);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, args) =>
                {
                    try
                    {
                        var text = Encoding.UTF8.GetString(args.Body.ToArray());
                        log.LogInformation($"brand <- {text}");
                        await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: Token);
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Error at [BrandService] incomming messages: {ex}");
                        await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: Token);
                    }
                };

                _ = await channel.BasicConsumeAsync(queue, autoAck: false, consumerTag: string.Empty,
                            noLocal: false, exclusive: false, arguments: null, consumer: consumer,
                            cancellationToken: Token);

                log.LogInformation($"listening on {queue}");

                await Task.Delay(Timeout.Infinite, Token);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex} Brand consumer crashed; retrying in 3s");
                await Task.Delay(3000, Token);
            }
        }
    }
}
