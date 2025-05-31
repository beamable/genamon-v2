using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Server.Api.Content;
using Beamable.Web3SolanaFederation.Extensions;
using Beamable.Web3SolanaFederation.Features.Inventory.Models;
using Beamable.Web3SolanaFederation.Features.Rpc;
using Solnet.Wallet;

namespace Beamable.Web3SolanaFederation.Features.Inventory;

public class InventoryService : IService
{
    private readonly SolanaRpc _solanaRpc;
    private readonly IMicroserviceContentApi _contentService;
    private readonly QuickNodeIndexerService _quickNodeIndexerService;

    public InventoryService(SolanaRpc solanaRpc, IMicroserviceContentApi contentService, QuickNodeIndexerService quickNodeIndexerService)
    {
        _solanaRpc = solanaRpc;
        _contentService = contentService;
        _quickNodeIndexerService = quickNodeIndexerService;
    }

    public async Task<FederatedInventoryProxyState> GetInventoryState(string id)
    {
        var tokenAccounts = await _solanaRpc.GetTokenAccountsByOwnerAsync(id);
        var playerTokens = tokenAccounts
            .Where(x => x.Account.Data.Parsed.Info.TokenAmount.Decimals == 0 && x.Account.Data.Parsed.Info.TokenAmount.AmountUlong > 0)
            .Select(x => new PlayerTokenInfo
            {
                TokenAccount = new PublicKey(x.PublicKey),
                Mint = new PublicKey(x.Account.Data.Parsed.Info.Mint),
                Amount = x.Account.Data.Parsed.Info.TokenAmount.AmountUlong
            })
            .ToList();

        var solanaContent = await _contentService.GetManifest(new ContentQuery
        {
            TypeConstraints = new HashSet<Type>
            {
                typeof(ItemContent)
            }
        });

        var solanaCollections = new HashSet<CollectionItem>();
        foreach (var entry in solanaContent.entries)
        {
            var contentRef = await entry.Resolve();
            var collectionName = "beamable";
            if (string.IsNullOrWhiteSpace(collectionName)) continue;
            solanaCollections.Add(new CollectionItem(contentRef.Id, collectionName));
        }

        var state = new PlayerTokenState(playerTokens);
        foreach (var token in playerTokens)
        {
            var metadataAccount = await _solanaRpc.FetchMetadataAccount(token.Mint);
            if (metadataAccount?.offchainData?.collection != null)
            {
                var targetCollection = solanaCollections.FirstOrDefault(x => string.Equals(x.CollectionName,
                    metadataAccount.offchainData.collection.name, StringComparison.OrdinalIgnoreCase));
                if (targetCollection is null) continue;
                token.ContentId = targetCollection.ContentId;
                token.Properties = metadataAccount.offchainData.ToProperties();
            }
        }
        return state.ToProxyState();
    }
}