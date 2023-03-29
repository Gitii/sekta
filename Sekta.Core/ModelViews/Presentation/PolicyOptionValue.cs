using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sekta.Admx.Schema;

namespace Sekta.Core.ModelView.Presentation;

public readonly struct PolicyOptionValue : IEquatable<PolicyOptionValue>
{
    private enum ValueType
    {
        String,
        Uint,
        KeyValueList,
    }

    public readonly string Id;
    public readonly string Path;
    public readonly string KeyName;
    public readonly uint? KeyValueUSignedInteger;
    public readonly string KeyValueString;
    public readonly KeyValuePair<string, string>[] KeyValueList;

    private readonly ValueType Type;

    private PolicyOptionValue(
        string path,
        string keyName,
        uint? keyValueUSignedInteger,
        string keyValueString,
        KeyValuePair<string, string>[] keyValueList,
        string id
    )
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        KeyName = keyName;
        if (keyName == null && keyValueList == null)
        {
            throw new ArgumentNullException(
                nameof(keyName),
                "KeyName can only be null if a key value list is used."
            );
        }

        KeyValueUSignedInteger = keyValueUSignedInteger;
        KeyValueString = keyValueString;
        KeyValueList = keyValueList;
        Id = id;

        if (keyValueUSignedInteger.HasValue)
        {
            Type = ValueType.Uint;
        }
        else if (keyValueString != null)
        {
            Type = ValueType.String;
        }
        else if (keyValueList != null)
        {
            Type = ValueType.KeyValueList;
        }
        else
        {
            throw new ArgumentException("Failed to determine type!");
        }
    }

    public PolicyOptionValue(string path, string keyName, uint keyValueUSignedInteger, string id)
        : this(path, keyName, keyValueUSignedInteger, null, null, id) { }

    public PolicyOptionValue(string path, string keyName, string keyValueString, string id)
        : this(path, keyName, null, keyValueString, null, id)
    {
        if (keyValueString == null)
        {
            throw new ArgumentNullException(nameof(keyValueString));
        }
    }

    public PolicyOptionValue(
        string path,
        string keyValuePrefix,
        KeyValuePair<string, string>[] keyValueList,
        string id
    )
        : this(path, keyValuePrefix, null, null, keyValueList, id)
    {
        if (keyValueList == null)
        {
            throw new ArgumentNullException(nameof(keyValueList));
        }
    }

    public bool Equals(PolicyOptionValue other)
    {
        return Path == other.Path
            && KeyName == other.KeyName
            && KeyValueUSignedInteger == other.KeyValueUSignedInteger
            && KeyValueString == other.KeyValueString
            && Equals(KeyValueList, other.KeyValueList);
    }

    public override bool Equals(object obj)
    {
        return obj is PolicyOptionValue other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (Path != null ? Path.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (KeyName != null ? KeyName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ KeyValueUSignedInteger.GetHashCode();
            hashCode =
                (hashCode * 397) ^ (KeyValueString != null ? KeyValueString.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (KeyValueList != null ? KeyValueList.GetHashCode() : 0);
            return hashCode;
        }
    }

    public bool Equals(uint uintValue)
    {
        return KeyValueUSignedInteger.HasValue && KeyValueUSignedInteger == uintValue;
    }

    public bool Equals(string strValue)
    {
        return KeyValueString != null && KeyValueString == strValue;
    }

    public bool Equals(string[] strArrayValue)
    {
        return KeyValueList != null
            && strArrayValue != null
            && KeyValueList.Length == strArrayValue.Length
            && strArrayValue.SequenceEqual(strArrayValue);
    }

    public static bool operator ==(PolicyOptionValue left, PolicyOptionValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PolicyOptionValue left, PolicyOptionValue right)
    {
        return !left.Equals(right);
    }

    public uint ToUIntValue()
    {
        if (KeyValueUSignedInteger.HasValue)
        {
            return KeyValueUSignedInteger.Value;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public string ToStringValue()
    {
        if (KeyValueString != null)
        {
            return KeyValueString;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public KeyValuePair<string, string>[] ToStringListValue()
    {
        if (KeyValueList != null)
        {
            return KeyValueList;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public bool HasSameValue(ValueContainer valueContainer)
    {
        if (valueContainer is null)
        {
            throw new ArgumentNullException(nameof(valueContainer));
        }

        switch (valueContainer.Item)
        {
            case ValueString str:
                return ToStringValue() == str.Value;
            case ValueDecimal dcm:
                return ToUIntValue() == dcm.Value;
            default:
                throw new NotSupportedException(valueContainer.GetType().FullName);
        }
    }

    public string Serialize()
    {
        var container = new SerializedValueContainer();
        if (KeyValueString != null)
        {
            container.SerializedValue = KeyValueString;
        }
        else if (KeyValueUSignedInteger.HasValue)
        {
            container.SerializedValue = KeyValueUSignedInteger.ToString();
        }
        else if (KeyValueList != null)
        {
            container.SerializedValue = JsonConvert.SerializeObject(KeyValueList);
        }
        else
        {
            throw new ArgumentException();
        }

        container.SerializedValueType = Type;
        container.KeyName = KeyName;
        container.Path = Path;
        container.Id = Id;

        return JsonConvert.SerializeObject(container);
    }

    public static PolicyOptionValue Deserialize(string serializedValue)
    {
        SerializedValueContainer container =
            JsonConvert.DeserializeObject<SerializedValueContainer>(serializedValue);

        switch (container.SerializedValueType)
        {
            case ValueType.String:
                string strValue = container.SerializedValue;
                return new PolicyOptionValue(
                    container.Path,
                    container.KeyName,
                    strValue,
                    container.Id
                );
            case ValueType.Uint:
                uint uintValue = uint.Parse(container.SerializedValue);
                return new PolicyOptionValue(
                    container.Path,
                    container.KeyName,
                    uintValue,
                    container.Id
                );
            case ValueType.KeyValueList:
                KeyValuePair<string, string>[] list = JsonConvert.DeserializeObject<KeyValuePair<
                    string,
                    string
                >[]>(container.SerializedValue);
                return new PolicyOptionValue(container.Path, container.KeyName, list, container.Id);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private struct SerializedValueContainer
    {
        public string SerializedValue;
        public ValueType SerializedValueType;
        public string KeyName;
        public string Path;
        public string Id;
    }
}
