using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class ValueDecimal : ValueBase, IEquatable<ValueDecimal>
{
    public bool Equals(ValueDecimal other)
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
        return Equals((ValueDecimal)obj);
    }

    public override int GetHashCode()
    {
        return (int)Value;
    }

    public static bool operator ==(ValueDecimal left, ValueDecimal right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueDecimal left, ValueDecimal right)
    {
        return !Equals(left, right);
    }

    /// <remarks/>
    [XmlText]
    public string RawValueInBody { get; set; }

    [XmlAttribute("value")]
    public string RawValueInAttribute { get; set; }

    [XmlIgnore]
    public uint Value => uint.Parse(RawValueInAttribute ?? RawValueInBody);
}
