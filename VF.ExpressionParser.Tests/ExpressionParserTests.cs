using System;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace VF.ExpressionParser.Tests
{
    public class ExpressionParserTests
    {
        [Fact]
        public void Should_Retrieve_Local_Variable()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => foo > 2;
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => 78 GreaterThan 2");
        }

        [Fact]
        public void Should_Compile_Const_Variable()
        {
            const int foo = 78;
            Expression<Func<bool>> exp = () => foo > 2;
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => True");
        }

        [Fact]
        public void Should_Retrieve_Binary_Expression()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => foo + 5 > 2 && foo > 3;
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => 78 Add 5 GreaterThan 2 AndAlso 78 GreaterThan 3");
        }

        [Fact]
        public void Should_Retrieve_Binary_More_Complex_Expression()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => foo > 2 && foo > 3 || foo < 100;
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => 78 GreaterThan 2 AndAlso 78 GreaterThan 3 OrElse 78 LessThan 100");
        }

        [Fact]
        public void Should_Retrieve_Method()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => new TestValues().IsGreaterThan(foo, 5) || foo < 100;
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThan(78, 5) OrElse 78 LessThan 100");
        }

        [Fact]
        public void Should_Retrieve_Method_And_Display_Null_Arguments()
        {
            Expression<Func<bool>> exp = () => new TestValues().IsStringNullOrEmpty(null);
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsStringNullOrEmpty(null)");
        }

        [Fact]
        public void Should_Retrieve_Static_Method()
        {
            var foo = 78;
            Expression<Func<bool>> exp = () => TestValues.IsGreaterThanStatic(foo, 5) || foo < 100;
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThanStatic(78, 5) OrElse 78 LessThan 100");
        }

        [Fact]
        public void Should_Retrieve_Static_Method_With_Const_Arguments()
        {
            Expression<Func<bool>> exp = () => TestValues.IsGreaterThanStatic(78, 5);
            var res = ExpressionParser.GetBodyText(exp);
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
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThanStatic(50, 5)");
        }

        [Fact]
        public void Should_Retrieve_Expression_Arguments()
        {
            Expression<Func<int, int, int>> sum = (x, y) => x + y;
            var res = ExpressionParser.GetBodyText(sum);
            res.Should().BeEquivalentTo("(x, y) => x Add y");
        }

        [Fact]
        public void Should_Retrieve_Expression_Arguments_And_Static_Variable()
        {
            Expression<Func<int, int, int>> sum = (x, y) => x + y + 50;
            var res = ExpressionParser.GetBodyText(sum);
            res.Should().BeEquivalentTo("(x, y) => x Add y Add 50");
        }

        [Fact]
        public void Should_Retrieve_Expression_Arguments_And_Static_Variable_Subtract()
        {
            var foo = 50;
            Expression<Func<int, int, int>> sum = (x, y) => x + y - foo;
            var res = ExpressionParser.GetBodyText(sum);
            res.Should().BeEquivalentTo("(x, y) => x Add y Subtract 50");
        }

        [Fact]
        public void Should_Retrieve_Action_Expression()
        {
            Expression<Action> exp = () => TestValues.IsGreaterThanStatic(78, 5);
            var res = ExpressionParser.GetBodyText(exp);
            res.Should().BeEquivalentTo("() => TestValues.IsGreaterThanStatic(78, 5)");
        }
    }
}