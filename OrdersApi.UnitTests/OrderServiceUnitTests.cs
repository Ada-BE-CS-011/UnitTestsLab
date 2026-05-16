using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using OrdersApi.Bad.Application.Services;
using OrdersApi.Bad.Controllers;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Domain.Interfaces;
using OrdersApi.Bad.Dtos;
using OrdersApi.Bad.Infrastructure.Gateways;
using OrdersApi.Bad.Infrastructure.Repositories;

namespace OrdersApi.UnitTests
{
    public class OrderServiceUnitTests
    {
        private Mock<IOrderRepository> orderRepository;
        private Mock<IProductRepository> productRepository;
        private Mock<IPaymentGateway> paymentGateway;

        public OrderServiceUnitTests()
        {
            orderRepository = new Mock<IOrderRepository>();
            productRepository = new Mock<IProductRepository>();
            paymentGateway = new Mock<IPaymentGateway>();
        }

        [Fact]
        public void CreateOrder_WhenOrderIsNull_ShouldReturnError()
        {
            //var service = new OrderService(productRepository.Object, orderRepository.Object, paymentGateway.Object);
            var service = new OrderService(null, null, null);

            var actual = service.CreateOrder(null);

            Assert.False(actual.IsSuccess);
            Assert.NotEmpty(actual.Errors);
            Assert.Contains("request is null", actual.FirstError.Description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateOrder_WhenItemsIsNull_ShouldReturnError()
        {
            var service = new OrderService(null, null, null);
            var order = new CreateOrderRequest() { };

            var actual = service.CreateOrder(order);

            Assert.False(actual.IsSuccess);
            Assert.NotEmpty(actual.Errors);
            Assert.Contains("at least one item", actual.FirstError.Description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateOrder_WhenItemsIsEmpty_ShouldReturnError()
        {
            var service = new OrderService(null, null, null);
            var order = new CreateOrderRequest() { Items = [] };

            var actual = service.CreateOrder(order);

            Assert.False(actual.IsSuccess);
            Assert.NotEmpty(actual.Errors);
            Assert.Contains("at least one item", actual.FirstError.Description, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateOrder_WhenItemQuantityIsInvalid_ShouldReturnError(int quantity)
        {
            var service = new OrderService(null, null, null);
            var order = new CreateOrderRequest() { Items = [new() { Quantity = quantity }] };

            var actual = service.CreateOrder(order);

            Assert.False(actual.IsSuccess);
            Assert.NotEmpty(actual.Errors);
            Assert.Contains("quantity must be greater then zero", actual.FirstError.Description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateOrder_WhenItemNotFound_ShouldReturnError()
        {
            var service = new OrderService(productRepository.Object, null, null);
            var order = new CreateOrderRequest() { Items = [new() { Quantity = 1, ProductId = 1 }] };

            var actual = service.CreateOrder(order);

            Assert.False(actual.IsSuccess);
            Assert.NotEmpty(actual.Errors);
            Assert.Contains("not found", actual.FirstError.Description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(ErrorType.NotFound, actual.FirstError.Type);
        }


        [Fact]
        public void CreateOrder_WhenItemIsOutOfStock_ShouldReturnError()
        {
            var product = new Product() { Stock = 0, Name = "Test Product" };
            var order = new CreateOrderRequest() { Items = [new() { Quantity = 1, ProductId = product.Id }] };

            var service = new OrderService(productRepository.Object, null, null);
            productRepository.Setup(x => x.GetById(product.Id)).Returns(product);

            var actual = service.CreateOrder(order);

            Assert.False(actual.IsSuccess);
            Assert.NotEmpty(actual.Errors);
            Assert.Contains("out of stock", actual.FirstError.Description, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(ErrorType.Unexpected, actual.FirstError.Type);
        }

        [Fact]
        public void CreateOrder_WhenPaymentFails_ShouldReturnRejectedOrder()
        {
            // Arrange
            var products = CreateTestProducts((id: 1, price: 100m, stock: 1));
            var orderRequest = CreateOrderRequest((productId: 1, quantity: 1));

            SetupProductRepository(products);
            SetupPaymentGatewayFailure();
            SetupOrderRepositoryToReturnSameOrder();

            var service = new OrderService(productRepository.Object, orderRepository.Object, paymentGateway.Object);

            // Act
            var result = service.CreateOrder(orderRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("Rejected", result.Value.Status, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateOrder_WhenPaymentSucceed_ShouldReturnApprovedOrder()
        {
            // Arrange
            var products = CreateTestProducts((id: 1, price: 100m, stock: 1));
            var orderRequest = CreateOrderRequest((productId: 1, quantity: 1));

            SetupProductRepository(products);
            SetupPaymentGatewaySuccess();
            SetupOrderRepositoryToReturnSameOrder();

            var service = new OrderService(productRepository.Object, orderRepository.Object, paymentGateway.Object);

            // Act
            var result = service.CreateOrder(orderRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("Approved", result.Value.Status, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateOrder_WhenOrderApproved_ShouldCalculateSubtotalCorrectly()
        {
            // Arrange
            const decimal expectedSubtotal = 600m;
            var products = CreateTestProducts(
                (id: 1, price: 100m, stock: 1),
                (id: 2, price: 200m, stock: 1),
                (id: 3, price: 300m, stock: 1)
            );

            var orderRequest = CreateOrderRequest(
                (productId: 1, quantity: 1),
                (productId: 2, quantity: 1),
                (productId: 3, quantity: 1)
            );

            SetupProductRepository(products);
            SetupPaymentGatewaySuccess();
            SetupOrderRepositoryToReturnSameOrder();

            var service = new OrderService(productRepository.Object, orderRepository.Object, paymentGateway.Object);

            // Act
            var result = service.CreateOrder(orderRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedSubtotal, result.Value?.Subtotal);
        }

        #region Helper Methods

        private Product[] CreateTestProducts(params (int id, decimal price, int stock)[] productData)
        {
            return productData.Select(p => new Product
            {
                Id = p.id,
                Name = $"Test Product {p.id}",
                Price = p.price,
                Stock = p.stock
            }).ToArray();
        }

        private CreateOrderRequest CreateOrderRequest(params (int productId, int quantity)[] items)
        {
            return new CreateOrderRequest
            {
                Items = items.Select(i => new CreateOrderItemRequest
                {
                    ProductId = i.productId,
                    Quantity = i.quantity
                }).ToList()
            };
        }

        private void SetupProductRepository(params Product[] products)
        {
            foreach (var product in products)
            {
                productRepository.Setup(x => x.GetById(product.Id)).Returns(product);
            }
        }

        private void SetupPaymentGatewaySuccess()
        {
            paymentGateway.Setup(x => x.Pay(It.IsAny<decimal>())).Returns(true);
        }

        private void SetupPaymentGatewayFailure()
        {
            paymentGateway.Setup(x => x.Pay(It.IsAny<decimal>())).Returns(false);
        }

        private void SetupOrderRepositoryToReturnSameOrder()
        {
            orderRepository.Setup(x => x.Add(It.IsAny<Order>())).Returns((Order order) => 
            {
                order.Id = Random.Shared.Next(int.MaxValue);
                return order;
            });
        }

        #endregion
    }
}

