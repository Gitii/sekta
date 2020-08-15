using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class LocalizationPresentationTable
    {
        /// <remarks/>
        [XmlElement("presentation")]
        public PolicyPresentation[] presentation { get; set; }
    }
}