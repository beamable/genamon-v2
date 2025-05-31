#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Server;
using Beamable.Server.Api.RealmConfig;

namespace Beamable.Web3SolanaFederation;

public class Configuration : IService
{
    private const string ConfigurationNamespace = "web3_solana_federation";
    private readonly IMicroserviceRealmConfigService _realmConfigService;

    public readonly string RealmSecret = Environment.GetEnvironmentVariable("SECRET") ?? "";
    private RealmConfig? _realmConfig;

    public Configuration(IMicroserviceRealmConfigService realmConfigService)
    {
        _realmConfigService = realmConfigService;
    }

    public ValueTask<string> RPCEndpoint => GetValue(nameof(RPCEndpoint),"");
    public ValueTask<int> AuthenticationChallengeTtlSec => GetValue(nameof(AuthenticationChallengeTtlSec), 600);
    public ValueTask<bool> AllowManagedAccounts => GetValue(nameof(AllowManagedAccounts), false);
    public ValueTask<string> ProgramId => GetValue(nameof(ProgramId), "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");

    private async ValueTask<T> GetValue<T>(string key, T defaultValue) where T : IConvertible
    {
        _realmConfig ??= await _realmConfigService.GetRealmConfigSettings();

        var namespaceConfig = _realmConfig!.GetValueOrDefault(ConfigurationNamespace) ?? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        var value = namespaceConfig.GetValueOrDefault(key);
        if (value is null)
        {
            return defaultValue;
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
}

class ConfigurationException : MicroserviceException
{
    public ConfigurationException(string message) : base((int)HttpStatusCode.BadRequest, "ConfigurationError", message)
    {
    }
}