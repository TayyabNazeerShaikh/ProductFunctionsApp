using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductFunctionsApp.Application.Interfaces;
using ProductFunctionsApp.Application.Services;
using ProductFunctionsApp.Infrastructure.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
    })
    .Build();

await host.RunAsync();
