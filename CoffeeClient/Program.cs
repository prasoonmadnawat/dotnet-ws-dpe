using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoffeeClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AuthenticationResponse authResponse = await GetAuthTokenAsync();

            await OrderCoffeeAsync(authResponse);
        }

        private static async Task<AuthenticationResponse> GetAuthTokenAsync()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("");
            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token");
            var requestBody = "{\"client_id\":\"\",\"client_secret\":\"\",\"audience\":\"http://localhost:5000\",\"grant_type\":\"client_credentials\"}";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);

            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            var authResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(
                Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length));
            return authResponse;
        }

        private static async Task OrderCoffeeAsync(AuthenticationResponse authResponse)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                // NB: You should make this more robust by actually checking the certificate:
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                var httpClient2 = new HttpClient(httpClientHandler);
                httpClient2.BaseAddress = new Uri("http://localhost:5000");
                var request2 = new HttpRequestMessage(HttpMethod.Post, "/api/coffee");
                request2.Headers.Add("Authorization", $"Bearer {authResponse.access_token}");
                var coffeeRequest = new CoffeeOrder()
                {
                    CoffeeType = "Long Black",
                    MilkType = "none",
                    Sugar = 0
                };

                var requestBody2 = JsonConvert.SerializeObject(coffeeRequest);

                request2.Content = new StringContent(requestBody2, Encoding.UTF8, "application/json");
                var response2 = await httpClient2.SendAsync(request2);
                var responseString = await response2.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
        }
    }

    public class AuthenticationResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class CoffeeOrder
    {
        public string CoffeeType { get; set; }

        public int Sugar { get; set; }

        public string MilkType { get; set; }
    }
}
