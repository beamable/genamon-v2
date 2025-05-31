using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Web3SolanaFederation.Features.Inventory.Models;
using Beamable.Web3SolanaFederation.Features.Rpc.Exceptions;
using Solnet.Metaplex.NFT.Library;
using Solnet.Rpc;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;

namespace Beamable.Web3SolanaFederation.Features.Rpc;

public class SolanaRpc : IService
{
    private readonly Configuration _configuration;
    private readonly Lazy<Task<IRpcClient>> _client;
    private static readonly HttpClient HttpClient = new();

    public SolanaRpc(Configuration configuration)
    {
        _configuration = configuration;
        _client = new Lazy<Task<IRpcClient>>(InitializeClientAsync);
    }

    private async Task<IRpcClient> InitializeClientAsync()
    {
        var rpcEndpoint = await _configuration.RPCEndpoint;
        return ClientFactory.GetClient(rpcEndpoint);
    }

    private async Task<IRpcClient> GetClientAsync()
    {
        return await _client.Value;
    }

    public async Task<List<TokenAccount>> GetTokenAccountsByOwnerAsync(string owner, string tokenProgram = null)
    {
        var operation = "GetTokenAccountsByOwner";
        using (new Measure(operation))
        {
            try
            {
                var client = await GetClientAsync();
                var contract = tokenProgram ?? await _configuration.ProgramId;
                var result = await RunWithRetry(_ => client.GetTokenAccountsByOwnerAsync(owner, tokenProgramId: contract), operation);
                if (!result.WasSuccessful)
                    throw new RpcCallException($"Error calling {operation} for user {owner} for {result.Reason}.");
                if (result.Result.Value is null)
                    throw new RpcCallException($"Error calling {operation} for user {owner} for results is empty.");
                return result.Result.Value;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Error fetching token accounts for {key}", owner);
                BeamableLogger.LogError(ex);
                return Enumerable.Empty<TokenAccount>().ToList();
            }
        }
    }

    public async Task<MetadataAccount?> FetchMetadataAccount(PublicKey publicKey)
    {
        var operation = "FetchMetadataAccount";
        using (new Measure(operation))
        {
            try
            {
                var client = await GetClientAsync();
                var result = await RunWithRetry(_ => MetadataAccount.GetAccount(client, publicKey), operation);
                return result;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Error fetching metadata account for {key}", publicKey.Key);
                BeamableLogger.LogError(ex);
                return null;
            }
        }
    }

    public async Task<ResponseValue<AccountInfo>> GetAccountInfo(PublicKey publicKey)
    {
        var operation = "FetchMetadataAccount";
        using (new Measure(operation))
        {
            try
            {
                var client = await GetClientAsync();
                var result = await RunWithRetry(_ => client.GetAccountInfoAsync(publicKey), operation);
                return result.Result;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Error fetching metadata account for {key}", publicKey.Key);
                BeamableLogger.LogError(ex);
                return null;
            }
        }
    }

    public async Task<NftExternalMetadata> FetchOffChainData(string uri)
    {
        var operation = "FetchOffChainData";
        using (new Measure(operation))
        {
            try
            {
                var response = await HttpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<NftExternalMetadata>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError("Error fetching external metadata at {url}", uri);
                BeamableLogger.LogError(ex);
                return new NftExternalMetadata();
            }
        }
    }

    private async Task<T> RunWithRetry<T>(Func<int, Task<T>> func, string operationName)
    {
        var maxRetries = 8;
        var retryCount = 0;
        var initialDelayMs = 300;

        while (true)
        {
            try
            {
                return await func(retryCount);
            }
            catch (Exception ex)
            {
                BeamableLogger.LogWarning("Exception calling: {op}", operationName);
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw new RpcCallException(
                        $"Maximum retry count exceeded for operation: {operationName}. ERROR: {ex.Message}");
                }
                var delayMs = (int)Math.Pow(2, retryCount - 1) * initialDelayMs;
                await Task.Delay(delayMs);
            }
        }
    }
}