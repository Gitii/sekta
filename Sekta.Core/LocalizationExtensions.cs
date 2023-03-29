using System.Linq;
using Sekta.Admx.Schema;

namespace Sekta.Core;

public static class LocalizationExtensions
{
    public static string LocalizeWith(this string str, PolicyDefinitionResources resources)
    {
        if (string.IsNullOrEmpty(str) || resources == null)
        {
            return str;
        }

        if (str.StartsWith("$(string.") && str.EndsWith((")")))
        {
            var parts = str.Substring(2, str.Length - 3).Split('.');
            if (parts.Length == 2)
            {
                string key = parts[1];

                return resources.Resources.StringTable.FirstOrDefault((ls) => ls.Id == key)?.Value;
            }
        }

        LocalizedString localizedString = resources.Resources.StringTable.FirstOrDefault(
            (ls) => ls.Id == str
        );
        if (localizedString != null)
        {
            return localizedString.Value;
        }

        return str;
    }

    public static PolicyPresentation GetLocalizedPresentation(
        this string str,
        PolicyDefinitionResources resources
    )
    {
        if (string.IsNullOrEmpty(str) || resources == null)
        {
            return null;
        }

        if (str.StartsWith("$(presentation.") && str.EndsWith((")")))
        {
            var parts = str.Substring(2, str.Length - 3).Split('.');
            if (parts.Length == 2)
            {
                string key = parts[1];

                return resources.Resources.PresentationTable.FirstOrDefault((ls) => ls.Id == key);
            }
        }

        return null;
    }
}
