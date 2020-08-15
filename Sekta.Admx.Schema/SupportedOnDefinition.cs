using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class SupportedOnDefinition
    {
        /// <remarks/>
        [XmlElement("and", typeof(SupportedAndCondition))]
        [XmlElement("or", typeof(SupportedOrCondition))]
        public object Item { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string displayName { get; set; }
    }
}