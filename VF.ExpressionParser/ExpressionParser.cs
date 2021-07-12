using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace VF.ExpressionParser
{
    public class ExpressionParser
    {
        public static string? GetBodyText(LambdaExpression expression)
        {
            var body = ResolveExpressionValue(expression.Body);
            if (body is null) return null;
            var lambda = Expression.Lambda(Expression.Constant(body), expression.Parameters);
            return Regex.Replace(lambda.ToString(), "\"", "", RegexOptions.None);
        }

        public static object? ResolveExpressionValue(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                // Expression which contains converting from type to type
                var expressionArgumentAsUnary = (UnaryExpression)expression;
                expression = expressionArgumentAsUnary.Operand;
            }

            if (expression is BinaryExpression inner)
            {
                var left = ResolveExpressionValue(inner.Left);
                var right = ResolveExpressionValue(inner.Right);
                var text = $"{left} {inner.NodeType} {right}";
                return text;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    return expression;
                case ExpressionType.Call:
                    {
                        var expressionArgumentAsMethodCall = (MethodCallExpression)expression;
                        var context = expressionArgumentAsMethodCall.GetMethodCallContext();
                        return context.GetMethodCallExpression();
                    }
                // Expression of type c => c.Action({const})
                // Value can be extracted without compiling.
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).Value;
                case ExpressionType.MemberAccess:
                    if (expression is not MemberExpression
                        { Expression: ConstantExpression } memberExpression) return expression;
                    var constantExpression = (ConstantExpression)memberExpression.Expression;
                    var innerMemberName = memberExpression.Member.Name;
                    switch (memberExpression.Member)
                    {
                        case FieldInfo:
                            {
                                var compiledLambdaScopeField =
                                    constantExpression.Value?.GetType().GetField(innerMemberName);
                                return compiledLambdaScopeField?.GetValue(constantExpression.Value);
                            }
                        default:
                            return expression;
                    }
            }

            // Expression needs compiling because it is not of constant type.
            var convertExpression = Expression.Convert(expression, typeof(object));
            return Expression.Lambda<Func<object>>(convertExpression, Array.Empty<ParameterExpression>())
                .Compile()
                .Invoke();
        }
    }
}