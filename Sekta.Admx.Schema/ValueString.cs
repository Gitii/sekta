using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class ValueString : ValueBase, IEquatable<ValueString>
{
    public bool Equals(ValueString other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((ValueString)obj);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 1;
    }

    public static bool operator ==(ValueString left, ValueString right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueString left, ValueString right)
    {
        return !Equals(left, right);
    }

    /// <remarks/>
    [XmlText()]
    public string ValueInBody { get; set; }

    [XmlAttribute("value")]
    public string ValueInAttribute { get; set; }

    [XmlIgnore]
    public string Value => ValueInAttribute ?? ValueInBody;
}
