using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Beamable.Web3SolanaFederation.Features.Rpc.Models;

public class NFTResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = null!;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("result")]
    public NFTResult Result { get; set; } = null!;

    public class NFTResult
    {
        [JsonPropertyName("owner")]
        public string Owner { get; set; } = null!;

        [JsonPropertyName("ensName")]
        public string EnsName { get; set; } = null!;

        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; } = new();

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        public class Asset
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("collectionTokenId")]
            public string CollectionTokenId { get; set; } = null!;

            [JsonPropertyName("collectionName")]
            public string CollectionName { get; set; } = null!;

            [JsonPropertyName("description")]
            public string Description { get; set; } = null!;

            [JsonPropertyName("imageUrl")]
            public string ImageUrl { get; set; } = null!;

            [JsonPropertyName("collectionAddress")]
            public string CollectionAddress { get; set; } = null!;

            [JsonPropertyName("chain")]
            public string Chain { get; set; } = null!;

            [JsonPropertyName("network")]
            public string Network { get; set; } = null!;
        }
    }
}