using SkinCareBot.Scenarios;
using SkinCareBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SkinCareBot
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly IRecommandService recommendService;
        private readonly IScenarioContextRepository contextRepository;
        private readonly IEnumerable<IScenario> scenarios;
        public UpdateHandler(IUserService userService, IProductService productService, IRecommandService recommendService, IScenarioContextRepository contextRepository, IEnumerable<IScenario> scenarios)
        {
            this.userService = userService;
            this.productService = productService;
            this.recommendService = recommendService;
            this.contextRepository = contextRepository;
            this.scenarios = scenarios;

        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {exception})");
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message != null)
            {
                await this.OnMessage(botClient, update, update.Message, ct);
                return;
            }

            if (update.CallbackQuery != null)
            {
                await this.OnCallbackQuery(botClient, update, update.CallbackQuery, ct);
                return;
            }

            await this.OnUnknown(update);
        }

        private async Task OnUnknown(Update update)
        {
            throw new NotImplementedException();
        }

        private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CallbackQuery callbackQuery, CancellationToken ct)
        {
            var context = await this.contextRepository.GetContext(callbackQuery.Message.Chat.Id, ct);
            if (context != null)
            {
                await this.ProcessScenario(botClient, context, update, ct);
                return;
            }
            var message = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message : update.Message;

            switch (update.CallbackQuery.Data)
            {
                case "Products":
                    var buttons = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Добавить продукты ➕", "AddProducts")},
                        new [] {InlineKeyboardButton.WithCallbackData("Показать продукты 📄", "ShowProducts")}
                    });
                    await botClient.SendMessage(message.Chat, "Выбери действие! 👇", replyMarkup: buttons, cancellationToken: ct);
                    break;

                case "Recommands":
                    var button = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Показать рекомендации", "ShowRecommands")}
                    });
                    await botClient.SendMessage(message.Chat, "Могу показать рекомендации.", replyMarkup: button, cancellationToken: ct);
                    break;

                case "AddProducts":
                    await this.AddProduct(botClient, update, ct);
                    break;

                case "ShowProducts":
                    await this.ShowProducts(botClient, update, ct);
                    break;
                case "ShowRecommands":
                    await this.ShowRecommandations(botClient, update, ct);
                    break;
                case "Khow":
                    var reply = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Подбери продукты 🧺","SuggestProducts")},
                        new [] {InlineKeyboardButton.WithCallbackData("Дай рекомендации 💡", "SuggestRecommands")}
                    });
                    await botClient.SendMessage(message.Chat, "Как могу тебе помочь?", replyMarkup: reply, cancellationToken: ct);
                    break;
                case "Help":
                    await botClient.SendMessage(message.Chat, "Прочитай для правильного выбора.\n Различают 5 основных типа кожи:\n 1. Сухая - тонкая, матовая, поры едва заметны.\n 2. Нормальная - эластичная, упругая кожа, равномерного цвета.\n" +
                        "3. Комбинированная - отличается повышенным салоотделением в Т-зоне.\n 4. Жирная - характеризуется повышенной активностью сальных желез, блестящей поверхностью, расширенными порами.\n" +
                        "5. Чувствительная - характеризуется повышенной реакцией на внешние и внутренние раздражители, такие как косметика, погода или стресс.",
                        cancellationToken: ct);
                    var reply1 = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Подбери продукты 🧺", "SuggestProducts")},
                        new [] {InlineKeyboardButton.WithCallbackData("Дай рекомендации 💡", "SuggestRecommands")}
                    });
                    await botClient.SendMessage(message.Chat, "Как могу тебe помочь?", replyMarkup: reply1, cancellationToken: ct);
                    break;

                case "SuggestProducts":
                    await this.SuggestProducts(botClient, update, ct);
                    break;
                case "SuggestRecommands":
                    await this.SuggestRecommands(botClient, update, ct);
                    break;
            }
        }

        private async Task OnMessage(ITelegramBotClient botClient, Update update, Message message, CancellationToken ct)
        {
            var telegramUserid = message.From.Id;
            var telegramUserName = message.From.Username;
            var user = await this.userService.GetUser(telegramUserid, ct);
            if (user == null)
            {
                user = await this.userService.RegisterUser(telegramUserid, telegramUserName, ct);

            }
            if (user.IsAdmin)
            {
                await this.HandleAdminMessage(botClient, update, ct);
            }
            else
            {
                await this.HandleUserMessage(botClient, update, ct);
            }
        }
        private async Task HandleAdminMessage(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var message = update.Message;
            var context = await this.contextRepository.GetContext(update.Message.Chat.Id, ct);

            if (message.Text == "/start")
            {
                if (context != null)
                {
                    await this.contextRepository.ResetContext(message.From.Id, ct);
                }
                var buttons = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Продукты ","Products")},
                        new [] {InlineKeyboardButton.WithCallbackData("Рекомендации","Recommands")}
                    });
                await botClient.SendMessage(message.Chat, "Привет! Выбери тему 👇!", replyMarkup: buttons, cancellationToken: ct);
                return;
            }

            if (context != null)
            {
                await this.ProcessScenario(botClient, context, update, ct);
                return;
            }
        }
        private async Task HandleUserMessage(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var message = update.Message;

            var context = await this.contextRepository.GetContext(update.Message.Chat.Id, ct);
            
            if (message.Text == "/start")
            {
                if (context != null)
                {
                    await this.contextRepository.ResetContext(message.From.Id, ct);
                }
                var buttons = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Знаю 🤓","Khow")},
                        new [] {InlineKeyboardButton.WithCallbackData("Помоги 💭","Help")}
                    });
                await botClient.SendMessage(message.Chat, "Привет! Давай узнаем, какой у тебя тип кожи. Если не знаешь, могу помочь!", replyMarkup: buttons, cancellationToken: ct);
            }
            if (context != null)
            {
                await this.ProcessScenario(botClient, context, update, ct);
                return;
            }

        }
        public async Task SuggestRecommands(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var context = new ScenarioContext(ScenarioType.SuggestRecommand);
            await this.ProcessScenario(botClient, context, update, ct);
        }
        public async Task SuggestProducts(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var context = new ScenarioContext(ScenarioType.SuggestProduct);
            await this.ProcessScenario(botClient, context, update, ct);
        }
        public async Task AddProduct(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var context = new ScenarioContext(ScenarioType.AddPoduct);
            await this.ProcessScenario(botClient, context, update, ct);

        }
        public async Task ShowProducts(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var context = new ScenarioContext(ScenarioType.ShowProduct);
            await this.ProcessScenario(botClient, context, update, ct);

        }
        public async Task ShowRecommandations(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var context = new ScenarioContext(ScenarioType.ShowRecommand);
            await this.ProcessScenario(botClient, context, update, ct);

        }
        public IScenario GetScenario(ScenarioType scenarioType)
        {
            foreach (var scenario in this.scenarios)
            {
                if (scenario.CanHandle(scenarioType))
                {
                    return scenario;
                }
            }

            throw new Exception();
        }
        public async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            var message = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message : update.Message;

            if (!await this.contextRepository.HasContext(message.Chat.Id, ct))
            {
                await this.contextRepository.SetContext(message.Chat.Id, context, ct);
            }

            var scenario = GetScenario(context.CurrentScenario);

            var result = await scenario.HandleMessageAsync(botClient, context, update, ct);

            if (result == ScenarioResult.Transition)
            {

            }

            if (result == ScenarioResult.Completed)
            {
                this.contextRepository.ResetContext(message.Chat.Id, ct);
            }
        }
    }
}
