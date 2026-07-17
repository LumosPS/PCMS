using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PCMS.Application;

namespace PCMS.Application.Tests;

public class DependencyInjectionSmokeTests
{
    // Validator registration is exercised here but not asserted: the Application
    // assembly has no validators yet. Extend this test when the first one exists.
    [Fact]
    public void Mediatr_registers_and_resolves_isender_from_the_application_assembly()
    {
        var services = new ServiceCollection();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ISender>().Should().NotBeNull();
    }
}
