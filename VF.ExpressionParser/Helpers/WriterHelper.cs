using System.Text;

namespace VF.ExpressionParser.Helpers
{
    public static class WriterHelper
    {
        public static void WriteConstantValue(object? obj, StringBuilder writer)
        {
            switch (obj)
            {
                case string str:
                    writer.Append('"');
                    writer.Append(str);
                    writer.Append('"');
                    break;
                default:
                    writer.Append(obj ?? "null");
                    break;
            }
        }
    }
}