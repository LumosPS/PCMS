using FluentAssertions;
using PCMS.Application;

namespace PCMS.Application.Tests.Architecture;

public class ApplicationDependencyTests
{
    [Fact]
    public void Application_must_not_depend_on_infrastructure_or_api()
    {
        var pcmsReferences = typeof(ApplicationAssemblyMarker).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null && name.StartsWith("PCMS."));

        pcmsReferences.Should().NotContain(
            name => name == "PCMS.Infrastructure" || name == "PCMS.API",
            "the Application layer may only depend on the Domain layer");
    }
}
