using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Feint
{
    class ArgumentsEngine
    {
        public List<ConsoleArgument> Arguments = new List<ConsoleArgument>();

        public void Analize(String[] argv)
        {
            for (int i = 0; i < argv.Length; i++)
            {
                var arg = argv[i];
                foreach (var argument in Arguments)
                {
                    if(Regex.IsMatch(arg,argument.Pattern))
                    {
                        List<String> args = new List<string>();
                        int index = i;
                        for (; i < Math.Min(index + argument.argCount, argv.Length); )
                        {
                            i++;
                            args.Add(argv[i]);
                        }
                        argument.Analize(args);
                        break;
                    }
                }
                
                 
            }
        }
    }

    class ConsoleArgument
    {
        public String Pattern { get; set; }
        public int argCount { get; set; }
        public WorkWithArgumentFunc Analize { get; set; }
        public delegate void WorkWithArgumentFunc(List<String> parameters);
    }
}
