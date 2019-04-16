using System;

namespace Angelo.Common.Extensions
{
    public static class EnumExtensions
    {
        public static EnumType ToEnumOrDefault<EnumType>(this string enumText, EnumType defaultValue) where EnumType : struct, IConvertible
        {
            EnumType enumValue;
            if (Enum.TryParse(enumText, true, out enumValue))
                return enumValue;
            else
                return defaultValue;
        }

        public static EnumType ToEnum<EnumType>(this string enumText) where EnumType : struct, IConvertible
        {
            return (EnumType)Enum.Parse(typeof(EnumType), enumText);
        }

    }
}

