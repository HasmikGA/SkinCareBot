using SkinCareBot.Dto;
using SkinCareBot.Entities;
using SkinCareBot.Services;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SkinCareBot.Scenarios
{
    internal class ShowProductsScenario : IScenario
    {
        private readonly IUserService userService;
        private readonly IProductService productService;
        public ShowProductsScenario(IUserService userService, IProductService productService)
        {
            this.userService = userService;
            this.productService = productService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.ShowProduct;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            var message = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message : update.Message;
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data == "Cancel")
            {
                var replyCancel = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Добавить продукты ➕", "AddProducts")},
                        new [] {InlineKeyboardButton.WithCallbackData("Показать продукты 📄", "ShowProducts")}
                    });
                await bot.SendMessage(message.Chat, "Выбери тему! 👇", replyMarkup: replyCancel, cancellationToken: ct);

                return ScenarioResult.Completed;
            }

            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "SelectSkinType";

                    var replyType = new InlineKeyboardMarkup([
                        [InlineKeyboardButton.WithCallbackData("Нормальная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.Normal }.ToString())],
                        [InlineKeyboardButton.WithCallbackData("Жирная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.Oily }.ToString())],
                        [InlineKeyboardButton.WithCallbackData("Сухая", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.Dry }.ToString())],
                        [InlineKeyboardButton.WithCallbackData("Комбинированная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.CombinationSkin}.ToString())],
                        [InlineKeyboardButton.WithCallbackData("Чувствительная", new SkinTypeCallbackDto { Action = "choose_skinType", SkinType = SkinType.SensitiveSkin }.ToString())],
                        [InlineKeyboardButton.WithCallbackData("Отменить", "Cancel")]
                    ]);


                    await bot.SendMessage(message.Chat, "Выбери тип кожи! 👇", replyMarkup: replyType, cancellationToken: ct);

                    return ScenarioResult.Transition;

                case "SelectSkinType":
                    context.CurrentStep = "SelectProductType";
                    var skinTypeCallbackDto = SkinTypeCallbackDto.FromString(update.CallbackQuery.Data);
                    context.Data["SkinType"] = skinTypeCallbackDto.SkinType;

                    var reply = new InlineKeyboardMarkup([
                         [InlineKeyboardButton.WithCallbackData("Oчищение", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Cleansing }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Тоник", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Toning }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Сыворотка", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Serum }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Крем для глаз", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.EyeCream}.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Увлажнение", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.Moisturizing }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("SPF", new ProductTypeCallbackDto { Action = "choose_product", ProductType = ProductType.SPF }.ToString())],
                         [InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel")]
                    ]);

                    await bot.SendMessage(message.Chat, "Выбери тип продукта! 👇", replyMarkup: reply, cancellationToken: ct);
                    return ScenarioResult.Transition;

                case "SelectProductType":
                    context.CurrentStep = "ShowProducts";
                    var productCallbackDto = ProductTypeCallbackDto.FromString(update.CallbackQuery.Data);
                    context.Data["ProductType"] = productCallbackDto.ProductType;
                    var productType = (ProductType)context.Data["ProductType"];
                    var skinType = (SkinType)context.Data["SkinType"];
                    var products = await this.productService.GetAllProducts(productType, skinType, ct);

                    var replyProducts = new InlineKeyboardMarkup(
                          products.Select(x => new[] { InlineKeyboardButton.WithCallbackData(x.Name, x.Id.ToString()) }).ToList()
                          .Concat(new[] { new[] { InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel") } }).ToList());


                    await bot.SendMessage(message.Chat, "Вот все продукты! 📄", replyMarkup: replyProducts, cancellationToken: ct);

                    return ScenarioResult.Transition;

                case "ShowProducts":
                    context.CurrentStep = "DeleteOrUpdate";
                    context.Data["Id"] = Guid.Parse(update.CallbackQuery.Data);
                    var replyDelete = new InlineKeyboardMarkup([
                         [InlineKeyboardButton.WithCallbackData("Удалить 🗑️","Delete")],
                         [InlineKeyboardButton.WithCallbackData("Редактировать 📝","Update")],
                         [InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel")],
                    ]);

                    await bot.SendMessage(message.Chat, "Что ты хочешь сделать? 🤔", replyMarkup: replyDelete, cancellationToken: ct);
                    return ScenarioResult.Transition;

                case "DeleteOrUpdate":
                    if (update.CallbackQuery.Data == "Delete")
                    {
                        await this.productService.Delete((Guid)context.Data["Id"], ct);
                        await bot.SendMessage(message.Chat, "Продукт успешно удален! ", cancellationToken: ct);
                        var button = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Добавить продукты ➕", "AddProducts")},
                        new [] {InlineKeyboardButton.WithCallbackData("Показать продукты 📄", "ShowProducts")}
                    });
                        await bot.SendMessage(message.Chat, "Выбери тему! 👇", replyMarkup: button, cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }

                    if (update.CallbackQuery.Data == "Update")
                    {
                        await bot.SendMessage(message.Chat, "Введи новое название продукта!", cancellationToken: ct);
                        context.CurrentStep = "NewName";
                        return ScenarioResult.Transition;
                    }

                    return ScenarioResult.Completed;

                case "NewName":
                    var newName = message.Text;
                    var p = await this.productService.Get((Guid)context.Data["Id"], ct);
                    await this.productService.Update(new Product
                    {
                        Id = (Guid)context.Data["Id"],
                        Name = newName,
                        SkinType = (SkinType)context.Data["SkinType"],
                        Type = (ProductType)context.Data["ProductType"]
                    }, ct);

                    await bot.SendMessage(message.Chat, "Название продукта успешно изменено!", cancellationToken: ct);

                    var buttons = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Продукты ","Products")},
                        new [] {InlineKeyboardButton.WithCallbackData("Рекомендации","Recommands")}
                    });
                    await bot.SendMessage(message.Chat, "Выбери тему! 👇", replyMarkup: buttons, cancellationToken: ct);

                    return ScenarioResult.Completed;
            }
            return ScenarioResult.Transition;
        }
    }
}
