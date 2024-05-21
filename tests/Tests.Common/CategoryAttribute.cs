namespace TaskTitan.Tests.Common;

public class CategoryAttribute : Attribute, ITraitAttribute
{
    public CategoryAttribute(TestCategory category)
    {
    }
}
