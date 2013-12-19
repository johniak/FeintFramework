using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
