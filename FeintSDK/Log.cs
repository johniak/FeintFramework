using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    public class Log
    {
        public static void D(object o)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(o);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void I(object o)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(o);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void E(object o)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(o);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
