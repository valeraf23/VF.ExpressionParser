using System.Linq.Expressions;
using System.Text;

namespace VF.ExpressionParser
{
    public static class ExpressionExtension
    {
        public static string? ConvertToString(Expression? expression)
        {
            var writerVisitor = new ExpressionWriterVisitor(new StringBuilder());
            return writerVisitor.ConvertToString(expression);
        }
    }
}