using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class DecimalElement : BaseElement
    {
        private string idField;

        private string clientExtensionField;

        private string keyField;

        private string valueNameField;

        public DecimalElement()
        {
            required = false;
            minValue = ((uint) (0));
            maxValue = ((uint) (9999));
            storeAsText = false;
            soft = false;
        }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool required { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "0")]
        public uint minValue { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "9999")]
        public uint maxValue { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool storeAsText { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool soft { get; set; }
    }
}