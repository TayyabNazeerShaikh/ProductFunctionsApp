using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductFunctionsApp.Application.DTOs;
using ProductFunctionsApp.Application.Interfaces;

namespace ProductFunctionsApp.Api.Functions;

public class QueueProductFunctions
{
    private readonly ILogger<QueueProductFunctions> _logger;
    private readonly IProductService _productService;

    public QueueProductFunctions(
        ILogger<QueueProductFunctions> logger,
        IProductService productService
    )
    {
        _logger = logger;
        _productService = productService;
    }

    // This function is triggered when a new message arrives in the "products-to-create" queue.
    // It creates a product and then outputs a message to the "products-processed" queue.
    [Function("CreateProductFromQueue")]
    [QueueOutput("products-processed")] // Output binding
    public async Task<ProductProcessedDto> Run(
        [QueueTrigger("products-to-create")] CreateProductDto createDto
    )
    {
        _logger.LogInformation($"C# Queue trigger function processed: {createDto.Name}");

        var createdProduct = await _productService.CreateProductAsync(createDto);

        _logger.LogInformation($"Product created with ID: {createdProduct.Id}");

        // Return a DTO to be placed on the output queue
        return new ProductProcessedDto
        {
            ProductId = createdProduct.Id,
            Status = "SuccessfullyProcessed",
            ProcessedAt = DateTime.UtcNow,
        };
    }
}
