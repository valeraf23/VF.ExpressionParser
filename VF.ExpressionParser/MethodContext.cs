using System;
using System.Collections.Generic;

namespace VF.ExpressionParser
{
    internal class ArgumentMetadata
    {
        public ArgumentMetadata(string? name, Type parameterType)
        {
            Name = name;
            ParameterType = parameterType;
        }

        public string? Name { get; }
        public Type ParameterType { get; }
    }

    public class ArgumentContext
    {
        public ArgumentContext(string? name, Type parameterType, object? value)
        {
            Name = name;
            ParameterType = parameterType;
            Value = value;
        }

        public string? Name { get; }
        public Type ParameterType { get; }
        public object? Value { get; }
    }

    public class MethodContext
    {
        public MethodContext(string name, Type? reflectedType, List<ArgumentContext> arguments)
        {
            Name = name;
            ReflectedType = reflectedType;
            Arguments = arguments;
        }

        public string Name { get; }
        public Type? ReflectedType { get; }
        public List<ArgumentContext> Arguments { get; }
    }
}