using System.Collections;
using System.Collections.Generic;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using Web3FederationCommon;

public class SolanaAuth : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var ctx = await BeamContext.Default.Instance;
        await ctx.Accounts.Refresh();
        await ctx.Accounts.OnReady;

        if (ctx.Accounts.Current.TryGetExternalIdentity<Web3SolanaIdentity, Web3SolanaFederationClient>(out var externalIdentity))
        {
            Debug.Log($"Solana Wallet Address: {externalIdentity.userId}");
        }
        else
        {
            Debug.Log("No Solana Wallet Detected. Creating one...");
            var result = await ctx.Accounts.Current.AddExternalIdentity<Web3SolanaIdentity, Web3SolanaFederationClient>("", challenge => "");
            if (result.isSuccess)
            {
                if (ctx.Accounts.Current.TryGetExternalIdentity<Web3SolanaIdentity, Web3SolanaFederationClient>(
                        out var createdWallet))
                {
                    Debug.Log($"Created Custodial Wallet: {createdWallet.userId}");
                    return;
                }
            }

            Debug.LogError("Failed to create Solana wallet.");
        }
    }
}
