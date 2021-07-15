using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace VF.ExpressionParser.Tests
{
    public class ExpressionParserTests
    {
        [Fact]
        public void Should_Retrieve_Property()
        {
            var s = new SomeClass
            {
                Child = new OtherClass()
                {
                    SomeNumber = 1
                },
                SomeNumber = 2
            };
            var foo = 78;
            Expression<Func<SomeClass, bool>> exp = a => s.Child.SomeNumber == 1 && a.SomeNumber == 3 && s.SomeNumber == 3 && foo > 0 || new TestValues().Sum(s.SomeNumber, 5) > 5;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo($"(a) => {s.Child.SomeNumber} == 1 && a.SomeNumber == 3 && {s.SomeNumber} == 3 && {foo} > 0 || TestValues.Sum({s.SomeNumber}, 5) > 5");
        }

        [Fact]
        public void Should_Display_Parameters()
        {
            var foo = 78;
            Expression<Func<SomeClass, bool>> exp = a => a.Child.SomeNumber == 1 && a.SomeNumber == 3 && foo > 0 || new TestValues().Sum(foo, 5) > 5;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("(a) => a.Child.SomeNumber == 1 && a.SomeNumber == 3 && 78 > 0 || TestValues.Sum(78, 5) > 5");
        }

        [Fact]
        public void Should_Retrieve_Parameter_Name_In_Method_Call_Arguments()
        {
            Expression<Func<string, bool>> exp = x => string.IsNullOrEmpty(x);
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("(x) => String.IsNullOrEmpty(x)");
        }

        [Fact]
        public void Should_Retrieve_Parameter_Name_In_Method_Call_Arguments_With_Not()
        {
            Expression<Func<string, bool>> exp = x => !string.IsNullOrEmpty(x);
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("(x) => Not(IsNullOrEmpty(x))");
        }

        [Fact]
        public void Should_Retrieve_Local_Variable()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => foo > 2;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => 78 > 2");
        }

        [Fact]
        public void Should_Compile_Const_Variable()
        {
            const int foo = 78;
            Expression<Func<bool>> exp = () => foo > 2;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => True");
        }

        [Fact]
        public void Should_Retrieve_Binary_Expression()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => foo + 5 > 2 && foo > 3;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => 78 + 5 > 2 && 78 > 3");
        }

        [Fact]
        public void Should_Retrieve_Binary_More_Complex_Expression()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => foo > 2 && foo > 3 || foo < 100;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => 78 > 2 && 78 > 3 || 78 < 100");
        }

        [Fact]
        public void Should_Retrieve_Method()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => new TestValues().IsGreaterThan(foo, 5) || foo < 100;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThan(78, 5) || 78 < 100");
        }

        [Fact]
        public void Should_Retrieve_Method_And_Display_Null_Arguments()
        {
            Expression<Func<bool>> exp = () => new TestValues().IsStringNullOrEmpty(null);
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsStringNullOrEmpty(null)");
        }

        [Fact]
        public void Should_Retrieve_Static_Method()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => TestValues.IsGreaterThanStatic(foo, 5) || foo < 100;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThanStatic(78, 5) || 78 < 100");
        }

        [Fact]
        public void Should_Retrieve_Static_Method_With_Const_Arguments()
        {
            Expression<Func<bool>> exp = () => TestValues.IsGreaterThanStatic(78, 5);
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThanStatic(78, 5)");
        }

        [Fact]
        public void Should_Retrieve_Static_Method_With_Instance_Arguments()
        {
            var foo = new TestValues
            {
                FooInt = 50
            };
            Expression<Func<bool>> exp = () => TestValues.IsGreaterThanStatic(foo.FooInt, 5);
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThanStatic(50, 5)");
        }

        [Fact]
        public void Should_Retrieve_Expression_Arguments()
        {
            Expression<Func<int, int, int>> sum = (x, y) => x + y;
            var res = ExpressionExtension.ConvertToString(sum);
            res.Should().BeEquivalentTo("(x, y) => x + y");
        }

        [Fact]
        public void Should_Retrieve_Expression_Arguments_And_Static_Variable()
        {
            Expression<Func<int, int, int>> sum = (x, y) => x + y + 50;
            var res = ExpressionExtension.ConvertToString(sum);
            res.Should().BeEquivalentTo("(x, y) => x + y + 50");
        }

        [Fact]
        public void Should_Retrieve_Expression_Arguments_And_Static_Variable_Subtract()
        {
            var foo = 50;
            Expression<Func<int, int, int>> sum = (x, y) => x + y - foo;
            var res = ExpressionExtension.ConvertToString(sum);
            res.Should().BeEquivalentTo("(x, y) => x + y - 50");
        }

        [Fact]
        public void Should_Retrieve_Action_Expression()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => TestValues.SumStatic(foo, 5) > 0;
            var res = ExpressionExtension.ConvertToString(exp);
            res.Should().BeEquivalentTo("() => TestValues.SumStatic(78, 5) > 0");
        }
    }
}