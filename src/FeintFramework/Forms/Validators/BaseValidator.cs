
using Microsoft.AspNetCore.Mvc;

namespace FeintFramework.Forms.Validators;


public abstract class BaseValidator{

    public abstract void Validate(object obj);
}