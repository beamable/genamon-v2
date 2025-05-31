using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Beamable.Web3SolanaFederation.Features.Accounts.Storage.Models;

public record Vault
{
    [BsonElement("_id")]
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public KeyStore KeyStore { get; set; } = null!;
    public DateTime Created { get; set; } = DateTime.Now;
}

public record Crypto
{
    [JsonPropertyName("cipher")]
    public string Cipher { get; set; }

    [JsonPropertyName("ciphertext")]
    public string Ciphertext { get; set; }

    [JsonPropertyName("cipherparams")]
    public CipherParams Cipherparams { get; set; }

    [JsonPropertyName("kdf")]
    public string Kdf { get; set; }

    [JsonPropertyName("mac")]
    public string Mac { get; set; }

    [JsonPropertyName("kdfparams")]
    public KdfParams Kdfparams { get; set; }
}

public record CipherParams
{
    [JsonPropertyName("iv")]
    public string Iv { get; set; }
}

public record KdfParams
{
    [JsonPropertyName("n")]
    public int N { get; set; }

    [JsonPropertyName("r")]
    public int R { get; set; }

    [JsonPropertyName("p")]
    public int P { get; set; }

    [JsonPropertyName("dklen")]
    public int Dklen { get; set; }

    [JsonPropertyName("salt")]
    public string Salt { get; set; }
}

public record KeyStore
{
    [JsonPropertyName("crypto")]
    public Crypto Crypto { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }
}