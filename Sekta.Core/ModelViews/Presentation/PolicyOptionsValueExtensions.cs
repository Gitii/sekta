using System;
using Sekta.Admx.Schema;

namespace Sekta.Core.ModelView.Presentation;

public static class PolicyOptionsValueExtensions
{
    public static PolicyOptionValue AsPolicyOption(
        this ValueContainer valueContainer,
        string path,
        string keyName,
        string id
    )
    {
        if (valueContainer is null)
        {
            throw new ArgumentNullException(nameof(valueContainer));
        }

        switch (valueContainer.Item)
        {
            case ValueString str:
                return new PolicyOptionValue(path, keyName, str.Value, id);
            case ValueDecimal dcm:
                return new PolicyOptionValue(path, keyName, dcm.Value, id);
            default:
                throw new NotSupportedException(valueContainer.GetType().FullName);
        }
    }
}