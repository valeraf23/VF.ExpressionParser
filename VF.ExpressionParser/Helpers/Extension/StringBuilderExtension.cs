using System.Text;

namespace VF.ExpressionParser.Helpers.Extension
{
    public static class StringBuilderExtension
    {
        public static StringBuilder RemoveLast(this StringBuilder sb)
        {
            if (sb.Length > 1) sb.Length -= 1;

            return sb;
        }
    }
}