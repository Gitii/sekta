using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class EnumerationElement : BaseElement
{
    private string idField;

    private string clientExtensionField;

    private string keyField;

    private string valueNameField;

    public EnumerationElement()
    {
        required = false;
    }

    /// <remarks/>
    [XmlElement("item")]
    public EnumerationElementItem[] item { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool required { get; set; }
}
