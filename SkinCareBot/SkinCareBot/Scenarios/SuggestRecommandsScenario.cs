using LinqToDB.SqlQuery;
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
    internal class SuggestRecommandsScenario : IScenario
    {
        private readonly IUserService userService;
        private readonly IRecommandService recommandService;

        public SuggestRecommandsScenario(IUserService userService, IRecommandService recommandService)
        {
            this.userService = userService;
            this.recommandService = recommandService;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.SuggestRecommand;
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
                await bot.SendMessage(message.Chat, "Знаешь ли ты свой тип кожи, или нужна помощь??", replyMarkup: replyCancel, cancellationToken: ct);

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
                    var skinCallbackDto = SkinTypeCallbackDto.FromString(update.CallbackQuery.Data);
                    context.Data["SkinType"] = skinCallbackDto.SkinType;
                    var skinType = (SkinType)context.Data["SkinType"];
                    var recommands = await this.recommandService.GetAllRecommands(skinType, ct);

                    var respons = string.Join('\n', recommands.Select((x, i) => $"{i + 1}: {x.Text}"));
                    await bot.SendMessage(message.Chat, $"Вот краткие рекомендации для тебя! 🤏\n{respons}", cancellationToken: ct);

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
