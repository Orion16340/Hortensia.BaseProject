using System;

namespace Hortensia.Core
{
    public interface ILogger
    {
        void LogCustom(string header, string message, ConsoleColor color, params object[] args);
        void LogDatabase(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogProtocol(string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }

    public class Logger : ILogger
    {
        public void LogCustom(string header, string message, ConsoleColor color, params object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{header} : {message}", args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void LogInformation(string message, params object[] args)
            => LogCustom("Information", message, ConsoleColor.Gray, args);

        public void LogWarning(string message, params object[] args)
            => LogCustom("Warning", message, ConsoleColor.Yellow, args);

        public void LogError(string message, params object[] args)
            => LogCustom("Error", message, ConsoleColor.Red, args);

        public void LogDatabase(string message, params object[] args)
            => LogCustom("Database", message, ConsoleColor.Magenta, args);

        public void LogProtocol(string message, params object[] args)
            => LogCustom("Protocol", message, ConsoleColor.Cyan, args);
    }
}
