using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

/// <remarks/>
public partial class Category
{
    /// <remarks/>
    [XmlElement("annotation")]
    public Annotation[] annotation { get; set; }

    [XmlElement("parentCategory", typeof(CategoryReference))]
    public CategoryReference ParentCategory { get; set; }

    /// <remarks/>
    [XmlElement("seeAlso")]
    public string[] seeAlso { get; set; }

    /// <remarks/>
    public string keywords { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string name { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string displayName { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string explainText { get; set; }
}
