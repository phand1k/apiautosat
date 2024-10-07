namespace AvtoMigBussines.Services.Interfaces
{
    public interface IWhatsappSenderService
    {
        Task SendMessage(string? phoneNumber, string? body);
    }
}
