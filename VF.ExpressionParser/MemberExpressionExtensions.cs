using System.Linq.Expressions;
using System.Reflection;

namespace VF.ExpressionParser
{
    public static class MemberExpressionExtensions
    {
        public static object? GetConstantValue(this MemberExpression node)
        {
            if (node.Expression is ParameterExpression) return null;
            switch (node.Member)
            {
                case FieldInfo fieldInfo when node.Expression is ConstantExpression constExpr:
                    return fieldInfo.GetValue(constExpr.Value);
                case FieldInfo fieldInfo when node.Expression is MemberExpression memberExpression:
                    return fieldInfo.GetValue(GetConstantValue(memberExpression));
                case PropertyInfo propertyInfo when node.Expression is MemberExpression memberExpression:
                    var value = GetConstantValue(memberExpression);
                    return value is not null ? propertyInfo.GetValue(value) : value;

                default:
                    return null;
            }
        }
    }
}