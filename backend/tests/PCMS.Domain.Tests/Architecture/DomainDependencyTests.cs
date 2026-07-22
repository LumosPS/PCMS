using FluentAssertions;

namespace PCMS.Domain.Tests.Architecture;

public class DomainDependencyTests
{
    [Fact]
    public void Domain_must_not_depend_on_any_other_pcms_layer()
    {
        var pcmsReferences = typeof(DomainAssemblyMarker).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null && name.StartsWith("PCMS."));

        pcmsReferences.Should().BeEmpty(
            "the Domain layer must remain pure and have no dependencies on other PCMS layers");
    }
}
