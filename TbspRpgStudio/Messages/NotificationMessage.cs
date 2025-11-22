using Avalonia.Controls.Notifications;

namespace TbspRpgStudio.Messages;

public class NotificationMessage(string message, NotificationType type)
{
    public string Message { get; } = message;
    public NotificationType Type { get; } = type;
}