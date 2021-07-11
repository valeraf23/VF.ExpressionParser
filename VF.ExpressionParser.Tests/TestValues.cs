namespace VF.ExpressionParser.Tests
{
    public class TestValues
    {
        public int FooInt { get; set; }

        public bool IsGreaterThan(int x, int y) => x > y;

        public static bool IsGreaterThanStatic(int x, int y) => x > y;

        public bool IsStringNullOrEmpty(string val) => string.IsNullOrEmpty(val);
    }
}