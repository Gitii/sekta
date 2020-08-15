using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class ComboBox : DataElement
    {
        public ComboBox()
        {
            NoSort = false;
        }

        public string label { get; set; }

        /// <remarks/>
        public string @default { get; set; }

        /// <remarks/>
        [XmlElement("suggestion")]
        public string[] Suggestion { get; set; }

        /// <remarks/>
        [XmlAttribute("noSort")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool NoSort { get; set; }
    }
}