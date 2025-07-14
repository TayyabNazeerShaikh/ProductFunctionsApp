using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Storage.Blobs;
using Microsoft.Extensions.Logging;
using ProductFunctionsApp.Application.DTOs;
using ProductFunctionsApp.Application.Interfaces;

namespace ProductFunctionsApp.Api.Functions;

public class BlobProductFunctions
{
    private readonly ILogger<BlobProductFunctions> _logger;
    private readonly IProductService _productService;

    public BlobProductFunctions(
        ILogger<BlobProductFunctions> logger,
        IProductService productService
    )
    {
        _logger = logger;
        _productService = productService;
    }

    // This function is triggered when a new blob is created in the "product-uploads" container.
    // It expects a simple CSV format: Name,Description,Price
    [Function("ProcessProductUpload")]
    public async Task Run([BlobTrigger("product-uploads/{name}")] byte[] fileContent, string name)
    {
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name}");

        using var reader = new StreamReader(new MemoryStream(fileContent), Encoding.UTF8);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var parts = line.Split(',');
            if (parts.Length == 3 && decimal.TryParse(parts[2], out var price))
            {
                var dto = new CreateProductDto
                {
                    Name = parts[0],
                    Description = parts[1],
                    Price = price,
                };
                await _productService.CreateProductAsync(dto);
                _logger.LogInformation($"Created product from blob: {dto.Name}");
            }
            else
            {
                _logger.LogWarning($"Skipping invalid line in blob {name}: {line}");
            }
        }
    }
}
