using System.Collections.Generic;
using System.Linq;
using Sekta.Admx.Schema;

namespace Sekta.Core.Schema;

public class AdmxCategory
{
    private readonly Category _rawCategory;
    private readonly AdmxCategory _parent;
    private readonly List<AdmxPolicy> _policies;
    private readonly List<AdmxCategory> _children;

    public List<AdmxCategory> Children => _children;
    public List<AdmxPolicy> Policies => _policies;

    public string Name => _rawCategory.name;
    public string DisplayName => _rawCategory.displayName;

    /// <summary>
    /// Marks this category as stub.
    /// "Stubs" are used to mark categories which are auto created because the definition is missing in the admx file.
    /// </summary>
    public bool IsStub { get; set; } = false;

    public string[] PathElements =>
        GetCategoryPathElements().SelectMany((c) => new string[] { "/", c.DisplayName }).ToArray();

    public AdmxCategory[] GetCategoryPathElements()
    {
        if (_parent == null)
        {
            return IsStub ? new AdmxCategory[0] : new[] { this };
        }
        else if (IsStub)
        {
            // this element is a stub, ignore it
            return _parent.GetCategoryPathElements();
        }
        else
        {
            return _parent.GetCategoryPathElements().Concat(new[] { this }).ToArray();
        }
    }

    public AdmxCategory(Category rawCategory, AdmxCategory parent)
    {
        this._rawCategory = rawCategory;
        _parent = parent;
        _policies = new List<AdmxPolicy>();
        _children = new List<AdmxCategory>();
    }

    public static List<AdmxCategory> From(List<Category> rawCategoryList)
    {
        List<AdmxCategory> rootCategories = new List<AdmxCategory>(
            rawCategoryList
                .Where((rc) => rc.ParentCategory == null)
                .Select((rc) => new AdmxCategory(rc, null))
        );

        List<string> knownCategoryNames = rawCategoryList.Select((c) => c.name).ToList();

        List<string> missingCategoryNames = rawCategoryList
            .Where((c) => c.ParentCategory != null)
            .Select((c) => c.ParentCategory.ReferenceName)
            .Except(knownCategoryNames)
            .ToList();

        foreach (string missingCategoryName in missingCategoryNames)
        {
            rootCategories.Add(
                new AdmxCategory(
                    new Category()
                    {
                        displayName = "MISSING CAT. " + missingCategoryName,
                        explainText = "This category has been referenced but never defined.",
                        name = missingCategoryName
                    },
                    null
                )
                {
                    IsStub = true,
                }
            );
        }

        List<Category> unorderedCategories = new List<Category>(
            rawCategoryList.Where((rc) => rc.ParentCategory != null)
        );
        while (unorderedCategories.Count > 0)
        {
            var matchedCategories = unorderedCategories
                .Where(
                    (rc) =>
                    {
                        var parent = FindParentInTrees(rc, rootCategories);
                        if (parent != null)
                        {
                            parent.Children.Add(new AdmxCategory(rc, parent));
                            return true;
                        }

                        return false;
                    }
                )
                .ToList();

            foreach (Category matchedCategory in matchedCategories)
            {
                unorderedCategories.Remove(matchedCategory);
            }
        }

        return rootCategories;
    }

    private static AdmxCategory FindParentInTrees(
        Category category,
        IList<AdmxCategory> rootCategories
    )
    {
        if (category.ParentCategory == null)
        {
            return null;
        }

        foreach (AdmxCategory otherCategory in rootCategories)
        {
            if (category.ParentCategory.ReferenceName == otherCategory.Name)
            {
                return otherCategory;
            }

            var parent = FindParentInTrees(category, otherCategory.Children);
            if (parent != null)
            {
                return parent;
            }
        }

        return null;
    }
}
