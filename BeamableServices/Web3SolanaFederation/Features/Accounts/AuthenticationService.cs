using System;
using System.Text;
using Beamable.Common;
using Beamable.Web3SolanaFederation.Features.Accounts.Exceptions;
using Solnet.Wallet;

namespace Beamable.Web3SolanaFederation.Features.Accounts;

public class AuthenticationService : IService
{
    public bool IsSignatureValid(string address, string challenge, string signature)
    {
        try
        {
            var challengeBytes = Encoding.UTF8.GetBytes(challenge);
            var signatureBytes = Convert.FromBase64String(signature);
            return new PublicKey(address).Verify(challengeBytes, signatureBytes);
        }
        catch (Exception ex)
        {
            BeamableLogger.LogError(ex);
            throw new UnauthorizedException();
        }
    }
}