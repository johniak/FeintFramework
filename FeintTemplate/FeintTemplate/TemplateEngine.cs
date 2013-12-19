using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FeintTemplate
{
    class TemplateEngine
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        string filename;
        const int BRACKET_OPEN_VALUE = 0;
        const int OPERATOR_OR_ID = 1;
        const int OPERATOR_AND_ID = 2;
        const int OPERATOR_EQ_ID = 3;
        const int OPERATOR_NEQ_ID = 4;
        const int OPERATOR_G_ID = 5;
        const int OPERATOR_L_ID = 6;
        const int OPERATOR_GE_ID = 7;
        const int OPERATOR_LE_ID = 8;

        public TemplateEngine(String filename, object parameters)
        {
            this.filename = filename;
            this.parameters = getVariablesFromObject(parameters);
        }

        public TemplateEngine(String filename, object parameters, Dictionary<string, object> paramsDictionary)
        {
            this.filename = filename;
            this.parameters = new Dictionary<string, object>(paramsDictionary);
            var dict = getVariablesFromObject(parameters);
            foreach (var d in dict)
            {
                this.parameters.Add(d.Key, d.Value);
            }
        }

        public TemplateEngine(String filename, Dictionary<string, object> paramsDictionary)
        {
            this.filename = filename;
            this.parameters = new Dictionary<string, object>(paramsDictionary);
        }

        public TemplateEngine(object parameters)
        {
            this.parameters = getVariablesFromObject(parameters);
        }

        public TemplateEngine(object parameters, Dictionary<string, object> paramsDictionary)
        {
            this.parameters = new Dictionary<string, object>(paramsDictionary);
            var dict = getVariablesFromObject(parameters);
            foreach (var d in dict)
            {
                this.parameters.Add(d.Key, d.Value);
            }
        }

        public TemplateEngine(Dictionary<string, object> paramsDictionary)
        {
            this.parameters = new Dictionary<string, object>(paramsDictionary);
        }

        public string Parse()
        {
            TextReader reader = File.OpenText(filename);
            String text = reader.ReadToEnd();
            reader.Close();
            return Parse(text);
        }

        public string Parse(string code)
        {
            return parseBlock(code);
        }

        string parseBlock(string code)
        {
            TextReader reader = new StringReader(code);
            String output = "";
            while (reader.Peek() >= 0)
            {
                string line = reader.ReadLine();
                if (!isOperation(line))
                {
                    var mms = Regex.Matches(line, "{{[a-zA-Z0-9\\.\"_]+}}");
                    for (int i = 0; i < mms.Count; i++)
                    {
                        var m = mms[i];
                        string varString = m.Groups[0].Value;
                        string trimmed = trimVar(varString);
                        var variable = getVariable(trimmed, parameters);
                        line = line.Replace(varString, variable.ToString());
                    }
                    output += line + "\n";
                    continue;
                }
                string operation = trimOperation(line);
                if (Regex.IsMatch(operation, "^if[ ]+"))
                {
                    string ifBlock;
                    string elseBlock;
                    findIfBlock(reader, out ifBlock, out elseBlock);
                    bool condition = conditionParser(operation.Substring(2).Trim(), parameters);
                    continue;
                }
                output += line + "\n";
            }
            return output;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ifBlock">returns string contains all lines to parse when condition is true</param>
        /// <param name="elseBlock">returns string contains all lines to parse when condition is not true</param>
        void findIfBlock(TextReader reader, out string ifBlock, out string elseBlock)
        {
            elseBlock = "";
            ifBlock = "";
            int level = 1;
            string line = "";
            bool isInElseBlock = false;
            while (reader.Peek() >= 0 && level > 0)
            {
                line = reader.ReadLine();
                if (isOperation(line))
                {
                    var operation = trimOperation(line);
                    if (Regex.IsMatch(operation, "^if[s]+"))
                    {
                        level++;
                    }
                    else if (Regex.IsMatch(operation, "^else$"))
                    {
                        isInElseBlock = true;
                    }
                    else if (Regex.IsMatch(operation, "^endif$"))
                    {
                        level--;
                    }
                }
                else if (isInElseBlock)
                {
                    elseBlock += line + "\n";
                }
                else
                {
                    ifBlock += line + "\n";
                }
            }
        }

        bool conditionParser(string condition, Dictionary<string, object> paramsDictionary)
        {
            string rpn = toRPNLogic(condition);
            return true;
        }
        /// <summary>
        /// Change condition string to reverse polish notation logic condition string 
        /// Not working if string contains one of operators.
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        string toRPNLogic(string condition)
        {
            List<int> stack = new List<int>();
            List<string> variables = new List<string>();
            string rpn = "";
            bool wasOperator = false;
            int level = 0;
            for (int i = 0; i < condition.Length; i++)
            {
                if (condition[i] == '(')
                {
                    stack.Add(BRACKET_OPEN_VALUE);
                }
                else if (condition[i] == ')')
                {
                    while (stack.Count > 0 && stack[stack.Count - 1] != BRACKET_OPEN_VALUE)
                    {
                        rpn += " " + getOperatorById(stack[stack.Count - 1]);
                        stack.RemoveAt(stack.Count - 1);
                    }
                    if (stack.Count > 0)
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                    else
                        throw new CantParseException("bracket mismatch");
                    wasOperator = true;
                }
                else if (condition[i] == '=')
                {
                    if (condition[++i] == '=')
                    {
                        while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_EQ_ID)
                        {
                            rpn += " " + getOperatorById(stack[stack.Count - 1]);
                            stack.RemoveAt(stack.Count - 1);
                        }
                        stack.Add(OPERATOR_EQ_ID);
                    }
                    else
                    {
                        throw new CantParseException("unknown operator");
                    }
                    wasOperator = true;
                }
                else if (condition[i] == '!' && condition[i + 1] == '=')
                {
                    i++;
                    while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_EQ_ID)
                    {
                        rpn += " " + getOperatorById(stack[stack.Count - 1]);
                        stack.RemoveAt(stack.Count - 1);
                    }
                    stack.Add(OPERATOR_NEQ_ID);
                    wasOperator = true;
                }
                else if (condition[i] == '>' && condition[i + 1] != '=')
                {
                    i++;
                    while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_EQ_ID)
                    {
                        rpn += " " + getOperatorById(stack[stack.Count - 1]);
                        stack.RemoveAt(stack.Count - 1);
                    }
                    stack.Add(OPERATOR_GE_ID);
                    wasOperator = true;
                }
                else if (condition[i] == '<' && condition[i + 1] != '=')
                {
                    i++;
                    while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_EQ_ID)
                    {
                        rpn += " " + getOperatorById(stack[stack.Count - 1]);
                        stack.RemoveAt(stack.Count - 1);
                    }
                    stack.Add(OPERATOR_LE_ID);
                    wasOperator = true;
                }
                else if (condition[i] == '>')
                {
                    i++;
                    while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_EQ_ID)
                    {
                        rpn += " " + getOperatorById(stack[stack.Count - 1]);
                        stack.RemoveAt(stack.Count - 1);
                        stack.Add(OPERATOR_G_ID);
                    }
                    wasOperator = true;
                }
                else if (condition[i] == '<')
                {
                    i++;
                    while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_EQ_ID)
                    {
                        rpn += " " + getOperatorById(stack[stack.Count - 1]);
                        stack.RemoveAt(stack.Count - 1);
                        stack.Add(OPERATOR_L_ID);
                    }
                    wasOperator = true;
                }
                else if (condition[i] == '&')
                {
                    if (condition[++i] == '&')
                    {
                        while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_AND_ID)
                        {
                            rpn += " " + getOperatorById(stack[stack.Count - 1]);
                            stack.RemoveAt(stack.Count - 1);
                        }
                        stack.Add(OPERATOR_AND_ID);
                    }
                    else
                    {
                        throw new CantParseException("unknown operator");
                    }
                    wasOperator = true;
                }
                else if (condition[i] == '|')
                {
                    if (condition[++i] == '|')
                    {
                        while (stack.Count > 0 && stack[stack.Count - 1] >= OPERATOR_OR_ID)
                        {
                            rpn += " " + getOperatorById(stack[stack.Count - 1]);
                            stack.RemoveAt(stack.Count - 1);
                        }
                        stack.Add(OPERATOR_AND_ID);
                    }
                    else
                    {
                        throw new CantParseException("unknown operator");
                    }
                    wasOperator = true;
                }
                else
                {
                    if (wasOperator)
                        rpn += " ";
                    rpn += condition[i];
                    wasOperator = false;
                }
            }
            var st = stack.Count - 1;
            //while (s > 0 && stack[s] != '*' && stack[s] != '/' && stack[s] != '%')
            while (st >= 0)
            {
                rpn += " ";
                if (stack[st] != BRACKET_OPEN_VALUE)
                    rpn += getOperatorById(stack[st]);
                stack.RemoveAt(st);
                st--;
            }
            return rpn;
        }
        string getOperatorById(int id)
        {



            switch (id)
            {
                case OPERATOR_OR_ID: return "||";
                case OPERATOR_AND_ID: return "&&";
                case OPERATOR_EQ_ID: return "==";
                case OPERATOR_NEQ_ID: return "!=";
                case OPERATOR_G_ID: return ">";
                case OPERATOR_L_ID: return "<";
                case OPERATOR_GE_ID: return ">=";
                case OPERATOR_LE_ID: return "<=";
            }
            return "";
        }
        bool isOperation(string line)
        {
            return Regex.IsMatch(line, @"{%.+%}");
        }

        string trimVar(string code)
        {
            code = code.Replace("{{", "");
            code = code.Replace("}}", "");
            code = code.Trim();
            return code;
        }

        string trimOperation(string code)
        {
            code = code.Replace("{%", "");
            code = code.Replace("%}", "");
            code = code.Trim();
            return code;
        }
        /// <summary>
        /// Unification method of remeber variables to dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Dictionary<string, object> getVariablesFromObject(object obj)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Type t = obj.GetType();
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var f in fields)
            {
                dict.Add(f.Name, f.GetValue(obj));
            }
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var p in properties)
            {
                dict.Add(p.Name, p.GetValue(obj));
            }
            return dict;
        }

        /// <summary>
        /// Getting variable from
        /// -object (nested to)
        /// -string
        /// -bool value
        /// -numbers
        /// -floats
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameters"></param>
        /// <returns>vraibable which user want</returns>
        object getVariable(string str, Dictionary<string, object> parameters)
        {
            if (Regex.IsMatch(str, @"^([a-zA-Z_][a-zA-Z0-9_-]*)([\.]([a-zA-Z_][a-zA-Z0-9_\-]*))*$"))
            {

                object variable = getVariable(str.Split('.'), parameters);
                return variable;
            }
            else if (Regex.IsMatch(str, "^\".*\"$"))
            {
                return str.Substring(1, str.Length - 2);
            }
            else if (Regex.IsMatch(str, "^[-]?[0-9]+$"))
            {
                return int.Parse(str);
            }
            else if (Regex.IsMatch(str, "^true$"))
                return true;
            else if (Regex.IsMatch(str, "^false$"))
                return false;
            throw new CantParseException("Cant parse variable");
        }

        /// <summary>
        /// Gets Variable from object (nested to)
        /// </summary>
        /// <param name="pathToVariable"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object getVariable(string[] pathToVariable, Dictionary<string, object> parameters)
        {
            object actualVariable;
            try
            {
                actualVariable = parameters[pathToVariable[0]];
            }
            catch (KeyNotFoundException ex)
            {
                throw new UndeclaredVariableException("Undefined Variable");
            }
            for (int i = 1; i < pathToVariable.Length; i++)
            {
                var s = pathToVariable[i];

                var info = actualVariable.GetType().GetMember(s).Single();
                if (!(info is PropertyInfo || info is FieldInfo))
                {
                    throw new UndeclaredVariableException("Undefined Variable");
                }
                if (info is PropertyInfo)
                    actualVariable = ((PropertyInfo)info).GetValue(actualVariable);

                if (info is PropertyInfo)
                    actualVariable = ((PropertyInfo)info).GetValue(actualVariable);
                else if (info is FieldInfo)
                    actualVariable = ((FieldInfo)info).GetValue(actualVariable);
            }
            return actualVariable;
        }
    }
}
