using TlvSerializer.Attributes;

namespace TlvSerializer.Tests.Fixtures
{
    public class Authorization
    {
        [TlvTag(Id = 0x01)]
        public string Id { get; set; }
        
        [TlvTag(Id = 0x02)]
        public string Authorizer { get; set; }
        
        [TlvTag(Id = 0x03)]
        public string Operation { get; set; }
    }
}