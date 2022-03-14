using System;

namespace common.utils
{
    /// <summary>
    /// A type to representing 8 bools
    /// </summary>
    public struct BitField
    {
        private byte _value;

        public BitField(byte value = 0)
        {
            _value = value;
        }

        public bool this[byte index]
        {
            get
            {
                // Shift to right until index and check
                // And with 1 to compare that index's value
                var value = (_value >> index) & 1;
                return value == 1;
            }
            set
            {
                // Shifts 1 to position and then modifies the bit at that position
                
                if (value)
                    _value |= (byte)(1 << index);
                else
                    _value &= (byte)~(1 << index);
            }
        }
        
        public byte GetByte()
        {
            return _value;
        }

        public void SetByte(byte value)
        {
            _value = value;
        }
        
        /// <summary>
        /// Returns a String which represents the object instance.  The default for an object is to return the fully qualified name of the class.
        /// </summary>
        /// <returns>Binary string with index 0 representing the least significant bit and index 7 representing most significant bit.</returns>
        public override string ToString()
        {
            var output = String.Empty;
            for (byte i = 0; i < 8; ++i) 
                output += ((_value >> i) & 1).ToString();
            return output;
        }
    }
}