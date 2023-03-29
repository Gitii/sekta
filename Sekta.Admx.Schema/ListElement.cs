using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class ListElement : BaseElement
{
    private string idField;

    private string clientExtensionField;

    private string keyField;

    public ListElement()
    {
        additive = false;
        expandable = false;
        explicitValue = false;
    }

    /// <remarks/>
    [XmlAttribute()]
    public string valuePrefix { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool additive { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool expandable { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool explicitValue { get; set; }
}
