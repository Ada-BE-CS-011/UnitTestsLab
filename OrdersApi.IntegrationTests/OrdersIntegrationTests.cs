using Microsoft.AspNetCore.Mvc.Testing;
using OrdersApi.Bad;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace OrdersApi.IntegrationTests
{
    public class OrdersIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly HttpClient _client;

        public OrdersIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreatesAnOrderSuccessfully()
        {
            var request = new CreateOrderRequest
            {
                Coupon = null,
                Items = [new() { ProductId = 1, Quantity = 1 }]
            };

            var response = await _client.PostAsJsonAsync("/api/orders", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var order = await response.Content.ReadFromJsonAsync<Order>();
            Assert.NotNull(order);
        }
    }
}
