# VF.ExpressionParser

[![NuGet.org](https://img.shields.io/nuget/v/VF.ExpressionParser.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/VF.ExpressionParser/)
![NuGet Downloads](https://img.shields.io/nuget/dt/VF.ExpressionParser)
[![.NET Actions Status](https://github.com/valeraf23/VF.ExpressionParser/workflows/.NET/badge.svg)](https://github.com/valeraf23/VF.ExpressionParser)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**VF.ExpressionParser** turns a LINQ expression tree into a human-readable string — and, unlike `Expression.ToString()`, it inlines the *captured runtime values* of closures, fields, and properties. That makes it ideal for debugging, structured logging, and audit trails where you need to see not just the shape of an expression but the data that flowed through it.

```csharp
Expression<Func<int, bool>> exp = x => x > 10 && x < 20;
Console.WriteLine(ExpressionExtension.ConvertToString(exp));
// (x) => x > 10 && x < 20
```

## Table of Contents

- [Why VF.ExpressionParser?](#why-vfexpressionparser)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage](#usage)
  - [Inlining captured values](#inlining-captured-values)
  - [Method calls](#method-calls)
  - [Lower-level API](#lower-level-api)
- [Output format](#output-format)
- [Behavior notes & limitations](#behavior-notes--limitations)
- [Building and testing](#building-and-testing)
- [Contributing](#contributing)
- [License](#license)

## Why VF.ExpressionParser?

The built-in `Expression.ToString()` only shows the structure of an expression. Closed-over variables appear as opaque display-class member accesses, so you cannot tell what value the expression actually used at runtime:

```csharp
var threshold = 78;
Expression<Func<bool>> exp = () => threshold > 2;

exp.ToString();
// () => (value(...).threshold > 2)        ← the captured 78 is hidden

ExpressionExtension.ConvertToString(exp);
// () => threshold(78) > 2                  ← captured value inlined
```

## Features

- Convert complex LINQ expression trees into readable strings.
- Inline captured values from closures, fields, and properties, rendered as `name(value)`.
- Resolve instance and static method calls, including their arguments.
- Render unary conversions, conditionals, and the common binary/logical operators.
- Lightweight, dependency-light, with a one-method entry point.

## Requirements

- Target frameworks: `net6.0`, `netstandard2.1`, and `netstandard2.0` — so it runs on .NET 6+, .NET Core, .NET Framework 4.6.1+, Mono, Xamarin, and Unity.
- Single transitive dependency: [`OneOf`](https://www.nuget.org/packages/OneOf/).

## Installation

```bash
dotnet add package VF.ExpressionParser
```

Or with the NuGet Package Manager console:

```powershell
Install-Package VF.ExpressionParser
```

## Quick Start

```csharp
using System;
using System.Linq.Expressions;
using VF.ExpressionParser;

Expression<Func<int, int, int>> sum = (x, y) => x + y;

Console.WriteLine(ExpressionExtension.ConvertToString(sum));
// (x, y) => x + y
```

`ConvertToString` accepts any `Expression` (the parameter is nullable), so you can pass a full `LambdaExpression` or any sub-expression you have a handle on.

## Usage

> The snippets below mirror the project's test suite, so their outputs are exactly what the library produces.

### Inlining captured values

Local variables, fields, and properties referenced inside the expression are evaluated and printed as `name(value)`. Lambda *parameters* are printed as plain names, since they have no value until the expression runs.

```csharp
public class SomeClass
{
    public int SomeNumber;
    public OtherClass Child { get; init; }
}

public class OtherClass
{
    public int SomeNumber { get; init; }
    public string? SomeString { get; init; }
}

var s = new SomeClass
{
    Child = new OtherClass { SomeNumber = 1, SomeString = "s" },
    SomeNumber = 2
};
var foo = 78;

Expression<Func<SomeClass, bool>> exp =
    a => s.Child.SomeNumber == 1 && a.SomeNumber == 3 && s.Child.SomeString == "s" && foo > 0;

Console.WriteLine(ExpressionExtension.ConvertToString(exp));
// (a) => s.Child.SomeNumber(1) == 1 && a.SomeNumber == 3 && s.Child.SomeString("s") == "s" && foo(78) > 0
```

Note how `a` (the parameter) stays bare, while the captured `s.Child.SomeNumber`, `s.Child.SomeString`, and `foo` show their runtime values.

### Method calls

Instance and static method calls are rendered as `DeclaringType.Method(args)`, with captured arguments inlined the same way:

```csharp
var foo = 78;

Expression<Func<bool>> instanceCall = () => new TestValues().IsGreaterThan(foo, 5);
Console.WriteLine(ExpressionExtension.ConvertToString(instanceCall));
// () => TestValues.IsGreaterThan(foo(78), 5)

Expression<Func<bool>> staticCall = () => TestValues.IsGreaterThanStatic(foo, 5);
Console.WriteLine(ExpressionExtension.ConvertToString(staticCall));
// () => TestValues.IsGreaterThanStatic(foo(78), 5)

Expression<Func<string, bool>> negated = x => !string.IsNullOrEmpty(x);
Console.WriteLine(ExpressionExtension.ConvertToString(negated));
// (x) => Not(String.IsNullOrEmpty(x))
```

### Lower-level API

`ExpressionExtension.ConvertToString` is a thin wrapper over `ExpressionWriterVisitor`, an `ExpressionVisitor` you can use directly when you want to supply your own `StringBuilder`:

```csharp
var visitor = new ExpressionWriterVisitor(new StringBuilder());
string text = visitor.ConvertToString(expression);
```

## Output format

| Element | Rendered as | Example |
| --- | --- | --- |
| Lambda | `(params) => body` | `(x, y) => x + y` |
| Lambda parameter | bare name | `a` |
| Captured value (local/field/property) | `name(value)` | `foo(78)` |
| Instance method call | `Type.Method(args)` | `TestValues.Sum(foo(78), 5)` |
| Static method call | `Type.Method(args)` | `TestValues.IsGreaterThanStatic(78, 5)` |
| Unary negation | `Not(...)` | `Not(String.IsNullOrEmpty(x))` |
| Conversion | `(FullTypeName)(operand)` | `(System.Double)(testAmountField(1000))` |
| Logical / comparison | `&&`, `\|\|`, `==`, `>`, `<`, `>=`, `<=` | `foo(78) > 0 \|\| foo(78) < 100` |

## Behavior notes & limitations

- **Compile-time constants are folded by the C# compiler before the library sees them.** An expression like `() => foo > 2` where `foo` is a `const int = 78` reaches the parser already reduced, producing `() => True` rather than `foo(78) > 2`. Use a local variable if you want the value inlined.
- **Member access on a `null` value throws.** Inlining a captured value evaluates the member chain, so `s.Child.SomeNumber` throws a `NullReferenceException` when `s.Child` is `null`. Guard such expressions before converting them.
- The operator set is the common arithmetic/comparison/logical operators (see the table above); unmapped node types render as `???`.

## Building and testing

```bash
dotnet build
dotnet test
```

The test suite (xUnit + FluentAssertions) doubles as living documentation of the output format — see [`VF.ExpressionParser.Tests`](VF.ExpressionParser.Tests/ExpressionParserTests.cs).

## Contributing

Contributions are welcome:

1. Fork the repository and create a feature branch.
2. Add or update tests that capture the expected output of your change.
3. Run `dotnet test` to confirm the suite is green.
4. Open a pull request with a clear description of the change and its motivation.

## License

Licensed under the MIT License — see [LICENSE](LICENSE) for details.
