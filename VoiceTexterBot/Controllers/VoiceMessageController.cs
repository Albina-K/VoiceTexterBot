using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using VoiceTexterBot.Configuration;
using VoiceTexterBot.Services;
using System.Data;

namespace VoiceTexterBot.Controllers
{
    public class VoiceMessageController
    {
        // private readonly AppSettings _appSettings;
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;
        private readonly IFileHandler _audioFileHandler;

        public VoiceMessageController(AppSettings appSettings, ITelegramBotClient telegramBotClient, IFileHandler audioFileHandler, IStorage memoryStorage)
        {
            //_appSettings = appSettings;
            _telegramClient = telegramBotClient;
            _audioFileHandler = audioFileHandler;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            var fileId = message.Voice?.FileId;
            if (fileId == null) return;
            
            await _audioFileHandler.Download(fileId, ct);

           // await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Загружено голосовое сообщение", cancellationToken: ct);

            string userLanguageCode = _memoryStorage.GetSession(message.Chat.Id).LanguageCode; // Здесь получим язык из сессии пользователя
           var result = _audioFileHandler.Process(userLanguageCode); // Запустим обработку
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, result, cancellationToken: ct);
        }
    }
}
