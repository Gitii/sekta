using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class SupportedOnRange
    {
        /// <remarks/>
        [XmlAttribute()]
        public string @ref { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public uint minVersionIndex { get; set; }

        /// <remarks/>
        [XmlIgnore()]
        public bool minVersionIndexSpecified { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public uint maxVersionIndex { get; set; }

        /// <remarks/>
        [XmlIgnore()]
        public bool maxVersionIndexSpecified { get; set; }
    }
}