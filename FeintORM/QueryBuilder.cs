using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public class QueryBuilder<T>
    {

        String orderBy;
        bool desc;

        List<Object> argsStack = new List<object>();

        #region operators precedece
        private Dictionary<ExpressionType, int> operatorsPrecedence = new Dictionary<ExpressionType, int>()
        {
            {ExpressionType.Increment,0},
            {ExpressionType.Decrement,0},

            {ExpressionType.Not,1},

            {ExpressionType.Multiply,2},
            {ExpressionType.Divide,2},
            {ExpressionType.Modulo,2},

            {ExpressionType.Add,3},
            {ExpressionType.Subtract,3},
            
            {ExpressionType.GreaterThan,5},
            {ExpressionType.GreaterThanOrEqual,5},
            {ExpressionType.LessThan,5},
            {ExpressionType.LessThanOrEqual,5},

            
            {ExpressionType.Equal,6},
            {ExpressionType.NotEqual,6},

            {ExpressionType.AndAlso,10},
            {ExpressionType.OrElse,11},


        };
        #endregion


        public WhereBuilder<T> Where()
        {
            return new WhereBuilder<T>(this);
        }

        public WhereBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            var builder = Where();
            analizeExpresion(builder, predicate.Body);

            return builder;
        }


        private void analizeExpresion<T>(WhereBuilder<T> builder, Expression expression)
        {
            if (!typeof(BinaryExpression).IsAssignableFrom(expression.GetType()))
            {
                if (expression.NodeType == ExpressionType.MemberAccess)
                {
                    argsStack.Add(expressionMemberAccess(expression));
                }
                else if (expression.NodeType == ExpressionType.Constant)
                {
                    ConstantExpression constExpression = (ConstantExpression)expression;
                    argsStack.Add(constExpression.Value);
                }
                else if (expression.NodeType == ExpressionType.Not)
                {
                    builder.Not();
                    builder.Lp();
                    UnaryExpression unaryExpression = (UnaryExpression)expression;
                    analizeExpresion(builder, unaryExpression.Operand);
                    builder.Rp();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                BinaryExpression binaryExpression = (BinaryExpression)expression;
                if (getOperatorPrecedece(binaryExpression.Left.NodeType) > getOperatorPrecedece(binaryExpression.Right.NodeType))
                {
                    builder.Lp();
                    Console.WriteLine("(");
                }
                analizeExpresion(builder, binaryExpression.Left);
                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.AndAlso:
                        Console.WriteLine(binaryExpression.NodeType);
                        builder.And();
                        break;
                    case ExpressionType.OrElse:
                        Console.WriteLine(binaryExpression.NodeType);
                        builder.Or();
                        break;
                }
                analizeExpresion(builder, binaryExpression.Right);
                if (argsStack.Count > 1)
                    Console.WriteLine(argsStack[0] + " " + binaryExpression.NodeType + " " + argsStack[1]);
                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        break;
                    case ExpressionType.GreaterThan:
                        var args = getOrderedParameters();
                        builder.Gt(args.Item1, args.Item2);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        args = getOrderedParameters();
                        builder.Ge(args.Item1, args.Item2);
                        break;
                    case ExpressionType.LessThan:
                        args = getOrderedParameters();
                        builder.Lt(args.Item1, args.Item2);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        args = getOrderedParameters();
                        builder.Le(args.Item1, args.Item2);
                        break;
                    case ExpressionType.Equal:
                        args = getOrderedParameters();
                        builder.Eq(args.Item1, args.Item2);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (getOperatorPrecedece(binaryExpression.Left.NodeType) > getOperatorPrecedece(binaryExpression.Right.NodeType))
                {
                    builder.Rp();
                    Console.WriteLine(")");
                }
            }


        }

        private object expressionMemberAccess(Expression expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression;
            if (memberExpression.Expression.NodeType == ExpressionType.Constant)
            {
                var valueFunc = Expression.Lambda<Func<Object>>(memberExpression.Expression).Compile()();
                var value = ((Type)valueFunc.GetType()).GetField(memberExpression.Member.Name).GetValue(valueFunc);
                return value;
            }
            else
            {
                Expression tempExpression = memberExpression;
                ExpressionType endType = ExpressionType.Parameter;
                StringBuilder pathBuilder = new StringBuilder();
                while (tempExpression.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression tempMemberExpression = (MemberExpression)tempExpression;
                    pathBuilder.Insert(0, tempMemberExpression.Member.Name);
                    pathBuilder.Insert(tempMemberExpression.Member.Name.Length, ".");
                    tempExpression = tempMemberExpression.Expression;
                }
                pathBuilder.Remove(pathBuilder.Length - 1, 1);
                endType = tempExpression.NodeType;
                if (endType == ExpressionType.Parameter)
                {
                    return pathBuilder.ToString();
                }
                else if (endType == ExpressionType.Constant)
                {
                    var valueFunc = Expression.Lambda<Func<Object>>(memberExpression.Expression).Compile()();
                    Object value;
                    System.Reflection.MemberTypes memberType = ((Type)valueFunc.GetType()).GetMember(memberExpression.Member.Name)[0].MemberType;
                    if (memberType == System.Reflection.MemberTypes.Field)
                    {
                        value = ((Type)valueFunc.GetType()).GetField(memberExpression.Member.Name).GetValue(valueFunc);
                    }
                    else if (memberType == System.Reflection.MemberTypes.Property)
                    {
                        value = ((Type)valueFunc.GetType()).GetProperty(memberExpression.Member.Name).GetValue(valueFunc);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return value;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }


        private Tuple<string, object> getOrderedParameters()
        {
            Tuple<string, object> result;
            if (argsStack[0] is string)
            {
                result = Tuple.Create(argsStack[0].ToString(), argsStack[1]);
            }
            else if (argsStack[1] is string)
            {
                result = Tuple.Create(argsStack[1].ToString(), argsStack[0]);
            }
            else
            {
                result = Tuple.Create(argsStack[0].ToString(), argsStack[1]);
            }
            argsStack.RemoveAt(0);
            argsStack.RemoveAt(0);
            return result;
        }

        private int getOperatorPrecedece(ExpressionType operatorType)
        {
            if (operatorsPrecedence.ContainsKey(operatorType))
            {
                return operatorsPrecedence[operatorType];
            }
            return operatorsPrecedence.Count;
        }


    }



}
