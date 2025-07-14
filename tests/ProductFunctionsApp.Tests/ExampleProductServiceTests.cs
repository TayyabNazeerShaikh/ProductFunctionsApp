using Moq;
using ProductFunctionsApp.Application.DTOs;
using ProductFunctionsApp.Application.Interfaces;
using ProductFunctionsApp.Application.Services;
using ProductFunctionsApp.Domain.Entities;

namespace ProductFunctionsApp.Tests;

public class ExampleProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly IProductService _productService;

    public ExampleProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _productService = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 9.99m,
        };
        _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnCreatedProductDto()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Widget",
            Description = "A shiny new widget.",
            Price = 100.50m,
        };

        // We need to capture the product passed to AddAsync to ensure its properties are set correctly
        Product? capturedProduct = null;
        _mockRepo
            .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .ReturnsAsync(() => capturedProduct!);

        // Act
        var result = await _productService.CreateProductAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Price, result.Price);
        Assert.NotEqual(Guid.Empty, result.Id);

        _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }
}
