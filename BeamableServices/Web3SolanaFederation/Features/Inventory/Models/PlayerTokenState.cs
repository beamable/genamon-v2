using System.Collections.Generic;
using System.Linq;
using Beamable.Common;
using Beamable.Common.Api.Inventory;

namespace Beamable.Web3SolanaFederation.Features.Inventory.Models;

public class PlayerTokenState
{
    private readonly List<PlayerTokenInfo> _tokens;

    public PlayerTokenState(List<PlayerTokenInfo> tokens)
    {
        _tokens = tokens;
    }

    public FederatedInventoryProxyState ToProxyState()
    {
        var items = _tokens.Where(x => x.Amount > 0 && !string.IsNullOrWhiteSpace(x.ContentId))
            .GroupBy(x => x.ContentId)
            .ToList();

        return new FederatedInventoryProxyState
        {
            currencies = new Dictionary<string, long>(),
            items = items.ToDictionary(x => x.Key, x => x.Select(gv => new FederatedItemProxy
            {
                proxyId = gv.Mint.Key,
                properties = gv.Properties.Select(p => new ItemProperty
                {
                    name = p.Key,
                    value = p.Value
                }).ToList()
            }).ToList())
        };
    }
}