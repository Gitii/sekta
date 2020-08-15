using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class TextElement : BaseElement
    {
        public TextElement()
        {
            required = false;
            maxLength = ((uint) (1023));
            expandable = false;
            soft = false;
        }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool required { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "1023")]
        public uint maxLength { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool expandable { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool soft { get; set; }
    }
}