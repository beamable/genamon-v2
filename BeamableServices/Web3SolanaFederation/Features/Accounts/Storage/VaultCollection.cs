using System.Threading.Tasks;
using Beamable.Server;
using Beamable.Web3SolanaFederation.Features.Accounts.Storage.Models;
using MongoDB.Driver;

namespace Beamable.Web3SolanaFederation.Features.Accounts.Storage;

public class VaultCollection : IService
{
    private static readonly Collation CaseInsensitiveCollation = new("en", strength: CollationStrength.Primary);

    private readonly IStorageObjectConnectionProvider _storageObjectConnectionProvider;
    private IMongoCollection<Vault>? _collection;

    public VaultCollection(IStorageObjectConnectionProvider storageObjectConnectionProvider)
    {
        _storageObjectConnectionProvider = storageObjectConnectionProvider;
    }

    private async ValueTask<IMongoCollection<Vault>> Get()
    {
        if (_collection is null)
        {
            _collection = (await _storageObjectConnectionProvider.Web3SolanaFederationStorageDatabase()).GetCollection<Vault>("vault");
            await _collection.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Vault>(Builders<Vault>.IndexKeys.Ascending(x => x.Address), new CreateIndexOptions
                {
                    Name = "address",
                    Unique = true,
                    Collation = CaseInsensitiveCollation
                }),
            });
        }
        return _collection;
    }

    public async Task<Vault?> GetVaultByName(string name)
    {
        var collection = await Get();
        return await collection.Find(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task<string?> GetNameByAddress(string address)
    {
        var collection = await Get();
        return await collection
            .Find(x => x.Address == address, options: new FindOptions
            {
                Collation = CaseInsensitiveCollation
            })
            .Project(x => x.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetAddressByName(string name)
    {
        var collection = await Get();
        return await collection
            .Find(x => x.Name == name)
            .Project(x => x.Address)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> TryInsertVault(Vault vault)
    {
        var collection = await Get();
        try
        {
            await collection.InsertOneAsync(vault);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return false;
        }
    }
}