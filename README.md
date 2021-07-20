# VF.ExpressionParser

[![NuGet.org](https://img.shields.io/nuget/v/VF.ExpressionParser.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/VF.ExpressionParser/)
![Nuget](https://img.shields.io/nuget/dt/VF.ExpressionParser)
[![.NET Actions Status](https://github.com/valeraf23/VF.ExpressionParser/workflows/.NET/badge.svg)](https://github.com/valeraf23/VF.ExpressionParser)

## Example:

```csharp
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
            //"(a) => s.Child.SomeNumber(1) == 1 && a.SomeNumber == 3 && s.SomeNumber(2) == 3 && foo(78) > 0 || TestValues.Sum(s.Child.SomeNumber(1), 5) > 5");         
```
