using System;
using System.Collections.Generic;
using Beamable.Common;
using Solnet.Metaplex.Utilities.Json;

namespace Beamable.Web3SolanaFederation.Extensions;

public static class MetaplexExtensions
{
    public static Dictionary<string, string> ToProperties(this MetaplexTokenStandard offChainData)
    {
        var properties = new Dictionary<string, string>();
        try
        {
            properties.TryAdd("name", offChainData.name);
            properties.TryAdd("description", offChainData.description);
            properties.TryAdd("image", offChainData.default_image);
            properties.TryAdd("external_url", offChainData.external_url);
            foreach (var attribute in offChainData.attributes)
                properties.TryAdd(attribute.trait_type, attribute.value);
        }
        catch (Exception)
        {
            BeamableLogger.LogError("Error fetching external metadata for {name}", offChainData.name);
            return properties;
        }
        return properties;
    }
}