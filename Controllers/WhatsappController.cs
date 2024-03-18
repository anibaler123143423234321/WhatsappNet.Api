using Microsoft.AspNetCore.Mvc;
using WhatsappNet.Api.Models.WhatsappCloud;
using WhatsappNet.Api.Services.OpenAI.ChatGPT;
using WhatsappNet.Api.Services.WhatsappCloud.SendMessage;
using WhatsappNet.Api.Util;

namespace WhatsappNet.Api.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhatsappController : Controller
    {
        private readonly IWhatsappCloudSendMessage _whatsappCloudSendMessage;
        private readonly IUtil _util;
        private readonly IChatGPTService _chatGPTService;
        public WhatsappController(IWhatsappCloudSendMessage whatsappCloudSendMessage, IUtil util, IChatGPTService chatGPTService)
        {
            _whatsappCloudSendMessage = whatsappCloudSendMessage;
            _util = util;
            _chatGPTService = chatGPTService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Sample()
        {
            var data = new
            {
                messaging_product = "whatsapp",
                to = "51958875176",
                type = "text",
                text = new
                {
                    body = "este es un mensaje de prueba"
                }
            };

            var result = await _whatsappCloudSendMessage.Execute(data);


            return Ok("ok sample");
        }

        [HttpGet]
        public IActionResult VerifyToken()
        {
            string AccessToken = "50DD88RRREREREREFDVCN38DU3JJODJLDH";

            var token = Request.Query["hub.verify_token"].ToString();
            var challenge = Request.Query["hub.challenge"].ToString();

            if(challenge != null && token != null && token == AccessToken)
            {
                return Ok(challenge);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReceivedMessage([FromBody] WhatsAppCloudModel body)
        {
            try
            {
                var Message = body.Entry[0]?.Changes[0]?.Value?.Messages[0];
                if (Message != null)
                {
                    var userNumber = Message.From;
                    var userText = GetUserText(Message);

                    List<object> listObjectMessage = new List<object>();

                    var responseChatGPT = await _chatGPTService.Execute(userText);

                    if (responseChatGPT != null)
                    {
                        var objectMesage = _util.TextMessage(responseChatGPT, userNumber);
                        listObjectMessage.Add(objectMesage);
                    }
                    else
                    {
                        // Handle the case where responseChatGPT is null
                        var errorMessage = _util.TextMessage("Lo siento, no pude entender tu pregunta. Por favor, inténtalo de nuevo.", userNumber);
                        listObjectMessage.Add(errorMessage);
                    }

                    foreach (var item in listObjectMessage)
                    {
                        await _whatsappCloudSendMessage.Execute(item);
                    }
                }

                return Ok("EVENT_RECEIVED");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error en la recepción del mensaje: {ex.Message}");
                return StatusCode(500); // Internal Server Error
            }
        }


        private string GetUserText(Message message)
        {
            string TypeMessage = message.Type;

            if(TypeMessage.ToUpper() == "TEXT")
            {
                return message.Text.Body;
            }
            else if (TypeMessage.ToUpper() == "INTERACTIVE")
            {
                string interactiveType = message.Interactive.Type;

                if(interactiveType.ToUpper() == "LIST_REPLY")
                {
                    return message.Interactive.List_Reply.Title;
                }
                else if (interactiveType.ToUpper() == "BUTTON_REPLY")
                {
                    return message.Interactive.Button_Reply.Title;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
