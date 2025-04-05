using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using FeintFramework.Core;

namespace FeintFramework.Forms;

public class ValidationException : Exception
{
    public Dictionary<string, Strings>? ErrorsDict { get; protected set; }
    public Strings Errors { get; protected set; }
    public ValidationException(Dictionary<string, Strings> errors)
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