using SkinCareBot.DataAccess;
using SkinCareBot.Repositories;
using SkinCareBot.Scenarios;
using SkinCareBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SkinCareBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string clToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN_EX2", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(clToken))
            {
                Console.WriteLine("Bot token not found. Please set the TELEGRAM_BOT_TOKEN environment variable.");
                return;
            }
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=SkinCare";

            var telegramBot = new TelegramBotClient(clToken);
            var dataContextFactory = new DataContextFactory(connectionString);

            IUserRepository userRepository = new UserRepository(dataContextFactory);
            var userService = new UserService(userRepository);
            IProductRepository productRepository = new ProductRepository(dataContextFactory);
            var productService = new ProductService(productRepository);
            IRecommandRepository recommendReposiroty = new RecommandRepository(dataContextFactory);
            var recommendService = new RecommandService(recommendReposiroty);
            IScenarioContextRepository contextRepository = new InMemoryScenarioContextRepository();

            var scenarios = new IScenario[]
            {
                new AddProductScenario(userService, productService),
                new ShowProductsScenario(userService, productService),
                new ShowRecommandsScenario(userService, recommendService),
                new SuggestProductsScenario(userService,productService),
                new SuggestRecommandsScenario(userService,recommendService)
            };

            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            CancellationToken ct = cancellationToken.Token;

            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
                DropPendingUpdates = true
            };

            await SetBotCommands(telegramBot);

            var updateHandler = new UpdateHandler(userService, productService, recommendService, contextRepository, scenarios);

            telegramBot.StartReceiving(updateHandler, receiverOptions, ct);

            Console.ReadKey();
        }
        private static async Task SetBotCommands(ITelegramBotClient botClient)
        {
            var commands = new BotCommand[]
            {
                new BotCommand { Command = "/start", Description = "Запустить бота" },
             
            };

            await botClient.SetMyCommands(commands);
        }
    }
}
