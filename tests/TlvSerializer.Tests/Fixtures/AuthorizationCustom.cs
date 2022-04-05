using System;
using TlvSerializer.Attributes;

namespace TlvSerializer.Tests.Fixtures
{
    public class AuthorizationCustom
    {
        [TlvTag(Id = 0x01)]
        public int Id { get; set; }
        
        [TlvTag(Id = 0x02)]
        public string Authorizer { get; set; }
        
        [TlvTag(Id = 0x03)]
        public string Operation { get; set; }
        
        [TlvTag(Id = 0x04, Pattern = "yyyyMMddHHmmss")]
        public DateTime CreatedAt { get; set; }
        
        [TlvTag(Id = 0x05)]
        public bool DebugMode { get; set; }
        
        [TlvTag(Id = 0x06)]
        public int Installments { get; set; }
        
        [TlvTag(Id = 0x07)]
        public double Amount { get; set; }
        
        [TlvTag(Id = 0x08)]
        public byte Enable { get; set; }
    }
}