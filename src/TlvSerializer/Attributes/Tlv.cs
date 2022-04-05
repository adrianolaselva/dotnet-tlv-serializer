namespace TlvSerializer.Attributes
{
    /// <summary>
    ///     TLV (type, length and value) Structure
    /// </summary>
    public class Tlv
    {
        /// <summary>
        ///     Tag in byte
        /// </summary>
        public byte Tag { get; set; }
        
        /// <summary>
        ///     Length
        /// </summary>
        public int Length { get; set; }
        
        /// <summary>
        ///     Value in hex
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        public Tlv()
        {
            
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        public Tlv(byte id, int length)
        {
            Tag = id;
            Length = length;
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        public Tlv(byte id, int length, string value)
        {
            Tag = id;
            Length = length;
            Value = value;
        }
    }
}