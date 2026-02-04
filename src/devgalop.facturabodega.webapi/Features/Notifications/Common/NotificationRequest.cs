using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Notifications.Common
{
    public record NotificationRequest(
        string Subject,
        string Body,
        string Recipient,
        string Sender
    );

    public record NotificationResponse(bool IsSuccessful, string Message);
}