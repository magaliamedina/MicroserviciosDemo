using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class PaymentServiceClient
    {
        private readonly HttpClient _httpClient;

        public PaymentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ProcessPaymentAsync(object paymentRequest)
        {
            var json = JsonSerializer.Serialize(paymentRequest);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "/api/payments",
                content
            );

            return await response.Content.ReadAsStringAsync();
        }
    }
}