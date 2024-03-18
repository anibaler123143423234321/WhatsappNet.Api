using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WhatsappNet.Api.Services.WhatsappCloud.SendMessage
{
    public class WhatsappCloudSendMessage : IWhatsappCloudSendMessage
    {
        public async Task<bool> Execute(object model)
        {
            try
            {
                var client = new HttpClient();
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

                using (var content = new ByteArrayContent(byteData))
                {
                    string endpoint = "https://graph.facebook.com";
                    string phoneNumberId = "154748354392397";
                    string accessToken = "API";
                    string uri = $"{endpoint}/v17.0/{phoneNumberId}/messages";

                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var response = await client.PostAsync(uri, content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Mensaje enviado exitosamente.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el mensaje: {responseBody}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el mensaje: {ex.Message}");
                return false;
            }
        }
    }
}
