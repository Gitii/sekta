using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sekta.Admx.Schema;

namespace Sekta.Core.Schema
{
    public class AdmxPolicy
    {
        private readonly PolicyDefinition _rawPolicyDefinition;
        private readonly AdmxCategory _category;
        
        public string ValueName => _rawPolicyDefinition.valueName;
        public string Key => _rawPolicyDefinition.key;
        public string DisplayName => _rawPolicyDefinition.displayName;
        public string ExplainText => _rawPolicyDefinition.explainText;
        public string Presentation => _rawPolicyDefinition.presentation;
        public string Name => _rawPolicyDefinition.name;
        public string ClassName => _rawPolicyDefinition.@class.ToString();
        public PolicyClass Class => _rawPolicyDefinition.@class;

        public ValueContainer EnabledValue => _rawPolicyDefinition.enabledValue;
        public ValueContainer DisabledValue => _rawPolicyDefinition.disabledValue;

        public ValueList EnabledList => _rawPolicyDefinition.enabledList;
        public ValueList DisabledList => _rawPolicyDefinition.disabledList;

        private AdmxPolicy(PolicyDefinition rawPolicyDefinition, AdmxCategory category)
        {
            _rawPolicyDefinition = rawPolicyDefinition;
            _category = category;
        }

        public AdmxCategory Category => _category;
        public BaseElement[] Elements => _rawPolicyDefinition.Elements.ToArray();

        public static List<AdmxPolicy> From(List<PolicyDefinition> rawPolicyList, List<AdmxCategory> categories)
        {
            return rawPolicyList.Select((rp) =>
            {
                AdmxCategory category = categories.First((c) => c.Name == rp.ParentCategory.ReferenceName);
                var policy = new AdmxPolicy(rp, category);
                category.Policies.Add(policy);

                return policy;
            }).ToList();
        }

        public const string POLICY_EMBEDDED_OPTION_ID = "POLICY_EMBEDDED_OPTION_ID";
    }
}
