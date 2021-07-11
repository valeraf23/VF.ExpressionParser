using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace VF.ExpressionParser
{
    public class ExpressionParser
    {

        public static string? GetBodyText<T>(Expression<Func<T>> expression) => ResolveExpressionValue(expression.Body)?.ToString();

        public static string? GetBodyText<T, T1>(Expression<Func<T, T1>> expression) => ResolveExpressionValue(expression.Body)?.ToString();

        public static string? GetBodyText<T, T1, T2>(Expression<Func<T, T1, T2>> expression) => ResolveExpressionValue(expression.Body)?.ToString();
       
        public static string? GetBodyText<T>(Expression<Action<T>> expression) => ResolveExpressionValue(expression.Body)?.ToString();

        public static string? GetBodyText(Expression<Action> expression) => ResolveExpressionValue(expression.Body)?.ToString();

        public static string? GetBodyText<T, T1>(Expression<Action<T, T1>> expression) => ResolveExpressionValue(expression.Body)?.ToString();

        private static List<ArgumentContext> GetMethodArgumentContext(
            IEnumerable<ArgumentMetadata> argumentContexts,
            IReadOnlyList<object?> values)
            => argumentContexts.Select((t, i)
                => new ArgumentContext(t.Name, t.ParameterType, values[i])).ToList();

        private static string GetMethodCallExpressionBody(MethodContext methodContext)
        {
            var st = new StringBuilder();
            st.Append(methodContext.ReflectedType?.Name);
            st.Append('.');
            st.Append(methodContext.Name);
            st.Append('(');
            foreach (var argumentValue in methodContext.Arguments.Select(value => value.Value))
            {
                if (argumentValue is null)
                    st.Append("null").Append(", ");
                else
                    st.Append(argumentValue).Append(", ");
            }


            st.Remove(st.Length - 2, 2);
            st.Append(')');
            return st.ToString();
        }

        private static MethodContext GetMethodCallContext(MethodCallExpression expression)
        {
            var reflectedType = expression.Method.ReflectedType;
            var methodName = expression.Method.Name;
            var argumentsContext = expression.Method
                .GetParameters()
                .Select(p => new ArgumentMetadata(p.Name, p.ParameterType))
                .ToArray();

            var values = expression.Arguments
                .Select(arg =>
                {
                    if (arg.NodeType == ExpressionType.Constant)
                    {
                        var constantExpression = (ConstantExpression)arg;
                        return constantExpression.Value;
                    }

                    // () => (object)arg
                    var convertExpression = Expression.Convert(arg, typeof(object));
                    var funcExpression =
                        Expression.Lambda<Func<object>>(convertExpression, Array.Empty<ParameterExpression>());
                    return funcExpression.Compile().Invoke();
                })
                .ToArray();

            var methodContext = new MethodContext(
                methodName,
                reflectedType,
                GetMethodArgumentContext(argumentsContext, values));

            return methodContext;
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
                        var context = GetMethodCallContext(expressionArgumentAsMethodCall);
                        return GetMethodCallExpressionBody(context);
                    }
                // Expression of type c => c.Action({const})
                // Value can be extracted without compiling.
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).Value;
                case ExpressionType.MemberAccess when ((MemberExpression)expression).Member is FieldInfo:
                    {
                        // Expression of type c => c.Action(id)
                        // Value can be extracted without compiling.
                        if (expression is MemberExpression { Expression: ConstantExpression constantExpression } memberAccessExpr)
                        {
                            var innerMemberName = memberAccessExpr.Member.Name;
                            var compiledLambdaScopeField = constantExpression.Value?.GetType().GetField(innerMemberName);
                            return compiledLambdaScopeField?.GetValue(constantExpression.Value);
                        }

                        break;
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