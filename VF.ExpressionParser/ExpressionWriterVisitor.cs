using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VF.ExpressionParser.Helpers;
using VF.ExpressionParser.Helpers.Extension;

namespace VF.ExpressionParser
{
    public sealed class ExpressionWriterVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _writer;

        public ExpressionWriterVisitor(StringBuilder writer) => _writer = writer;

        public string ConvertToString(Expression? exp)
        {
            Visit(exp);
            return _writer.ToString();
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _writer.Append(node.Name);
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                _writer.Append('(');
                _writer.Append(node.Type.FullName);
                _writer.Append(')');
            }
            else
            {
                _writer.Append(node.NodeType);
            }

            _writer.Append('(');
            Visit(node.Operand);
            _writer.Append(')');

            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _writer.Append('(');
            _writer.Append(string.Join(", ", node.Parameters.Select(param => param.Name)));
            _writer.Append(')');
            _writer.Append(" => ");

            Visit(node.Body);

            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            Visit(node.Test);
            _writer.Append('?');
            Visit(node.IfTrue);
            _writer.Append(':');
            Visit(node.IfFalse);
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            _writer.Append(' ').Append(GetOperator(node.NodeType)).Append(' ');
            Visit(node.Right);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // Closures are represented as a constant object with fields representing each closed over value.
            // This gets and prints the value of that closure.

            node.GetConstantExpressionMetaData()
                .Switch(
                    data => data.WriteConstantValueAndPath(_writer),
                    _ =>
                    {
                        Visit(node.Expression);
                        _writer.Append('.');
                        _writer.Append(node.Member.Name);
                    });
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            WriterHelper.WriteConstantValue(node.Value, _writer);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var context = node.GetMethodCallContext();
            context.GetMethodCallExpression(_writer);
            return node;
        }

        private static string GetOperator(ExpressionType type) => type switch
        {
            ExpressionType.Equal => "==",
            ExpressionType.Not => "!",
            ExpressionType.NotEqual => "!==",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.Or => "|",
            ExpressionType.OrElse => "||",
            ExpressionType.And => "&",
            ExpressionType.AndAlso => "&&",
            ExpressionType.Add => "+",
            ExpressionType.AddAssign => "+=",
            ExpressionType.Subtract => "-",
            ExpressionType.SubtractAssign => "-=",
            ExpressionType.Divide => "/",
            ExpressionType.Multiply => "*",
            _ => "???"
        };
    }
}