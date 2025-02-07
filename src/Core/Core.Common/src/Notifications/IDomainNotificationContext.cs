namespace Tilray.Integrations.Core.Common.Notifications;

public enum DomainNotificationType
{
    Success = 1,
    Error = 2
}

public interface IDomainNotificationContext
{
    bool HasErrorNotifications { get; }
    void NotifyError(string message);
    void NotifySuccess(string message);
    IEnumerable<DomainNotification> GetErrorNotifications();
}

public class DomainNotificationContext : IDomainNotificationContext
{
    private readonly List<DomainNotification> _notifications;

    public DomainNotificationContext()
    {
        _notifications = new List<DomainNotification>();
    }

    public bool HasErrorNotifications
        => _notifications.Any(x => x.Type == DomainNotificationType.Error);

    public void NotifySuccess(string message)
        => Notify(message, DomainNotificationType.Success);

    public void NotifyError(string message)
        => Notify(message, DomainNotificationType.Error);

    private void Notify(string message, DomainNotificationType type)
        => _notifications.Add(new DomainNotification(type, message));

    public IEnumerable<DomainNotification> GetErrorNotifications()
        => _notifications.Where(x => x.Type == DomainNotificationType.Error).ToList();
}

public class DomainNotification : INotification
{
    public DomainNotificationType Type { get; protected set; }
    public string Value { get; protected set; }

    public DomainNotification(DomainNotificationType type, string value)
    {
        Type = type;
        Value = value;
    }
}
