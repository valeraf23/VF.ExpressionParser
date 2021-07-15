namespace VF.ExpressionParser.Tests
{
    public class TestValues
    {
        public int FooInt { get; set; }

        public bool IsGreaterThan(int x, int y) => x > y;

        public static bool IsGreaterThanStatic(int x, int y) => x > y;

        public static int SumStatic(int x, int y) => x + y;

        public int Sum(int x, int y) => x + y;

        public bool IsStringNullOrEmpty(string val) => string.IsNullOrEmpty(val);
    }

    public class SomeClass
    {
        public int SomeNumber;
        public OtherClass Child { get; init; }
    }

    public class OtherClass
    {
        public int SomeNumber { get; init; }
    }
}