using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class EnumerationElementItem
    {
        /// <remarks/>
        public ValueContainer value { get; set; }

        /// <remarks/>
        public ValueList valueList { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string displayName { get; set; }
    }
}