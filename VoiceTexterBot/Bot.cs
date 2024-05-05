using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace VoiceTexterBot
{
    internal class Bot : BackgroundService
    {
        private ITelegramBotClient _telegramClient;

        //вытаскиваем ITelegramBotClient из контейнера через конструктор класса:
        public Bot(ITelegramBotClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, //здесь выбираем какие обновления хотим получать. в данном случае разрешены все
                cancellationToken: stoppingToken);
            Console.WriteLine("Бот запущен");
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
                Console.WriteLine($"Получено сообщение {update.Message.Text}");
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, $"Вы отправили сообщение { update.Message.Text}", cancellationToken: cancellationToken);
                return;
            }
        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
