using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class DecimalTextBox : DataElementContent
{
    public DecimalTextBox()
    {
        DefaultValue = 1;
        Spin = true;
        SpinStep = 1;
    }

    /// <remarks/>
    [XmlAttribute("defaultValue")]
    [System.ComponentModel.DefaultValueAttribute(typeof(uint), "1")]
    public uint DefaultValue { get; set; }

    /// <remarks/>
    [XmlAttribute("spin")]
    [System.ComponentModel.DefaultValueAttribute(true)]
    public bool Spin { get; set; }

    /// <remarks/>
    [XmlAttribute("spinStep")]
    [System.ComponentModel.DefaultValueAttribute(typeof(uint), "1")]
    public uint SpinStep { get; set; }
}
