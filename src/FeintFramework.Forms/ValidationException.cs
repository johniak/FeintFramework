using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace FeintFramework.Forms;

public class ValidationException : Exception
{
    public Dictionary<string, StringValues>? ErrorsDict { get; protected set; }
    public StringValues Errors { get; protected set; }
    public ValidationException(Dictionary<string, StringValues> errors)
    {
        ErrorsDict = errors;
    }

    public ValidationException(IEnumerable<string> errors)
    {
        Errors = errors.ToArray();
    }

    public ValidationException(string error)
    {
        Errors = error;
    }

}