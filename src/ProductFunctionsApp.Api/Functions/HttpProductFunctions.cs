using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductFunctionsApp.Application.DTOs;
using ProductFunctionsApp.Application.Interfaces;

namespace ProductFunctionsApp.Api.Functions;

public class HttpProductFunctions
{
    private readonly ILogger<HttpProductFunctions> _logger;
    private readonly IProductService _productService;

    public HttpProductFunctions(
        ILogger<HttpProductFunctions> logger,
        IProductService productService
    )
    {
        _logger = logger;
        _productService = productService;
    }

    [Function("CreateProduct")]
    public async Task<HttpResponseData> CreateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequestData req
    )
    {
        _logger.LogInformation("C# HTTP trigger function processed a request to create a product.");
        var createDto = await req.ReadFromJsonAsync<CreateProductDto>();
        if (createDto == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var productDto = await _productService.CreateProductAsync(createDto);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(productDto);
        return response;
    }

    [Function("GetProducts")]
    public async Task<HttpResponseData> GetProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestData req
    )
    {
        _logger.LogInformation("C# HTTP trigger function processed a request to get all products.");
        var products = await _productService.GetAllProductsAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        return response;
    }

    [Function("GetProductById")]
    public async Task<HttpResponseData> GetProductById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id:guid}")]
            HttpRequestData req,
        Guid id
    )
    {
        _logger.LogInformation(
            $"C# HTTP trigger function processed a request to get product with ID: {id}"
        );
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product);
        return response;
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> UpdateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id:guid}")]
            HttpRequestData req,
        Guid id
    )
    {
        _logger.LogInformation(
            $"C# HTTP trigger function processed a request to update product with ID: {id}"
        );
        var updateDto = await req.ReadFromJsonAsync<UpdateProductDto>();
        if (updateDto == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var updatedProduct = await _productService.UpdateProductAsync(id, updateDto);
        if (updatedProduct == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updatedProduct);
        return response;
    }

    [Function("DeleteProduct")]
    public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id:guid}")]
            HttpRequestData req,
        Guid id
    )
    {
        _logger.LogInformation(
            $"C# HTTP trigger function processed a request to delete product with ID: {id}"
        );
        var success = await _productService.DeleteProductAsync(id);

        return req.CreateResponse(success ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
    }
}
