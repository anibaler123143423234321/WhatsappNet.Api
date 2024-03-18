using OpenAI_API;
using OpenAI_API.Completions;

namespace WhatsappNet.Api.Services.OpenAI.ChatGPT
{
    public interface IChatGPTService
    {
        Task<string> Execute(string textUser);
    }
}
