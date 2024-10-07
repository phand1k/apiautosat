namespace AvtoMigBussines.Interfaces
{
    public interface INotificationAction
    {
        int Id { get; set; }
        string ActionType { get; } // Например, 'DetailingOrder', 'WashOrder' и т.д.
    }
}
