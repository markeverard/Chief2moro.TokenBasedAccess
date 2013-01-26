namespace POSSIBLE.TokenBasedAccess
{
    public interface INotificationService
    {
        bool Send(string to, string subject, string body);
    }
}