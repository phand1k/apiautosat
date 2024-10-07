using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using RestSharp;

namespace AvtoMigBussines.Services.Implementations
{
    public class WhatsappSenderService : IWhatsappSenderService
    {
        private readonly UserManager<AspNetUser> userManager;
        public WhatsappSenderService(UserManager<AspNetUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task SendMessage(string? phoneNumber, string? body)
        {
            try
            {
                var url = "https://api.ultramsg.com/instance95613/messages/chat";
                var client = new RestClient(url);

                var request = new RestRequest(url, Method.Post);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", "pt12bzpf9g1votn1");
                request.AddParameter("to", "%" + phoneNumber);
                Random rnd = new Random();
                double code = rnd.Next(1000, 9999);
                request.AddParameter("body", body);

                RestResponse response = await client.ExecuteAsync(request);
                var output = response.Content;

                if (response.IsSuccessful)
                {
                    Console.WriteLine("Whatsapp sms успешно отправлено.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception("Bad request");
                }
                else
                {
                    throw new Exception("Server exception");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error" + ex.Message);
            }
        }
    }
}
