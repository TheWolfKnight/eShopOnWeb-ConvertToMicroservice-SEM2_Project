using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microservice.Catalog.Item;

internal class Program
{
    static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args).ConfigureServices(services =>
        {
            services.AddHostedService<ItemService>();
        })
        .Build()
        .RunAsync();
    }
}
