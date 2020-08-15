using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class SupportedOnReference
    {
        /// <remarks/>
        [XmlAttribute()]
        public string @ref { get; set; }
    }
}