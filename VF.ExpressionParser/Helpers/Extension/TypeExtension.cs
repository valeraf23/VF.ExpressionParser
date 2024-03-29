﻿using System;

namespace VF.ExpressionParser.Helpers.Extension
{
    public static class TypeExtension
    {
        public static bool IsPrimitiveOrString(this Type type) =>
            type.IsPrimitive || type == typeof(string) || type.IsEnum;
    }
}