
namespace InvestigaIA.API
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using InvestigaIA.Classes;

    /// <summary>
    /// Provides methods to interact with the Gemini API for AI responses.
    /// </summary>
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _APIKey;

        private List<Content> _conversationHistory = new List<Content>();

        public GeminiService(string APIKey, HttpClient HttpClient)
        {
            _APIKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));
            _httpClient = HttpClient;
        }

        public async Task<APIResponse> SendRequestAsync(APIRequest request)
        {
            try
            {
                _conversationHistory.AddRange(request.contents);
                request.contents = _conversationHistory;

                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/v1beta/models/gemini-2.0-flash:generateContent?key={_APIKey}", httpContent);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<APIResponse>();


                _conversationHistory.Add(responseBody.candidates[0].content);

                return responseBody;

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                throw; // Re-throw the exception for further handling if necessary

            }

        }


        public async Task<APIResponse> Ask(APIRequest request, Suspeito suspeito)
        {
            try
            {
                suspeito._conversationHistory.AddRange(request.contents);
                request.contents = suspeito._conversationHistory;

                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/v1beta/models/gemini-2.0-flash:generateContent?key={_APIKey}", httpContent);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<APIResponse>();

                suspeito._conversationHistory.Add(responseBody.candidates[0].content);

                return responseBody;

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                throw; // Re-throw the exception for further handling if necessary

            }

        }



        private HttpClient HttpClienteFactory(string APIUrl)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(APIUrl)

            };

            client.DefaultRequestHeaders.Add("Accept", "application/json");


            return client;


        }


    }
}