# VF.ExpressionParser

[![.NET Actions Status](https://github.com/valeraf23/VF.ExpressionParser/workflows/.NET/badge.svg)](https://github.com/valeraf23/VF.ExpressionParser)

## Example:

```csharp
            var foo = 78;
            Expression<Func<bool>> exp = () => foo > 2 && foo > 3 || foo < 100;
            var res = ExpressionParser.GetBodyText(exp);
            Console.WriteLine(res); //78 GreaterThan 2 AndAlso 78 GreaterThan 3 OrElse 78 LessThan 100            
```
