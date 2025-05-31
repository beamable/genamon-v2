using System.Linq;
using System.Reflection;
using Beamable.Common.Dependencies;
using Beamable.Web3SolanaFederation.Extensions;

namespace Beamable.Web3SolanaFederation;

public static class ServiceRegistration
{
    public static void AddFeatures(this IDependencyBuilder builder)
    {
        Assembly.GetExecutingAssembly()
            .GetDerivedTypes<IService>()
            .ToList()
            .ForEach(serviceType => builder.AddSingleton(serviceType));

        //builder.AddScoped<TransactionManager>();
    }
}