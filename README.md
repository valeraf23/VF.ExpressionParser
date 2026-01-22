# VF.ExpressionParser

[![NuGet.org](https://img.shields.io/nuget/v/VF.ExpressionParser.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/VF.ExpressionParser/)
![NuGet Downloads](https://img.shields.io/nuget/dt/VF.ExpressionParser)
[![.NET Actions Status](https://github.com/valeraf23/VF.ExpressionParser/workflows/.NET/badge.svg)](https://github.com/valeraf23/VF.ExpressionParser)

`VF.ExpressionParser` is a .NET library that converts LINQ expression trees into readable string representations. It is useful for debugging, logging, and auditing complex expressions.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage](#usage)
  - [Basic Example](#basic-example)
  - [Advanced Example](#advanced-example)
- [API Notes](#api-notes)
- [Build and Test](#build-and-test)
- [Contributing](#contributing)
- [License](#license)

## Features

- Convert complex LINQ expressions into readable string formats.
- Evaluate captured values for clearer output.
- Useful for debugging, logging, and audit trails.
- Lightweight package with a simple API surface.

## Requirements

- .NET SDK (see the project file for the exact target frameworks).
- Supported platforms: any platform that supports the targeted .NET frameworks.

## Installation

Install via the .NET CLI:

```bash
dotnet add package VF.ExpressionParser
```

Or via the NuGet Package Manager:

```powershell
Install-Package VF.ExpressionParser
```

## Quick Start

```csharp
using System;
using System.Linq.Expressions;
using VF.ExpressionParser;

Expression<Func<int, bool>> exp = x => x > 10 && x < 20;
var res = ExpressionExtension.ConvertToString(exp);

Console.WriteLine(res);
```

## Usage

### Basic Example

```csharp
using System;
using System.Linq.Expressions;
using VF.ExpressionParser;

var s = new SomeClass
{
    Child = new OtherClass
    {
        SomeNumber = 1
    },
    SomeNumber = 2
};

var foo = 78;

Expression<Func<SomeClass, bool>> exp = a => s.Child.SomeNumber == 1 &&
    a.SomeNumber == 3 && s.SomeNumber == 3 &&
    foo > 0 ||
    new TestValues().Sum(s.Child.SomeNumber, 5) > 5;

var res = ExpressionExtension.ConvertToString(exp);
Console.WriteLine(res);
```

Output:

```plaintext
"(a) => s.Child.SomeNumber(1) == 1 && a.SomeNumber == 3 && s.SomeNumber(2) == 3 && foo(78) > 0 || TestValues.Sum(s.Child.SomeNumber(1), 5) > 5"
```

### Advanced Example

```csharp
using System;
using System.Linq.Expressions;
using VF.ExpressionParser;

var s = new SomeClass
{
    Child = new OtherClass
    {
        SomeNumber = 1,
        Nested = new NestedClass
        {
            Value = 10
        }
    },
    SomeNumber = 2,
    StringProperty = "Test"
};

Expression<Func<SomeClass, bool>> exp = a => s.Child.SomeNumber == a.SomeNumber &&
    s.StringProperty == "Test" &&
    s.Child.Nested.Value > 5;

var res = ExpressionExtension.ConvertToString(exp);
Console.WriteLine(res);
```

Output:

```plaintext
"(a) => s.Child.SomeNumber(1) == a.SomeNumber && s.StringProperty(\"Test\") == \"Test\" && s.Child.Nested.Value(10) > 5"
```

## API Notes

- `ExpressionExtension.ConvertToString(Expression expression)` returns the parsed string output.
- The output includes captured values (for example, `foo(78)`), which makes debugging and logging easier.

## Build and Test

```bash
dotnet build
```

```bash
dotnet test
```

## Contributing

1. Fork the repository.
2. Create a feature branch.
3. Add tests for your change when applicable.
4. Submit a pull request with a clear description.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
