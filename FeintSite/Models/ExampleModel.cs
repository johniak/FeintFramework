using System;
using FeintSDK;

namespace FeintSite.Models
{
    public class ExampleModel : BaseModel
    {
        public override int? Id { get; set; }
        public String ExamplePropertyString { get; set; }
        public int ExamplePropertyInteger { get; set; }
    }
}