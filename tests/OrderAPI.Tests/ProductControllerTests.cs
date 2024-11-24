using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Dtos;
using OrderAPI.Models;
using OrderAPI.Services;
using Xunit;

namespace OrderAPI.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductController _productController;

        public ProductControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _productController = new ProductController(_productServiceMock.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var productId = "6489f1f0c2e7c2eaf0b3f6a1";

            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 100.00M,
                Quantity = 300,
            };

            _productServiceMock
                .Setup(service => service.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.GetProductById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var productId = "6489f1f0c2e7c2eaf0b3f6a1";
            _productServiceMock
                .Setup(service => service.GetProductByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productController.GetProductById(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    Id = "6489f1f0c2e7c2eaf0b3f6a1",
                    Name = "Test Product 1",
                    Price = 100.00M,
                    Quantity = 300,
                },
                new Product
                {
                    Id = "6489f1f0c2e7c2eaf0b3f6a2",
                    Name = "Test Product 2",
                    Price = 200.00M,
                    Quantity = 400,
                },
            };

            _productServiceMock
                .Setup(service => service.GetAllProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _productController.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(products.Count, returnedProducts.Count);
        }

        public async Task CreateProductAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Test Product",
                Price = 100.00M,
                Quantity = 300,
            };

            var product = new Product
            {
                Id = "6489f1f0c2e7c2eaf0b3f6a1",
                Name = "Test Product",
                Price = 100.00M,
                Quantity = 300,
            };

            _productServiceMock
                .Setup(service => service.CreateProductAsync(It.IsAny<CreateProductDto>()))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.CreateProduct(createProductDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal(product.Id, returnedProduct.Id);
            Assert.Equal(product.Name, returnedProduct.Name);
            Assert.Equal(product.Price, returnedProduct.Price);
            Assert.Equal(product.Quantity, returnedProduct.Quantity);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidProduct_ReturnsNoContent()
        {
            // Arrange
            var productId = "6489f1f0c2e7c2eaf0b3f6a1";
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 100.00M,
                Quantity = 300,
            };

            _productServiceMock
                .Setup(service => service.UpdateProductAsync(product))
                .ReturnsAsync(true);

            // Act
            var result = await _productController.UpdateProduct(productId, product);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateProductAsync_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var productId = "6489f1f0c2e7c2eaf0b3f6a1";
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 100.00M,
                Quantity = 300,
            };

            _productServiceMock
                .Setup(service => service.UpdateProductAsync(product))
                .ReturnsAsync(false);

            // Act
            var result = await _productController.UpdateProduct(productId, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ValidId_ReturnsNoContent()
        {
            // Arrange
            var productId = "6489f1f0c2e7c2eaf0b3f6a1";

            _productServiceMock
                .Setup(service => service.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _productController.DeleteProduct(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProductAsync_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var productId = "6489f1f0c2e7c2eaf0b3f6a1";

            _productServiceMock
                .Setup(service => service.DeleteProductAsync(productId))
                .ReturnsAsync(false);

            // Act
            var result = await _productController.DeleteProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
