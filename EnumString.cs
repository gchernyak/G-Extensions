using System;
using System.Reflection;

namespace G.Extensions
{
    public static class EnumString
    {
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();
            FieldInfo fi = type.GetField(value.ToString());
            var attrs = fi.GetCustomAttributes(typeof (EnumValue), false) as EnumValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }
            return output;
        }
    }
}