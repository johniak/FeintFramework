using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    class DotInclude:DotLiquid.Tag
    {

        public override void Render(DotLiquid.Context context, System.IO.TextWriter result)
        {
            Template.Parse("").Render(RenderParameters.FromContext(context));
            base.Render(context, result);
            
        }
    }
}
