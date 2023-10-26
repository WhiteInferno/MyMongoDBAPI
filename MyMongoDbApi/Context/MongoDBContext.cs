using MyMongoDbApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace MyMongoDbApi.Context;

public class MongoDBContext
{
    public readonly IMongoCollection<Cliente> _comisionCollection;

    public readonly IMongoDatabase database;

    public MongoDBContext(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _comisionCollection = database.GetCollection<Cliente>(mongoDBSettings.Value.CollectionName);
    }

    public async Task CreateAsync(Cliente cliente)
    {
        await _comisionCollection.InsertOneAsync(cliente);
        return;
    }

    public async Task<List<Cliente>> GetAsync()
    {
        return await _comisionCollection.Find(new BsonDocument()).ToListAsync();
    }

    public Cliente GetById(string id)
    {
        FilterDefinition<Cliente> filter = Builders<Cliente>.Filter.Eq("Id", id);
        return _comisionCollection.Find(filter).First();
    }

    public async Task AddToComisionAsync(string id, string? comisionId)
    {
        FilterDefinition<Cliente> filter = Builders<Cliente>.Filter.Eq("Id", id);
        UpdateDefinition<Cliente> update = Builders<Cliente>.Update.AddToSet<string>("items", comisionId);
        await _comisionCollection.UpdateOneAsync(filter, update);
        return;
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<Cliente> filter = Builders<Cliente>.Filter.Eq("Id", id);
        await _comisionCollection.DeleteOneAsync(filter);
        return;
    }

    public async Task UploadFromBytesAsync(FileContent file)
    {
        //mongoDbDatabase : get database of MongoDb by establishing 
        //connection
        var gridFsBucket = new GridFSBucket(database);
        await gridFsBucket.UploadFromBytesAsync(file.Name, file.Content);
        return;
    }

    public async Task<byte[]> DownloadFromStream(string fileName)
    {
        //mongoDbDatabase: establish connection from mongodb and get //Databese
        var gridFsBucket = new GridFSBucket(database);
        var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
        var finData = await gridFsBucket.FindAsync(filter);
        var firstData = finData.FirstOrDefault();
        var bsonId = firstData.Id;
        var dataStream = await gridFsBucket.DownloadAsBytesAsync(bsonId);
        return dataStream;
    }
}