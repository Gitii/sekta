using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    [XmlInclude(typeof(ComboBox))]
    [XmlInclude(typeof(TextBox))]
    public abstract partial class DataElement : BaseDataElement
    {
        
    }
}