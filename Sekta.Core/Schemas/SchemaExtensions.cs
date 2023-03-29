using System.Collections.Generic;

namespace Sekta.Core.Schema;

public static class SchemaExtensions
{
    public static IEnumerable<AdmxCategory> FlattenCategories(this IEnumerable<AdmxCategory> cats)
    {
        foreach (AdmxCategory category in cats)
        {
            yield return category;
            foreach (AdmxCategory child in FlattenCategories(category.Children))
            {
                yield return child;
            }
        }
    }
}
