using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class SupportedProduct
    {
        /// <remarks/>
        [XmlElement("majorVersion")]
        public SupportedMajorVersion[] majorVersion { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string displayName { get; set; }
    }
}