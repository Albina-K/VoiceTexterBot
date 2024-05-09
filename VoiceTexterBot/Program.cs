using System;
using System.Text;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VoiceTexterBot.Controllers;
using VoiceTexterBot.Services;
using VoiceTexterBot.Configuration;

namespace VoiceTexterBot
{
    public class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            //Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) //задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            Console.WriteLine("Сервис запущен");
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            AppSettings appSettings = BuildAppSettings();
            services.AddSingleton(BuildAppSettings());

            services.AddSingleton<IStorage, MemoryStorage>();

            services.AddSingleton<IFileHandler, AudioFileHandler>();

            // Подключаем контроллеры сообщений и кнопок
            services.AddTransient<DefaultMessageController> ();
            services.AddTransient<VoiceMessageController> ();
            services.AddTransient<TextMessageController> ();
            services.AddTransient<InlineKeyboardController> ();
            
            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));//Добавляет в контейнер с зависимостями сервис TelegramBotClient (через интерфейс ITelegramBotClient).
            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
        }

        static AppSettings BuildAppSettings()
        {
            return new AppSettings()
            {
                DownloadsFolder = "C:\\User\\Luft\\SkillFactoryNew",
                BotToken = "6926607102:AAF9nVB9DpV3090e3UsYf_cHMr9iVs2IP4s",
                AudioFileName = "audio",
                InputAudioFormat = "ogg",
            };
        }
    }   
}
