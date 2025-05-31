using Beamable.Common.Dependencies;

namespace Beamable.Web3SolanaFederation.Endpoints;

public static class ServiceRegistration
{
    public static void AddEndpoints(this IDependencyBuilder builder)
    {
        builder.AddScoped<AuthenticateEndpoint>();
        builder.AddScoped<GetInventoryStateEndpoint>();
    }
}