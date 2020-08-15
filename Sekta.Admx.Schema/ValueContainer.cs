using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    [DataContract]
    public partial class ValueContainer : IEquatable<ValueContainer>
    {
        [XmlElement("decimal", typeof(ValueDecimal))]
        [XmlElement("delete", typeof(ValueDelete))]
        [XmlElement("string", typeof(ValueString))]
        [DataMember]
        public ValueBase Item { get; set; }

        public virtual string Serialize()
        {
            var serializer = new DataContractJsonSerializer(typeof(ValueContainer));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);

                return new StreamReader(stream).ReadToEnd();
            }
        }

        public static ValueContainer Deserialze(string strValue)
        {
            var serializer = new DataContractJsonSerializer(typeof(ValueContainer));

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(strValue);
                stream.Position = 0;

                var value = (ValueContainer) serializer.ReadObject(stream);

                return value;
            }
        }

        public bool Equals(ValueContainer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Item, other.Item);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueContainer) obj);
        }

        public override int GetHashCode()
        {
            return (Item != null ? Item.GetHashCode() : 0);
        }

        public static bool operator ==(ValueContainer left, ValueContainer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueContainer left, ValueContainer right)
        {
            return !Equals(left, right);
        }
    }
}