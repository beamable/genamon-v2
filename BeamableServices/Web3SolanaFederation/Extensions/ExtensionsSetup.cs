using Beamable.Server;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class ExtensionsSetup
{
    public static void SetupExtensions(this IServiceInitializer initializer)
    {
        initializer.SetupMongoExtensions();
    }
}