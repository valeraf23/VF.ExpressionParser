# VF.ExpressionParser

[![NuGet.org](https://img.shields.io/nuget/v/ObjectComparator.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/ObjectComparator/)
![Nuget](https://img.shields.io/nuget/dt/ObjectComparator)
[![.NET Actions Status](https://github.com/valeraf23/VF.ExpressionParser/workflows/.NET/badge.svg)](https://github.com/valeraf23/VF.ExpressionParser)

## Example:

```csharp
            var foo = 78;
            Expression<Func<bool>> exp = () => foo > 2 && foo > 3 || foo < 100;
            var res = ExpressionParser.GetBodyText(exp);
            Console.WriteLine(res); //78 GreaterThan 2 AndAlso 78 GreaterThan 3 OrElse 78 LessThan 100            
```
