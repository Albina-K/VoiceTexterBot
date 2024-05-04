using System;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace VoiceTexterBot
{
    class Bot
    {
        private ITelegramBotClient _telegramClient;

        public Bot(ITelegramBotClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botclient, Update update, CancellationToken cancellationToken)
        {
            //обрабатываем нажатие на кнопки из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы нажали кнопку", cancellationToken: cancellationToken);
                return;
            }

            //обрабатываем входящие сообщения из elegram Bot API: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы отправили сообщение", cancellationToken: cancellationToken);
                return;
            }

            Task HandleErrorAsyns(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                //задаем сообщение об ошибке в зависимости от тогоб какая именно ошибка произошла
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                //выводим в консоль информацию об ошибке
                Console.WriteLine(errorMessage);

                //задержка перед повторным подключением
                Console.WriteLine("Ожидаем 10 секунд перед повторным подключением");
                Thread.Sleep(10000);

                return Task.CompletedTask;
            }

        }
    }
}
