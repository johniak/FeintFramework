using System;
using System.Collections.Generic;
using System.IO;
using FeintFramework.Config;
using FeintFramework.Templating.Node;

namespace FeintFramework.Templating;
public static class TemplateLoader
{
    private static readonly Dictionary<string, TemplateNode> _templateCache = new Dictionary<string, TemplateNode>();

    public static TemplateNode Load(string templateName)
    {
        if (_templateCache.TryGetValue(templateName, out TemplateNode cachedTemplate) && !Configurator.Settings.Debug)
        {
            return cachedTemplate;
        }

        string templateContent = File.ReadAllText(templateName);
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(templateContent);
        Parser parser = new Parser(tokens);
        TemplateNode templateNode = parser.ParseTemplate();
        _templateCache[templateName] = templateNode;

        return templateNode;
    }
}