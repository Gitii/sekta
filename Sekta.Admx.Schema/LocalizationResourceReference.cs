using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class LocalizationResourceReference
    {
        public LocalizationResourceReference()
        {
            fallbackCulture = "en-US";
        }

        /// <remarks/>
        [XmlAttribute(DataType = "token")]
        public string minRequiredRevision { get; set; }

        /// <remarks/>
        [XmlAttribute(DataType = "language")]
        [System.ComponentModel.DefaultValueAttribute("en-US")]
        public string fallbackCulture { get; set; }
    }
}