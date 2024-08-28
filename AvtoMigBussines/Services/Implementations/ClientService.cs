using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace AvtoMigBussines.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ITelegramBotClient _botClient;

        public ClientService(IClientRepository clientRepository, string botToken)
        {
            _clientRepository = clientRepository;
            _botClient = new TelegramBotClient(botToken);
        }

        public async Task SubscribeToUpdatesAsync(string carNumber, long telegramUserId)
        {
            var existingClient = await _clientRepository.GetClientByCarNumberAsync(carNumber);
            if (existingClient == null)
            {
                var newClient = new Client
                {
                    CarNumber = carNumber,
                    TelegramUserId = telegramUserId,
                    DateOfRegister = DateTime.UtcNow
                };

                await _clientRepository.AddClientAsync(newClient);
            }
        }

        public async Task NotifyUsersAsync(string carNumber)
        {
            Console.WriteLine($"Начало уведомления для машины с номером: {carNumber}");
            var clients = await _clientRepository.GetClientsByCarNumberAsync(carNumber);
            foreach (var client in clients)
            {
                if (client.TelegramUserId.HasValue)
                {
                    try
                    {
                        Console.WriteLine($"Отправка уведомления пользователю с TelegramUserId: {client.TelegramUserId}");
                        await _botClient.SendTextMessageAsync(
                            chatId: new ChatId(client.TelegramUserId.Value),
                            text: $"Ваш автомобиль с номером {carNumber} готов!"
                        );
                        Console.WriteLine($"Уведомление отправлено пользователю с TelegramUserId: {client.TelegramUserId}");
                    }
                    catch (Exception ex)
                    {
                        // Обработка ошибок
                        Console.WriteLine($"Ошибка при отправке сообщения пользователю {client.TelegramUserId}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Пользователь с TelegramUserId: {client.TelegramUserId} не найден");
                }
            }
            Console.WriteLine($"Завершение уведомления для машины с номером: {carNumber}");
        }

    }

}
