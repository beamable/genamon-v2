using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Microservices.Pokemon.Storage.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Beamable.Microservices.Pokemon.Storage
{
	public static class PendingPokemonCollection
	{
		private static IMongoCollection<PendingPokemon> _collection;
		private static readonly string _collectionName = "pending_pokemon";
		private static readonly IEnumerable<CreateIndexModel<PendingPokemon>> _indexes = new CreateIndexModel<PendingPokemon>[]
		{
			new CreateIndexModel<PendingPokemon>(
				Builders<PendingPokemon>.IndexKeys.Ascending(x => x.ExpiresAt),
				new CreateIndexOptions { ExpireAfter = System.TimeSpan.FromSeconds(1) }
			)
		};

		public static async ValueTask<IMongoCollection<PendingPokemon>> Get(IMongoDatabase db)
		{
			if (_collection is null)
			{
				_collection = db.GetCollection<PendingPokemon>(_collectionName);
				await _collection.Indexes.CreateManyAsync(_indexes);
			}

			return _collection;
		}
		
		public static async Task<List<PendingPokemon>> GetById(IMongoDatabase db, string genamonId)
		{
			var collection = await Get(db);
			var query = Builders<PendingPokemon>.Filter.Eq(
			document => document.Id, ObjectId.Parse(genamonId)
			);
			return await collection.Find(query).ToListAsync();
		}

		public static async Task<bool> DeleteById(IMongoDatabase db, string genamonId)
		{
			var collection = await Get(db);
			var query = Builders<PendingPokemon>.Filter.Eq(
				document => document.Id, ObjectId.Parse(genamonId)
			);
			var result = await collection.DeleteOneAsync(query);

			return result.IsAcknowledged;
		}

		public static async Task<List<PendingPokemon>> GetAvailablePokemon(IMongoDatabase db)
		{
			var collection = await Get(db);
			return await collection
				.Find(_ => true)
				.ToListAsync();
		}

		public static async Task<bool> TryInsert(IMongoDatabase db, IEnumerable<PendingPokemon> pokemon)
		{
			var collection = await Get(db);
			try
			{
				await collection.InsertManyAsync(pokemon);
				return true;
			}
			catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
			{
				return false;
			}
		}
	}
}