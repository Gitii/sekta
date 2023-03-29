using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class TextBox : DataElement
{
    [XmlElement("label")]
    public string Label { get; set; }

    [XmlAttribute("defaultValue")]
    public string DefaultValue { get; set; }
}
