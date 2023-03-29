using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sekta.Admx.Schema;

namespace Sekta.Core.Schema;

public class AdmxPolicyDefinitions
{
    private readonly List<AdmxCategory> _categories;
    private readonly List<AdmxPolicy> _policies;

    public List<AdmxCategory> Categories => _categories;

    public AdmxPolicyDefinitions()
        : this(new List<AdmxCategory>(), new List<AdmxPolicy>()) { }

    private AdmxPolicyDefinitions(List<AdmxCategory> categories, List<AdmxPolicy> policies)
    {
        _categories = categories;
        _policies = policies;
    }

    public static AdmxPolicyDefinitions From(Stream admxFileStream)
    {
        PolicyDefinitions definitions = PolicyDefinitions.Deserialize(admxFileStream);

        return From(definitions);
    }

    public static AdmxPolicyDefinitions From(Sekta.Admx.Schema.PolicyDefinitions rawDefinitions)
    {
        List<AdmxCategory> categories = AdmxCategory.From(rawDefinitions.Categories);
        List<AdmxCategory> allCategories = categories.FlattenCategories().ToList();
        List<AdmxPolicy> policies = AdmxPolicy.From(rawDefinitions.Policies, allCategories);

        return new AdmxPolicyDefinitions(categories, policies);
    }
}
