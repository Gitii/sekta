using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

/// <remarks/>
public partial class PolicyDefinition
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
    public SupportedOnReference supportedOn { get; set; }

    /// <remarks/>
    public ValueContainer enabledValue { get; set; }

    /// <remarks/>
    public ValueContainer disabledValue { get; set; }

    /// <remarks/>
    public ValueList enabledList { get; set; }

    /// <remarks/>
    public ValueList disabledList { get; set; }

    /// <remarks/>
    [XmlArray("elements")]
    [XmlArrayItem("boolean", typeof(BooleanElement), IsNullable = false)]
    [XmlArrayItem("decimal", typeof(DecimalElement), IsNullable = false)]
    [XmlArrayItem("enum", typeof(EnumerationElement), IsNullable = false)]
    [XmlArrayItem("list", typeof(ListElement), IsNullable = false)]
    [XmlArrayItem("text", typeof(TextElement), IsNullable = false)]
    public BaseElement[] Elements { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string name { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public PolicyClass @class { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string displayName { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string explainText { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string presentation { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string key { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string valueName { get; set; }
}
