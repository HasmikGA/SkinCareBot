
using SkinCareBot.Dto;
using SkinCareBot.Entities;
using SkinCareBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SkinCareBot.Scenarios
{
    internal class SuggestProductsScenario : IScenario
    {
        private readonly IUserService userService;
        private readonly IProductService productService;
        public SuggestProductsScenario(IUserService userService, IProductService productService)
        {
            this.userService = userService;
            this.productService = productService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.SuggestProduct;
        }
        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            var message = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message : update.Message;
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data == "Cancel")
            {
                var replyCancel = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Знаю 🤓", "Khow") },
                        new [] {InlineKeyboardButton.WithCallbackData("Помоги 💡","Help")}
                    });
                await bot.SendMessage(message.Chat, "Знаешь ли ты свой тип кожи, или нужна помощь?", replyMarkup: replyCancel, cancellationToken: ct);

                return ScenarioResult.Completed;
            }
            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "SelectSkinType";

                    var replySkin = new InlineKeyboardMarkup([

                          [InlineKeyboardButton.WithCallbackData("Нормальная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.Normal }.ToString())],
                          [InlineKeyboardButton.WithCallbackData("Жирная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.Oily }.ToString())],
                          [InlineKeyboardButton.WithCallbackData("Сухая", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.Dry }.ToString())],
                          [InlineKeyboardButton.WithCallbackData("Комбинированная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.CombinationSkin}.ToString())],
                          [InlineKeyboardButton.WithCallbackData("Чувствительная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.SensitiveSkin }.ToString())],
                          [InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel")]
                    ]);

                    await bot.SendMessage(message.Chat, "Какая у тебя кожа?", replyMarkup: replySkin, cancellationToken: ct);
                    return ScenarioResult.Transition;

                case "SelectSkinType":
                    context.CurrentStep = "SelectProductType";
                    var skinCallbackDto = SkinTypeCallbackDto.FromString(update.CallbackQuery.Data);
                    context.Data["SkinType"] = skinCallbackDto.SkinType;
                    var replyProduct = new InlineKeyboardMarkup([
                         [InlineKeyboardButton.WithCallbackData("Oчищение", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Cleansing }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Тоник", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Toning }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Сыворотка", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Serum }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Крем для глаз", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.EyeCream}.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Увлажнение", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Moisturizing }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("SPF", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.SPF }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel")]
                     ]);

                    await bot.SendMessage(message.Chat, "Что нужно подобрать?", replyMarkup: replyProduct, cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "SelectProductType":
                    var productCallbackDto = ProductTypeCallbackDto.FromString(update.CallbackQuery.Data);
                    context.Data["ProductType"] = productCallbackDto.ProductType;
                    var skinType = (SkinType)context.Data["SkinType"];
                    var productType = (ProductType)context.Data["ProductType"];
                    var products = await productService.GetAllProducts(productType, skinType, ct);

                    var respons = string.Join('\n', products.Select((x, i) => $"{i + 1}: <code>{x.Name}</code>"));

                    await bot.SendMessage(message.Chat, $"Вот несколько продуктов для тебя! 👇\n{respons}", parseMode: ParseMode.Html, cancellationToken: ct);

                    var reply = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Подбери продукты 🧺","SuggestProducts")},
                        new [] {InlineKeyboardButton.WithCallbackData("Дай рекомендации 💡", "SuggestRecommands")}
                    });
                    await bot.SendMessage(message.Chat, "Как могу тебе помочь?", replyMarkup: reply, cancellationToken: ct);
                    return ScenarioResult.Completed;
            }
            return ScenarioResult.Transition;
        }
    }
}
