using FeintFramework.Core;

namespace FeintFramework.Forms.Fields;

public class FieldError
{
    public required BaseFormField Field { get; set; }
    public Strings? Errors { get; set; }
}