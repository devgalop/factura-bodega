using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Notifications.Common;

namespace devgalop.facturabodega.webapi.Features.Notifications.SendNotification
{
    public interface INotificationParams{}
    public record NotificationAddress(string Name, string Email);
    public record NotificationContent(
        string Subject, 
        string HtmlContent, 
        NotificationAddress Sender, 
        List<NotificationAddress> To);
    
    public record NotificationProviderOptions(string ApiUrl, string ApiKey);

    public sealed class NotificationProvider(NotificationProviderOptions options)
    {
        public async Task<NotificationResponse> SendAsync(NotificationContent request)
        {
            using HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(options.ApiUrl);

            httpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
            string body = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
            StringContent content = new(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/v3/smtp/email", content);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error sending notification: {errorMessage}");
                return new NotificationResponse(false, $"Error al enviar notificación: {errorMessage}");
            }
            return new NotificationResponse(true, "La notificación ha sido enviada exitosamente.");
        }
    }

    public static class NotificationProviderExtensions
    {
        public static WebApplicationBuilder RegisterNotificationProvider(this WebApplicationBuilder builder)
        {
            NotificationProviderOptions options = new NotificationProviderOptions(
                ApiUrl:builder.Configuration.GetValue<string>("NotificationProvider:ApiUrl")?? throw new ArgumentNullException("NotificationProvider:ApiUrl no está configurado apropiadamente en los appsettings."),
                ApiKey: builder.Configuration["NotificationProvider:ApiKey"] ?? throw new ArgumentNullException("NotificationProvider:ApiKey no está configurado apropiadamente en los appsettings.")
            );
            builder.Services.AddSingleton(options);
            builder.Services.AddScoped<NotificationProvider>();
            return builder;
        }
    }
}