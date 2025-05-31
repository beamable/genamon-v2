using Beamable.Common;
using MongoDB.Driver;

namespace Beamable.Server
{
	public static class Web3SolanaFederationStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for Web3SolanaFederationStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> Web3SolanaFederationStorageDatabase(
			this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<Web3SolanaFederationStorage>();

		/// <summary>
		/// Gets a MongoDB collection from Web3SolanaFederationStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="Web3SolanaFederationStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> Web3SolanaFederationStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<Web3SolanaFederationStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from Web3SolanaFederationStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="Web3SolanaFederationStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> Web3SolanaFederationStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<Web3SolanaFederationStorage, TCollection>();
	}
}
