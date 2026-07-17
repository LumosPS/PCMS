using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PCMS.Application;

namespace PCMS.Application.Tests;

public class DependencyInjectionSmokeTests
{
    [Fact]
    public void Mediatr_and_validators_register_from_the_application_assembly()
    {
        var services = new ServiceCollection();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ISender>().Should().NotBeNull();
    }
}
