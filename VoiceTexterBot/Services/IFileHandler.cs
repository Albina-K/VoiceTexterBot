using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceTexterBot.Services
{
    public interface IFileHandler
    {
        Task Download(string fileId, CancellationToken ct);//отвечает за первичное скачивание файла, возвращает Task а принимает идентификатор fileId и токен CancellationToken 
        string Process(string param); //метод обрабатывает файл(конвертирует и распознает)
    }
}
