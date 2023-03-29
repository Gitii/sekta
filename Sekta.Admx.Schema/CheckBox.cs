using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class CheckBox : DataElementContent
{
    public CheckBox()
    {
        DefaultChecked = false;
    }

    /// <remarks/>
    [XmlAttribute("defaultChecked")]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool DefaultChecked { get; set; }

    [XmlAttribute("defaultValue")]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool DefaultValue { get; set; }
}
