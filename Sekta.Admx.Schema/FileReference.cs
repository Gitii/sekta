using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class FileReference
{
    /// <remarks/>
    [XmlAttribute()]
    public string fileName { get; set; }
}
