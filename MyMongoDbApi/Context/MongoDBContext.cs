using MyMongoDbApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MyMongoDbApi.Context;

public class MongoDBContext {
    public readonly IMongoCollection<Cliente> _comisionCollection;

    public MongoDBContext(IOptions<MongoDBSettings> mongoDBSettings){
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _comisionCollection = database.GetCollection<Cliente>(mongoDBSettings.Value.CollectionName);
    }

    public async Task CreateAsync(Cliente cliente) {
        await _comisionCollection.InsertOneAsync(cliente);
        return;
    }

    public async Task<List<Cliente>> GetAsync() {
        return await _comisionCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task AddToComisionAsync(string id, string comisionId) {
        FilterDefinition<Cliente> filter = Builders<Cliente>.Filter.Eq("Id", id);
        UpdateDefinition<Cliente> update = Builders<Cliente>.Update.AddToSet<string>("items", comisionId);
        await _comisionCollection.UpdateOneAsync(filter, update);
        return;
    }

    public async Task DeleteAsync(string id) {
        FilterDefinition<Cliente> filter = Builders<Cliente>.Filter.Eq("Id", id);
        await _comisionCollection.DeleteOneAsync(filter);
        return;
    }
}