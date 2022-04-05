using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TlvSerializer.Attributes;

namespace TlvSerializer
{
    /// <summary>
    ///     TLV (type, length and value) Serialize
    /// </summary>
    public static class TlvSerialize
    {
        /// <summary>
        ///     Serialize to TLV object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Serialize(object content)
        {
            var properties = content.GetType().GetProperties();
            
            var bufferRx = Array.Empty<byte>();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetCustomAttributes().FirstOrDefault() is not TlvTag customAttribute) 
                    continue;
                
                var attributeContent = Type.GetTypeCode(propertyInfo.GetValue(content)!.GetType()) switch
                {
                    TypeCode.DateTime => (propertyInfo.GetValue(content) is DateTime ? 
                            (DateTime) propertyInfo.GetValue(content)! : default)
                        .ToString(customAttribute.Pattern ?? "yyyy-MM-dd'T'HH:mm:ss"),
                    TypeCode.Boolean => bool.Parse(
                        propertyInfo.GetValue(content)?.ToString() ?? string.Empty) ? "1" : "0",
                    _ => propertyInfo.GetValue(content)?.ToString() ?? string.Empty
                };
                
                var tag = customAttribute!.Id;
                var value = Encoding.UTF8.GetBytes(attributeContent);
                var length = Convert.ToByte(value.Length);

                var bufferRxLength = bufferRx.Length;
                
                Array.Resize(ref bufferRx, bufferRxLength + new[] { tag, length }.Length);
                Array.Copy(
                    new[] { tag, length }, 
                    0, 
                    bufferRx, 
                    bufferRxLength, 
                    new[] { tag, length }.Length);

                bufferRxLength = bufferRx.Length;
                
                Array.Resize(ref bufferRx, bufferRxLength + value.Length);
                Array.Copy(
                    value, 
                    0, 
                    bufferRx, 
                    bufferRxLength, 
                    value.Length);
            }

            return bufferRx;
        }
        
        /// <summary>
        ///     Serialize From TLV string hex
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Dictionary<byte, Tlv> Serialize(string content) => Serialize(Convert.FromHexString(content));
        
        /// <summary>
        ///     Serialize From TLV byte array 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Dictionary<byte, Tlv> Serialize(byte[] content) => Serialize(content, content.Length);
        
        /// <summary>
        ///     Serialize From TLV byte array and legth
        /// </summary>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static Dictionary<byte, Tlv> Serialize(byte[] content, int length)
        {
            var parent = new Dictionary<byte, Tlv>();
            int bytesRead;
            for (var i = 0; i < length; i = bytesRead)
            {
                var tag = content[i];
                var len = Convert.ToInt32(content[i+1]);
                var value = content[(i+2)..(i+2+len)];
                bytesRead = i + 2 + len;

                parent.Add(tag, new Tlv
                {
                    Tag = tag,
                    Length = len,
                    Value = Encoding.UTF8.GetString(value)
                });
            }
            
            return parent;
        }
        
        /// <summary>
        ///     Deserialize From TLV string hex
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string content) where T : new() => 
            Deserialize<T>(Convert.FromHexString(content));
        
        /// <summary>
        ///     Deserialize From TLV byte array
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] content) where T : new() => Deserialize<T>(content, content.Length);
        
        /// <summary>
        ///     Deserialize From TLV byte array and length
        /// </summary>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] content, int length) where T : new()
        {
            var parent = new T();
            int bytesRead;
            for (var i = 0; i < length; i = bytesRead)
            {
                var tag = content[i];
                var len = Convert.ToInt32(content[i+1]);
                var value = content[(i+2)..(i+2+len)];
                bytesRead = i + 2 + len;

                var properties = parent.GetType().GetProperties();
                foreach (var propertyInfo in properties)
                {
                    var customAttribute = propertyInfo
                        .GetCustomAttributes()
                        .Where(x => x.GetType() == typeof(TlvTag))
                        .OfType<TlvTag>()
                        .FirstOrDefault(x => x.Id == tag);

                    if (customAttribute == null)
                        continue;
                    
                    switch (Type.GetTypeCode(parent.GetType().GetProperty(propertyInfo.Name)?.PropertyType))
                    {
                        case TypeCode.DateTime:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, DateTime
                                    .ParseExact(
                                        Encoding.UTF8.GetString(value),
                                        customAttribute.Pattern ?? "yyyy-MM-dd'T'HH:mm:ss", null));
                            break;
                        case TypeCode.Boolean:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Encoding.UTF8.GetString(value) == "1");
                            break;
                        case TypeCode.Int16:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Int16.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.Int32:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Int32.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.Int64:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Int32.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.UInt16:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, UInt16.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.UInt32:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, UInt32.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.UInt64:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, UInt64.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.Decimal:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Decimal.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.SByte:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, SByte.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.Byte:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, byte.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.Double:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Double.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.Char:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Char.Parse(Encoding.UTF8.GetString(value)));
                            break;
                        case TypeCode.String:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Encoding.UTF8.GetString(value));
                            break;
                        case TypeCode.Object:
                            parent
                                .GetType()
                                .GetProperty(propertyInfo.Name)?
                                .SetValue(parent, Encoding.UTF8.GetString(value));
                            break;
                        default:
                            throw new Exception(
                                $"Type {parent.GetType().GetProperty(propertyInfo.Name)?.PropertyType} not defined");
                    }
                }
            }
            
            return parent;
        }
        
        /// <summary>
        ///     Dump from string hex
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Dump(string content) => Dump(Convert.FromHexString(content));
        
        /// <summary>
        ///     Dump from object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Dump(object content) => Dump(Serialize(content));
        
        /// <summary>
        ///     Dump from byte
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Dump(byte[] content)
        {
            var dump = new StringBuilder();
            int bytesRead;
            for (var i = 0; i < content.Length; i = bytesRead)
            {
                var tag = content[i];
                var len = Convert.ToInt32(content[i + 1]);
                var value = content[(i + 2)..(i + 2 + len)];
                bytesRead = i + 2 + len;
                
                dump.Append($"{tag.ToString().PadLeft(3, '0')}" +
                            $"({len.ToString().PadLeft(3, '0')}): " +
                            $"{Encoding.UTF8.GetString(value)}\n");
            }

            return dump.ToString();
        }
    }
}