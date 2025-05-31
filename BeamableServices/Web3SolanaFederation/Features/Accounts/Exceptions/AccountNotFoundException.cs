using System.Net;
using Beamable.Server;

namespace Beamable.Web3SolanaFederation.Features.Accounts.Exceptions;

internal class AccountNotFoundException : MicroserviceException
{
    public AccountNotFoundException(long userId) : base((int)HttpStatusCode.NotFound, "AccountNotFound", $"Account not found for user {userId}")
    {
    }
}