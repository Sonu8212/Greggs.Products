using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Helpers;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Greggs.Products.Api.DTO.ProductDTO;
using static Greggs.Products.Api.DTO.ProductEuroDTO;

namespace Greggs.Products.UnitTests;

public class ProductsApiTests
{
    #region ProductService Tests

    [Fact]
    public async Task ProductService_GetProductsAsync_ReturnsMappedProducts()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var mockConverter = new Mock<ICurrencyConverter>();
        var service = new ProductService(mockDataAccess.Object, mockConverter.Object);

        var products = new List<Product>
        {
            new Product { Name = "Sausage Roll", PriceInPounds = 1m },
            new Product { Name = "Steak Bake", PriceInPounds = 1.2m }
        };

        mockDataAccess.Setup(d => d.ListAsync(0, 2, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(products);

        // Act
        var result = await service.GetProductsAsync(0, 2);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("Sausage Roll", result.First().Name);
        Assert.Equal(1m, result.First().PriceInPounds);
    }

    [Fact]
    public async Task ProductService_GetProductsInEurosAsync_ReturnsConvertedPrices()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var mockConverter = new Mock<ICurrencyConverter>();
        var service = new ProductService(mockDataAccess.Object, mockConverter.Object);

        var products = new List<Product>
        {
            new Product { Name = "Yum Yum", PriceInPounds = 0.7m }
        };

        mockDataAccess.Setup(d => d.ListAsync(0, 1, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(products);

        // Mock currency conversion
        mockConverter.Setup(c => c.ConvertToEur(0.7m)).Returns(0.78m);

        // Act
        var result = await service.GetProductsInEurosAsync(0, 1);

        // Assert
        var productEuro = result.First();
        Assert.Equal("Yum Yum", productEuro.Name);
        Assert.Equal(0.7m, productEuro.PriceInPounds);
        Assert.Equal(0.78m, productEuro.PriceInEuros);
    }

    [Fact]
    public async Task ProductService_Pagination_WorksCorrectly()
    {
        // Arrange
        var mockDataAccess = new Mock<IDataAccess<Product>>();
        var products = Enumerable.Range(1, 5)
            .Select(i => new Product { Name = $"Product {i}", PriceInPounds = i })
            .ToList();

        mockDataAccess.Setup(d => d.ListAsync(2, 2, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(products.Skip(2).Take(2));

        var service = new ProductService(mockDataAccess.Object, Mock.Of<ICurrencyConverter>());

        // Act
        var result = await service.GetProductsAsync(2, 2);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("Product 3", result.First().Name);
        Assert.Equal("Product 4", result.Last().Name);
    }

    #endregion

    #region ProductController Tests

    [Fact]
    public async Task ProductController_Get_ReturnsOkWithProducts()
    {
        // Arrange
        var mockService = new Mock<IProductService>();
        var logger = new Mock<ILogger<ProductController>>();
        var controller = new ProductController(logger.Object, mockService.Object);

        var products = new List<ProductDto>
        {
            new ProductDto("Sausage Roll", 1m),
            new ProductDto("Steak Bake", 1.2m)
        };

        mockService.Setup(s => s.GetProductsAsync(0, 2, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(products);

        // Act
        var result = await controller.Get(0, 2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task ProductController_GetInEuros_ReturnsOkWithProductsInEuros()
    {
        // Arrange
        var mockService = new Mock<IProductService>();
        var logger = new Mock<ILogger<ProductController>>();
        var controller = new ProductController(logger.Object, mockService.Object);

        var products = new List<ProductEuroDto>
        {
            new ProductEuroDto("Yum Yum", 0.7m, 0.78m)
        };

        mockService.Setup(s => s.GetProductsInEurosAsync(0, 1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(products);

        // Act
        var result = await controller.GetInEuros(0, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<ProductEuroDto>>(okResult.Value);
        var product = returned.First();
        Assert.Equal("Yum Yum", product.Name);
        Assert.Equal(0.78m, product.PriceInEuros);
    }

    #endregion
}