using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VF.ExpressionParser.Helpers.Dto;

namespace VF.ExpressionParser.Helpers.Extension
{
    public static class MethodContextExtensions
    {
        private static List<ArgumentContext> GetMethodArgumentContext(
            IEnumerable<ArgumentMetadata> argumentContexts,
            IReadOnlyList<object?> values) =>
            argumentContexts.Select((t, i) => new ArgumentContext(t.Name, t.ParameterType, values[i])).ToList();

        public static void GetMethodCallExpression(this MethodContext methodContext, StringBuilder st)
        {
            st.Append(methodContext.ReflectedType?.Name);
            st.Append('.');
            st.Append(methodContext.Name);
            st.Append('(');
            foreach (var argumentValue in methodContext.Arguments.Select(value => value.Value))
                if (argumentValue is null)
                    st.Append("null").Append(", ");
                else
                    st.Append(argumentValue).Append(", ");

            st.Remove(st.Length - 2, 2);
            st.Append(')');
        }

        public static string GetMethodCallExpression(this MethodContext methodContext)
        {
            var sb = new StringBuilder();
            GetMethodCallExpression(methodContext, sb);
            return sb.ToString();
        }

        public static MethodContext GetMethodCallContext(this MethodCallExpression expression)
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
                    switch (arg.NodeType)
                    {
                        case ExpressionType.Constant:
                        {
                            var constantExpression = (ConstantExpression) arg;
                            return constantExpression.Value;
                        }
                        case ExpressionType.Parameter:
                        {
                            var parameterExpression = (ParameterExpression) arg;
                            return parameterExpression.Name;
                        }
                        case ExpressionType.MemberAccess:
                        {
                            var member = (MemberExpression) arg;
                            object obg = member;
                             member.GetConstantExpressionMetaData()
                                .MapT0(
                                    data =>
                                    {
                                        var sb = new StringBuilder();
                                        data.WriteConstantValueAndPath(sb);
                                        return sb.ToString();
                                    }
                                ).Switch(
                                    s => obg =s,
                                    _ => { });
                            return obg;
                        }
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
    }
}