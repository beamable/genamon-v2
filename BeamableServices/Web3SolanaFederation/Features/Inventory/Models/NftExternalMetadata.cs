using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Beamable.Web3SolanaFederation.Features.Inventory.Models;

public record NftExternalMetadata
{
    [JsonExtensionData]
    public Dictionary<string, object> SpecialProperties { get; init; } = new();

    [JsonPropertyName("attributes")]
    public IList<MetadataAttribute> Attributes { get; init; } = new List<MetadataAttribute>();

    public Dictionary<string, string> GetProperties()
    {
        var properties = new Dictionary<string, string>();

        foreach (var attribute in SpecialProperties)
        {
            if (AdditionalProperties.Contains(attribute.Key))
                properties.TryAdd(attribute.Key, attribute.Value.ToString());
        }
        foreach (var attribute in Attributes)
        {
            if (attribute.value is not null)
                properties.TryAdd(attribute.trait_type, attribute.value.ToString() ?? "");
        }
        return properties;
    }

    private HashSet<string> AdditionalProperties = new()
    {
        "name", "image", "description", "external_url"
    };

}

public record MetadataAttribute(string trait_type, object value);