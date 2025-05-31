using System.Net;
using Beamable.Server;

namespace Beamable.Web3SolanaFederation.Features.Rpc.Exceptions;

internal class RpcCallException : MicroserviceException
{
    public RpcCallException(string message) : base((int)HttpStatusCode.BadRequest, "RpcCallError", message)
    {
    }
}