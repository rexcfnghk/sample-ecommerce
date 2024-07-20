using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace SampleECommerce.Tests.Attributes;

public sealed class AutoNSubstituteDataAttribute() : AutoDataAttribute(
    () => new Fixture()
        .Customize(new AutoNSubstituteCustomization()));