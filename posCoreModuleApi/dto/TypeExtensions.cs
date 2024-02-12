using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public static class TypeExtensions
    {
        public static bool IsNumericType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // If Nullable<T>, check if T is numeric
                return Nullable.GetUnderlyingType(type).IsNumericType();
            }
            return type == typeof(byte) ||
                    type == typeof(sbyte) ||
                    type == typeof(short) ||
                    type == typeof(ushort) ||
                    type == typeof(int) ||
                    type == typeof(uint) ||
                    type == typeof(long) ||
                    type == typeof(ulong) ||
                    type == typeof(float) ||
                    type == typeof(double) ||
                    type == typeof(decimal);
            }
        }
}