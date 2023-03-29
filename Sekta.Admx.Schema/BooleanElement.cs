using System;
using System.Xml.Serialization;
#pragma warning disable CA1823

namespace Sekta.Admx.Schema;

public partial class BooleanElement : BaseElement
{
    private string idField;

    private string clientExtensionField;

    private string keyField;

    private string valueNameField;

    /// <remarks/>
    public ValueContainer trueValue { get; set; }

    /// <remarks/>
    public ValueContainer falseValue { get; set; }

    /// <remarks/>
    public ValueList trueList { get; set; }

    /// <remarks/>
    public ValueList falseList { get; set; }
}
