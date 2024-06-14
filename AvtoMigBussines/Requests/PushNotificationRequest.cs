namespace AvtoMigBussines.Requests
{
    public class PushNotificationRequest
    {
        public List<string> Tokens { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
