using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using OneOf;
using VF.ExpressionParser.Helpers.Dto;

namespace VF.ExpressionParser.Helpers.Extension
{
    public static class MemberExpressionExtensions
    {
        public static OneOf<object?, ParameterExpression> GetConstantValue(this MemberExpression node,
            StringBuilder writer)
        {
            void Func(MemberExpression n)
            {
                writer.Append(n.Member.Name);
                writer.Append('.');
            }

            return GetConstantValue(node, Func);
        }

        public static OneOf<object?, ParameterExpression> GetConstantValue(this MemberExpression node,
            Action<MemberExpression> writerFunc)
        {
            object? retrievedValue = null;

            switch (node.Expression)
            {
                case ParameterExpression paramExpr:
                    return paramExpr;
                case ConstantExpression constExpr when node.Member is FieldInfo fieldInfo:
                    retrievedValue = fieldInfo.GetValue(constExpr.Value);
                    break;
                case MemberExpression memberExpression:
                    retrievedValue = GetMemberExpressionValue(memberExpression, node.Member, writerFunc);
                    break;
            }

            if (retrievedValue is ParameterExpression p)
                return p;

            writerFunc(node);

            return retrievedValue;
        }

        private static object? GetMemberExpressionValue(MemberExpression memberExpression, MemberInfo memberInfo,
            Action<MemberExpression> writerFunc)
        {
            return GetConstantValue(memberExpression, writerFunc)
                .MapT0(o =>
                {
                    if (o is null) throw new NullReferenceException(memberExpression.ToString());

                    return o;
                })
                .Match(value => memberInfo switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(value),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(value),
                    _ => throw new Exception("Something goes wrong")
                }, paramExpr => paramExpr);
        }

        public static OneOf<ConstantExpressionMetaData, ParameterExpression> GetConstantExpressionMetaData(
            this MemberExpression node)
        {
            var sb = new StringBuilder();
            var value = node.GetConstantValue(sb);
            return value.MapT0(obj => new ConstantExpressionMetaData(sb.RemoveLast(), obj));
        }
    }
}