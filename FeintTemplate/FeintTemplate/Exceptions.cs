using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintTemplate
{
    class UndeclaredVariableException : Exception
    {
        public UndeclaredVariableException(string message)
            : base(message)
        {

        }
    }
    class CantParseException : Exception
    {
        public CantParseException(string message)
            : base(message)
        {

        }
    }
}
