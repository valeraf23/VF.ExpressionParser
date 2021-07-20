using System.Text;

namespace VF.ExpressionParser.Helpers.Dto
{
    public class ConstantExpressionMetaData
    {
        public ConstantExpressionMetaData(StringBuilder path, object? value)
        {
            Path = path;
            Value = value ?? "null";
        }

        public StringBuilder Path { get; }
        public object Value { get; }

        public void WriteConstantValueAndPath(StringBuilder writer)
        {
            writer.Append(Path); 
            // if (!Value.GetType().IsPrimitiveOrString()) return;
            writer.Append('(');
            WriterHelper.WriteConstantValue(Value, writer);
            writer.Append(')');

        }
    }
}