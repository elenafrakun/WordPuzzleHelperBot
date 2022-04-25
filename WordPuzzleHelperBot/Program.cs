using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using WordPuzzleHelperBot.Classes;

namespace WordPuzzleHelperBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var token = "{YOUR_ACCESS_TOKEN_HERE}";
            var botClient = new WordPuzzleHelperBotClient(token);
            using var cancellationTokenSource = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            botClient.StartReceiving(receiverOptions, cancellationTokenSource);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cancellationTokenSource.Cancel();
        }
    }
}
