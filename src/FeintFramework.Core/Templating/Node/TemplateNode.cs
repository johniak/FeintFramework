using System;
using System.Collections.Generic;
using System.Linq;

namespace FeintFramework.Core.Templating.Node;


public class TemplateNode : BaseNode
{
    public List<BaseNode> Children { get; } = new List<BaseNode>();
    public override string Render(Dictionary<string, object> context)
    {
        return string.Join("", Children.Select(child => child.Render(context)));
    }
}