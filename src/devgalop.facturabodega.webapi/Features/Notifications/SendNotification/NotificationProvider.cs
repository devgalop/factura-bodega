using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Notifications.Common;

namespace devgalop.facturabodega.webapi.Features.Notifications.SendNotification
{
    public record NotificationProviderOptions(string ApiUrl, string ApiKey, string Domain);
    public sealed class NotificationProvider(NotificationProviderOptions options)
    {
        public async Task<NotificationResponse> SendAsync(NotificationRequest request)
        {
            using HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(options.ApiUrl);
            var authToken = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"api:{options.ApiKey}")
            );
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authToken);
            
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", $"Learning Devgalop <lrn_devgalop@{options.Domain}>"),
                new KeyValuePair<string, string>("to", request.Recipient),
                new KeyValuePair<string, string>("subject", request.Subject),
                new KeyValuePair<string, string>("text", request.Body)
            });

            var response = await httpClient.PostAsync($"/v3/{options.Domain}/messages", form);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                return new NotificationResponse(false, $"Error sending notification: {errorMessage}");
            }
            return new NotificationResponse(true, "Notification sent successfully.");
        }
    }

    public static class NotificationProviderExtensions
    {
        public static WebApplicationBuilder RegisterNotificationProvider(this WebApplicationBuilder builder)
        {
            NotificationProviderOptions options = new NotificationProviderOptions(
                ApiUrl:builder.Configuration.GetValue<string>("NotificationProvider:ApiUrl")?? throw new ArgumentNullException("NotificationProvider:ApiUrl no está configurado apropiadamente en los appsettings."),
                ApiKey: builder.Configuration["NotificationProvider:ApiKey"] ?? throw new ArgumentNullException("NotificationProvider:ApiKey no está configurado apropiadamente en los appsettings."),
                Domain: builder.Configuration["NotificationProvider:Domain"] ?? throw new ArgumentNullException("NotificationProvider:Domain no está configurado apropiadamente en los appsettings.")
            );
            builder.Services.AddSingleton(options);
            builder.Services.AddScoped<NotificationProvider>();
            return builder;
        }
    }
}