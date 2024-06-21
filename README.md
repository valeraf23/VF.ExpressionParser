
# VF.ExpressionParser

[![NuGet.org](https://img.shields.io/nuget/v/VF.ExpressionParser.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/VF.ExpressionParser/)
![Nuget](https://img.shields.io/nuget/dt/VF.ExpressionParser)
[![.NET Actions Status](https://github.com/valeraf23/VF.ExpressionParser/workflows/.NET/badge.svg)](https://github.com/valeraf23/VF.ExpressionParser)

## Table of Contents
- [Overview](#overview)
- [Installation](#installation)
- [Usage](#usage)
  - [Basic Example](#basic-example)
  - [Advanced Example](#advanced-example)
- [Features](#features)
- [Contributing](#contributing)
- [License](#license)

## Overview

`VF.ExpressionParser` is a powerful .NET library designed to parse, analyze, and convert complex LINQ expressions into readable string formats. This tool is essential for developers who need to visualize, debug, or audit LINQ expressions efficiently.

## Installation

You can install the `VF.ExpressionParser` package via NuGet:

```bash
dotnet add package VF.ExpressionParser
```

Or via the NuGet Package Manager:

```powershell
Install-Package VF.ExpressionParser
```

## Usage

### Basic Example

Below is a basic example demonstrating how to use `VF.ExpressionParser`:

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

Here is a more advanced example showcasing additional capabilities of `VF.ExpressionParser`:

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
"(a) => s.Child.SomeNumber(1) == a.SomeNumber && s.StringProperty("Test") == "Test" && s.Child.Nested.Value(10) > 5"
```

## Features

- **Expression Parsing**: Convert complex LINQ expressions into readable string formats.
- **Debugging Aid**: Helps in visualizing LINQ expressions for debugging purposes.
- **Audit and Logging**: Useful for logging expressions for audit trails.
- **Customization**: Provides flexibility to customize the parsing output.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
