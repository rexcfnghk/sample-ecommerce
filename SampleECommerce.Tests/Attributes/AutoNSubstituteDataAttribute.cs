using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SampleECommerce.Tests.Attributes;

public sealed class AutoNSubstituteDataAttribute() : AutoDataAttribute(
    () => new Fixture()
        .Customize(new ActionContextCustomization())
        .Customize(new AutoNSubstituteCustomization()));

public sealed class ActionContextCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => new ActionContext());
        fixture.Customize<BindingInfo>(i => i.OmitAutoProperties());
    }
}