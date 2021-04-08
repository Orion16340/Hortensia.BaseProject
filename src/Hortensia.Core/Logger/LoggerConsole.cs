using System;
using System.Diagnostics;
using System.Timers;

namespace Hortensia.Core
{
    public static class LoggerConsole
    {
        private static readonly string[] AciiLogo = new string[]
        {
            "|------------------------------------------------------------------------|",
            "|██╗  ██╗ ██████╗ ██████╗ ████████╗███████╗███╗   ██╗███████╗██╗ █████╗  |",
            "|██║  ██║██╔═══██╗██╔══██╗╚══██╔══╝██╔════╝████╗  ██║██╔════╝██║██╔══██╗ |",
            "|███████║██║   ██║██████╔╝   ██║   █████╗  ██╔██╗ ██║███████╗██║███████║ |",
            "|██╔══██║██║   ██║██╔══██╗   ██║   ██╔══╝  ██║╚██╗██║╚════██║██║██╔══██║ |",
            "|██║  ██║╚██████╔╝██║  ██║   ██║   ███████╗██║ ╚████║███████║██║██║  ██║ |",
            "|╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═══╝╚══════╝╚═╝╚═╝  ╚═╝ |",
            "|------------------------------------------------------------------------|",
            "|                             Dofus 2.0.0                                |",
            "|------------------------------------------------------------------------|"
        };

        public static void Initialize(string consoleName)
        {
            Console.Title = $"{consoleName}Server | Uptime : {DateTime.Now - Process.GetCurrentProcess().StartTime:dd\\.hh\\:mm\\:ss}";
            Console.ForegroundColor = ConsoleColor.Blue;

            for (int i = 0; i < AciiLogo.Length; i++)
                Console.WriteLine(AciiLogo[i].PadLeft((int)(Console.BufferWidth + AciiLogo[i].Length) / 2));

            Console.WriteLine(Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.White;

            var timer = new Timer(1000);
            timer.Elapsed += (sender, e) => { Console.Title = $"{consoleName}Server | Uptime : {DateTime.Now - Process.GetCurrentProcess().StartTime:dd\\.hh\\:mm\\:ss}"; };
            timer.Start();
        }
    }
}
