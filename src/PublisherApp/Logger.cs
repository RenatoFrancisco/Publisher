using System;

namespace PublisherApp
{
    public static class Logger
    {
        public static void LogInfo(string info) => Console.WriteLine($"info : {info}");
        public static void LogWarn(string warn) 
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"warn : {warn}");
            Console.ForegroundColor = defaultColor;
        }
        public static void LogError(string error) 
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"error : {error}");
            Console.ForegroundColor = defaultColor;
        } 
    }
}