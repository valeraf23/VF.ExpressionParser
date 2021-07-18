using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VF.ExpressionParser
{
    public sealed class MemberGetter<T>
    {
        public Func<T, object>? Getter { get; set; }
        public string? Key { get; set; }
    }
    public static class MemberExpressionHelper
    {
        private static readonly Type TypeOfObject = typeof(object);

        public static IEnumerable<MemberGetter<T>> GetMembersFunctions<T>(LambdaExpression exp)
        {
            var parameterExpression = exp.Parameters.Single();
            var memberExpressionVisitor = new MemberExpressionVisitor();

            return memberExpressionVisitor.GetMemberExpressions(exp).Select(memberExpression =>
            {
                var body = Expression.Convert(memberExpression.Value, TypeOfObject);
                var lambda = Expression.Lambda<Func<T, object>>(body, parameterExpression);

                return new MemberGetter<T>
                {
                    Key = memberExpression.Key,
                    Getter = lambda.Compile()
                };
            });
        }

        private sealed class MemberExpressionVisitor : ExpressionVisitor
        {
            private readonly Dictionary<string, MemberExpression> _memberExpressions = new();
            private bool _canAdd = true;

            public Dictionary<string, MemberExpression> GetMemberExpressions(Expression exp)
            {
                Visit(exp);
                return _memberExpressions;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression is not ConstantExpression)
                {
                    if (_canAdd)
                    {
                        _memberExpressions.Add(node.ToString(), node);
                        _canAdd = false;
                    }

                    Visit(node.Expression);
                }

                _canAdd = true;
                return node;
            }
        }
    }
}