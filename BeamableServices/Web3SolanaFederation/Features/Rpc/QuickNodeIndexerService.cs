using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Beamable.Web3SolanaFederation.Features.Rpc.Models;

namespace Beamable.Web3SolanaFederation.Features.Rpc;

public class QuickNodeIndexerService : IService
{
    private const int PageSize = 40;
    private readonly Configuration _configuration;
    private readonly HttpClient _httpClient = new();

    public QuickNodeIndexerService(Configuration configuration)
    {
        _configuration = configuration;
    }

    public async Task FetchAssets(string walletAddress)
    {
        await FetchNfts(walletAddress);

    }

    private async Task FetchNfts(string wallet)
    {
        await FetchNFTPage(wallet, 1,new HashSet<string>());
    }

    private async Task FetchNFTPage(string wallet, int page, HashSet<string> contracts)
    {
        var requestData = new
        {
            id = 67,
            jsonrpc = "2.0",
            method = "qn_fetchNFTs",
            @params = new[]
            {
                new
                {
                    wallet,
                    omitFields = new[] { "traits" },
                    page,
                    perPage = PageSize,
                    contracts
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(await _configuration.RPCEndpoint, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
        }

        var responseStream = await response.Content.ReadAsStreamAsync();
        var jsonResponse = await JsonSerializer.DeserializeAsync<NFTResponse>(responseStream);
        var result = jsonResponse!.Result;

        var i = 0;
    }

}