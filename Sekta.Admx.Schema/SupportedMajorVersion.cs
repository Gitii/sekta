using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class SupportedMajorVersion
    {
        /// <remarks/>
        [XmlElement("minorVersion")]
        public SupportedMinorVersion[] minorVersion { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string displayName { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public uint versionIndex { get; set; }
    }
}