using Microsoft.AspNetCore.SignalR;

namespace AvtoMigBussines.SignalR
{
    public class OrderHub : Hub
    {
        // Метод для уведомления о том, что заказ-наряд был обновлен
        public async Task OrderUpdated(string message)
        {
            await Clients.All.SendAsync("OrderUpdated", message);
        }
    }
}
