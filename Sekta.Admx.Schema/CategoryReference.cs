using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class CategoryReference
{
    /// <remarks/>
    [XmlAttribute("ref")]
    public string ReferenceName { get; set; }
}
