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
    internal class ShowRecommandsScenario : IScenario
    {
        private readonly IUserService userService;
        private readonly IRecommandService recommandService;

        public ShowRecommandsScenario(IUserService userService, IRecommandService recommandService)
        {
            this.userService = userService;
            this.recommandService = recommandService;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.ShowRecommand;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            var message = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message : update.Message;
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data == "Cancel")
            {
                var button = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Покажи рекомендации", "ShowRecommands")}
                    });
                await bot.SendMessage(message.Chat, "Могу показать рекомендации.", replyMarkup: button, cancellationToken: ct);

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
                        [InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel")]
                    ]);

                    await bot.SendMessage(message.Chat, "Выбери тип кожи!", replyMarkup: replyType, cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "SelectSkinType":
                    context.CurrentStep = "ShowRecommands";
                    var skinTypeCallbackDto = SkinTypeCallbackDto.FromString(update.CallbackQuery.Data);
                    context.Data["SkinType"] = skinTypeCallbackDto.SkinType;
                    var skinType = (SkinType)context.Data["SkinType"];
                    var recommands = await this.recommandService.GetAllRecommands(skinType, ct);

                    var respons = string.Join('\n', recommands.Select((x, i) => $"{i + 1}: <code>{x.Text}</code>"));

                    await bot.SendMessage(message.Chat, $"Вот все рекомендации! \n {respons}", parseMode: ParseMode.Html, cancellationToken: ct);

                    var replyRecommand = new InlineKeyboardMarkup(
                         recommands.Select((x,i) => new[] { InlineKeyboardButton.WithCallbackData($"{i+1}", x.Id.ToString()) }).ToList()
                         .Concat(new[] { new[] { InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel") } }).ToList());

                    await bot.SendMessage(message.Chat, "Выбери рекомендацию!",  replyMarkup: replyRecommand, cancellationToken: ct);

                    return ScenarioResult.Transition;

                case "ShowRecommands":
                    context.CurrentStep = "Edit";
                    context.Data["Id"] = Guid.Parse(update.CallbackQuery.Data);
                    var replyEdit = new InlineKeyboardMarkup([
                         [InlineKeyboardButton.WithCallbackData("Редактировать 📝","Edit")],
                         [InlineKeyboardButton.WithCallbackData("Отменить ❌", "Cancel")],
                    ]);
                    await bot.SendMessage(message.Chat, "Ты можешь отредактировать данную рекомендацию!", replyMarkup: replyEdit, cancellationToken: ct);
                    return ScenarioResult.Transition;

                case "Edit":
                    context.CurrentStep = "NewText";
                    await bot.SendMessage(message.Chat, "Введи измененный текст рекомендации!", cancellationToken: ct);
                    return ScenarioResult.Transition;

                case "NewText":
                    var newText = message.Text;
                    var r = await this.recommandService.Get((Guid)context.Data["Id"], ct);
                    await this.recommandService.Update(new Recommandation
                    {
                        Id = (Guid)context.Data["Id"],
                        Text = newText,
                        SkinType = (SkinType)context.Data["SkinType"],
                    }, ct);

                    await bot.SendMessage(message.Chat, "Рекомендация успешно отредактирована! ✅", cancellationToken: ct);

                    var buttons = new InlineKeyboardMarkup(new[]{
                        new [] {InlineKeyboardButton.WithCallbackData("Продукты ","Products")},
                        new [] {InlineKeyboardButton.WithCallbackData("Рекомендации","Recommands")}
                    });
                    await bot.SendMessage(message.Chat, " Выбери тему 👇!", replyMarkup: buttons, cancellationToken: ct);
                    return ScenarioResult.Completed;
            }
            return ScenarioResult.Transition;
        }
    }
}
