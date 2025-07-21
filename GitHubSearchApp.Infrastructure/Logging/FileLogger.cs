using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubSearchApp.Infrastructure.Logging
{
    /// <summary>
    /// Classe utilitária para registrar logs em arquivo de texto local.
    /// Cria uma pasta "Logs" automaticamente no diretório da aplicação, se necessário.
    /// Os logs são armazenados em formato de texto com timestamp, nível e mensagem.
    /// </summary>
    public static class FileLogger
    {
        private static readonly object _lock = new();
        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "app-log.txt");

        static FileLogger()
        {
            // Cria a pasta Logs se nao existir
            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
        }

        public static void Log(string message, string level = "INFO")
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
            }
        }

        public static void LogError(Exception ex, string context = "")
        {
            var message = $"{context}\n{ex.Message}\n{ex.StackTrace}";
            Log(message, "ERROR");
        }
    }
}
