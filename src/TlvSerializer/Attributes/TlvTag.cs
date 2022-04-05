using System;

namespace TlvSerializer.Attributes
{
    /// <summary>
    ///     TLV (type, length and value) Attribute
    /// </summary>
    public class TlvTag : Attribute
    {
        /// <summary>
        ///     Type identifier in byte
        /// </summary>
        public byte Id { get; set; }
        
        /// <summary>
        ///     Pattern
        /// </summary>
        public string Pattern { get; set; }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        public TlvTag()
        {
            
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public TlvTag(byte id)
        {
            Id = id;
        }
    }
}