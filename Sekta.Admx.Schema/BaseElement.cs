using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public class BaseElement
{
    [XmlAttribute("id")]
    public string id { get; set; }

    [XmlAttribute("clientExtension")]
    public string clientExtension { get; set; }

    [XmlAttribute("key")]
    public string key { get; set; }

    [XmlAttribute("valueName")]
    public string valueName { get; set; }
}
