using System;
using TlvSerializer.Tests.Fixtures;
using Xunit;

namespace TlvSerializer.Tests
{
    public class TlvSerializeTest
    {
        [Fact]
        public void ShouldSerializeObjectToTlvWithSuccessful()
        {
            var authorization = new Authorization
            {
                Id = "001",
                Authorizer = "CIELO",
                Operation = "CREDIT"
            };

            var bufferRx = TlvSerialize.Serialize(authorization);
            
            Assert.Equal("0103303031", Convert.ToHexString(bufferRx)[..10]);
            Assert.Equal("02054349454C4F", Convert.ToHexString(bufferRx)[10..24]);
            Assert.Equal("0306435245444954", Convert.ToHexString(bufferRx)[24..40]);
        }
        
        [Theory]
        [InlineData("010330303102054349454C4F0306435245444954")]
        public void ShouldDeserializeTlvToDictionaryWithSuccessful(string bufferRx)
        {
            var content = TlvSerialize.Serialize(bufferRx);
            
            Assert.Equal(0x01, content[0x01].Tag);
            Assert.Equal(3, content[0x01].Length);
            Assert.Equal("001", content[0x01].Value);
            
            Assert.Equal(0x02, content[0x02].Tag);
            Assert.Equal(5, content[0x02].Length);
            Assert.Equal("CIELO", content[0x02].Value);
            
            Assert.Equal(0x03, content[0x03].Tag);
            Assert.Equal(6, content[0x03].Length);
            Assert.Equal("CREDIT", content[0x03].Value);
        }
        
        [Theory]
        [InlineData("010330303102054349454C4F0306435245444954", "001", "CIELO", "CREDIT")]
        [InlineData("010330303202045245444503084352C3894449544F", "002", "REDE", "CRÉDITO")]
        [InlineData(
            "0103303033021CC3A77E5E24252A282940232524212122272B3D3D60602727C2B427200306435245444954", 
            "003", "ç~^$%*()@#%$!!\"'+==``''´' ", 
            "CREDIT")]
        public void ShouldDeserializeTlvToObjectWithSuccessful(
            string bufferRx, 
            string id, 
            string authorizer, 
            string operation)
        {
            var content = TlvSerialize.Deserialize<Authorization>(bufferRx);
            
            Assert.Equal(id, content.Id);
            Assert.Equal(authorizer, content.Authorizer);
            Assert.Equal(operation, content.Operation);
        }
        
        [Theory]
        [InlineData("010330303102054349454C4F0306435245444954", "001(003): 001\n002(005): CIELO\n003(006): CREDIT\n")]
        public void ShouldDumpTlvFromObjectWithSuccessful(string bufferRx, string dumpRx)
        {
            var dump = TlvSerialize.Dump(bufferRx);
            
            Assert.Equal(dumpRx, dump);
        }
        
        [Fact]
        public void ShouldDumpTlvFromHexWithSuccessful()
        {
            var dumpRx = TlvSerialize.Dump(new Authorization
            {
                Id = "001",
                Authorizer = "CIELO",
                Operation = "CREDIT"
            });
            
            Assert.Equal("001(003): 001\n002(005): CIELO\n003(006): CREDIT\n", dumpRx);
        }
        
        [Fact]
        public void ShouldSerializeObjectToTlvWithCustomAttributesWithSuccessful()
        {
            var bufferRx = TlvSerialize.Serialize(new AuthorizationCustom
            {
                Id = 1,
                Authorizer = "CIELO",
                Operation = "CREDIT",
                CreatedAt = DateTime.Parse("2022-01-01 01:00:00"),
                DebugMode = true,
                Amount = 10.98,
                Enable = 1
            });
            
            Assert.Equal("3230323230313031303130303030", Convert.ToHexString(bufferRx)[40..68]);
        }
        
        [Theory]
        [InlineData("01013102054349454C4F0306435245444954040E3230323230313031303130303030050131060130070531302E3938080131")]
        public void ShouldDeserializeTlvToObjectWithCustomAttributesWithSuccessful(string bufferRx)
        {
            var content = TlvSerialize.Deserialize<AuthorizationCustom>(bufferRx);
            
            Assert.Equal(1, content.Id);
            Assert.Equal("CIELO", content.Authorizer);
            Assert.Equal("CREDIT", content.Operation);
            Assert.Equal("2022-01-01 01:00:00", content.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.True(content.DebugMode);
            Assert.Equal(10.98, content.Amount);
            Assert.Equal(0x01, content.Enable);
        }
    }
}