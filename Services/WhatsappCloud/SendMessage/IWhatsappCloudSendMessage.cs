namespace WhatsappNet.Api.Services.WhatsappCloud.SendMessage
{
    public interface IWhatsappCloudSendMessage
    {
        Task<bool> Execute(object model);
    }
}
