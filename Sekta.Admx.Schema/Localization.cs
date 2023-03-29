using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class Localization
{
    [XmlArray("stringTable")]
    [XmlArrayItem("string", typeof(LocalizedString))]
    public List<LocalizedString> StringTable { get; set; }

    [XmlArray("presentationTable")]
    [XmlArrayItem("presentation", typeof(PolicyPresentation))]
    public List<PolicyPresentation> PresentationTable { get; set; }
}
