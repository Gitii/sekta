using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class StringElement : BaseDataElement
    {
        [XmlText]
        public string Value { get; set; }
    }
}