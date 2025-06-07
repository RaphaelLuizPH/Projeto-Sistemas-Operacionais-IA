using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestigaIA.API.OpenAI;
using InvestigaIA.Classes;
namespace InvestigaIA.API
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;

        private List<Message> _messages = new List<Message>();
        public OpenAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<string> Ask(string prompt, Suspeito suspeito)
        {
            var newMessage = new Message
            {
                role = "user",
                content = prompt
            };


            var Messages = suspeito._conversationHistory.Select(c => c.parts).SelectMany(p => p)
                 .Select(m => new Message
                 {
                     role = "user",
                     content = m.text
                 }).ToList();

            Messages.Add(newMessage);

            var request = new OpenAiRequest()
            {
                messages = Messages

            };

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var candidates = System.Text.Json.JsonSerializer.Deserialize<OpenAiReponse>(jsonResponse);
                return candidates.choices.FirstOrDefault()?.message.content ?? "No response received.";

            }

            throw new Exception($"Error: {response.StatusCode}");
        }


    


        public async Task<string> SendRequestAsync(string prompt)
        {
            var newMessage = new Message
            {
                role = "user",
                content = prompt
            };


            _messages.Add(newMessage);

            var request = new OpenAiRequest()
            {
                messages = _messages

            };

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var candidates = System.Text.Json.JsonSerializer.Deserialize<OpenAiReponse>(jsonResponse);
                return candidates.choices.FirstOrDefault()?.message.content ?? "No response received.";

            }

            throw new Exception($"Error: {response.StatusCode}");
        }
    }
}