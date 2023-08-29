using Refit;
using System.Security.Cryptography.X509Certificates;

namespace Paypal_Sandbox
{
    public class Program
    {
        public const string BaseAddress = "https://api.sandbox.paypal.com/";

        static async Task Main(string[] args)
        {
            var authHandler = new AuthHeaderHandler { InnerHandler = new HttpClientHandler() };

            var payPalClient = RestService.For<IPayPalClient>(new HttpClient(authHandler)
            {
                BaseAddress = new Uri(BaseAddress)
            });

            var order = new Order
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new Purchase_Units
                    {
                        amount = new Amount
                        {
                            currency_code = "EUR", value = "100.00"
                        }
                    }
                }
            };
            var createOrderResponse = await payPalClient.CreateOrder(order);
            var payment = await payPalClient.GetOrder(createOrderResponse.id);
            var pay = payment.purchase_units.First();
            Console.WriteLine($"{pay.payee.email_address} - " +
                              $"{pay.amount.value}" +
                              $"{pay.amount.currency_code}");
        }
    }
}