using System;

namespace FeintSDK
{
    public class Log
    {

        public static void D(object o)
        {
            if (Settings.DebugMode)
                consoleLog(o, ConsoleColor.Cyan);
        }

        public static void I(object o)
        {
            if (Settings.DebugMode)
                consoleLog(o, ConsoleColor.DarkGreen);
        }

        public static void E(object o)
        {
            if (Settings.DebugMode)
                consoleLog(o, ConsoleColor.DarkMagenta);
        }

        
        private static void consoleLog(object o, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(o);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
