using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WhatsappNet.Api.Services.OpenAI.ChatGPT
{
    public class ChatGPTService : IChatGPTService
    {
        public async Task<string> Execute(string textUser)
        {
            try
            {
                string apiKey = "sk-API";
                string requestBody = "{\"messages\": [{\"role\": \"user\",\"content\": \"" + textUser + "\"}], \"model\": \"gpt-3.5-turbo\", \"temperature\": 0.5, \"max_tokens\": 100}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse JSON response to extract only the message content
                    var jsonResponse = JObject.Parse(responseBody);
                    var messageContent = jsonResponse["choices"][0]["message"]["content"].ToString();
                    return messageContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la llamada a la API de OpenAI: {ex.Message}");
                return "Error al comunicarse con el servicio de ChatGPT";
            }
        }
    }
}
