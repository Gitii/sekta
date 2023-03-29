using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class ValueList
{
    /// <remarks/>
    [XmlElement("item")]
    public ValueItem[] Items { get; set; }

    /// <remarks/>
    [XmlAttribute("defaultKey")]
    public string DefaultKey { get; set; }
}
