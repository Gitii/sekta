using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class PolicyPresentation
    {
        [XmlElement("checkBox", typeof(CheckBox))]
        [XmlElement("comboBox", typeof(ComboBox))]
        [XmlElement("decimalTextBox", typeof(DecimalTextBox))]
        [XmlElement("dropdownList", typeof(DropdownList))]
        [XmlElement("listBox", typeof(ListBox))]
        [XmlElement("text", typeof(StringElement))]
        [XmlElement("textBox", typeof(TextBox))]
        public BaseDataElement[] Items { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}