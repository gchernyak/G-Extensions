using System;

namespace G.Extensions
{
    public class EnumValue : Attribute
    {
        private readonly string _value;

        public EnumValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}