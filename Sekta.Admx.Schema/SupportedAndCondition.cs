using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class SupportedAndCondition
    {
        /// <remarks/>
        [XmlElement("range", typeof(SupportedOnRange))]
        [XmlElement("reference", typeof(SupportedOnReference))]
        public object[] Items { get; set; }
    }
}