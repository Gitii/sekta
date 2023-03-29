using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public abstract class BaseDataElement
{
    [XmlAttribute("refId")]
    public string RefId { get; set; }
}
