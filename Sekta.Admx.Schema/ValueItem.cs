using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class ValueItem
    {
        [XmlElement("value")]
        public ValueContainer Value { get; set; }

        /// <remarks/>
        [XmlAttribute("key")]
        public string Key { get; set; }

        /// <remarks/>
        [XmlAttribute("valueName")]
        public string ValueName { get; set; }
    }
}