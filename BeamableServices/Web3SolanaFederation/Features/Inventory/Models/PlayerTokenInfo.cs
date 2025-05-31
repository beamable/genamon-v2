using System.Collections.Generic;
using Solnet.Wallet;

namespace Beamable.Web3SolanaFederation.Features.Inventory.Models;

public class PlayerTokenInfo
{
    public PublicKey TokenAccount { get; set; }
    public PublicKey Mint { get; set; }
    public string ContentId { get; set; }
    public ulong Amount { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
}