using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class LocalizedString
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlText()]
    public string Value { get; set; }
}
