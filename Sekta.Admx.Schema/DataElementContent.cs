using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    /// <remarks/>
    [XmlInclude(typeof(ListBox))]
    [XmlInclude(typeof(DropdownList))]
    [XmlInclude(typeof(CheckBox))]
    [XmlInclude(typeof(DecimalTextBox))]
    public abstract partial class DataElementContent: BaseDataElement
    {
        /// <remarks/>
        [XmlText()]
        public string Value { get; set; }
    }
}