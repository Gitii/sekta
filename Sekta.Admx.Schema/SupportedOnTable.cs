using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class SupportedOnTable
{
    /// <remarks/>
    [XmlArrayItem("product", IsNullable = false)]
    public SupportedProduct[] products { get; set; }

    /// <remarks/>
    [XmlArrayItem("definition", IsNullable = false)]
    public SupportedOnDefinition[] definitions { get; set; }
}
