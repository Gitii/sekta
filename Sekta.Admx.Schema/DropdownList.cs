using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class DropdownList : DataElementContent
{
    public DropdownList()
    {
        NoSort = false;
    }

    /// <remarks/>
    [XmlAttribute("noSort")]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool NoSort { get; set; }

    /// <remarks/>
    [XmlAttribute("defaultItem")]
    public uint DefaultItem { get; set; }

    /// <remarks/>
    [XmlIgnore()]
    public bool defaultItemSpecified { get; set; }
}
