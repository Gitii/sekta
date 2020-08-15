using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class Annotation
    {
        /// <remarks/>
        [XmlAnyElement()]
        public System.Xml.XmlNode[] Any { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string Application { get; set; }
    }
}