using WhatsappNet.Api.Services.OpenAI.ChatGPT;
using WhatsappNet.Api.Services.WhatsappCloud.SendMessage;
using WhatsappNet.Api.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IWhatsappCloudSendMessage, WhatsappCloudSendMessage>();
builder.Services.AddSingleton<IUtil, Util>();
builder.Services.AddSingleton<IChatGPTService, ChatGPTService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
