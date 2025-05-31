using Beamable.Server;

namespace Beamable.Server
{
	/// <summary>
	/// This class represents the existence of the Web3SolanaFederationStorage database.
	/// Use it for type safe access to the database.
	/// <code>
	/// var db = await Storage.GetDatabase&lt;Web3SolanaFederationStorage&gt;();
	/// </code>
	/// </summary>
	[StorageObject("Web3SolanaFederationStorage")]
	public class Web3SolanaFederationStorage : MongoStorageObject
	{
		
	}
}
