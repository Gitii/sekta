using System;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class ValueDelete : ValueBase, IEquatable<ValueDelete>
{
    public bool Equals(ValueDelete other)
    {
        return true;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return true;
    }

    public override int GetHashCode()
    {
        return 1;
    }

    public static bool operator ==(ValueDelete left, ValueDelete right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueDelete left, ValueDelete right)
    {
        return !Equals(left, right);
    }
}
