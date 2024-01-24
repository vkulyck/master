using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Yubico;

namespace DevConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await TestVerification();
        }

        static async Task TestVerification()
        {
            string ClientID = ConfigurationManager.AppSettings["YubicoApiClientID"].ToString();
            string ClientKey = ConfigurationManager.AppSettings["YubicoApiSecretKey"].ToString();
            var client = new YubicoClient(ClientID, ClientKey);
            Console.WriteLine("Enter your OTP key: ");
            string otp = Console.ReadLine();
            Console.WriteLine($"Verifying otp '{otp}' with Yubico servers...");
            var response = await client.VerifyAsync(otp);
            Console.WriteLine($"Got response: {response}");
        }
    }
}
