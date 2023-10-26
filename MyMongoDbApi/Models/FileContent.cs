using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyMongoDbApi.Models;

public class FileContent
{
    [BsonId]
    public ObjectId Id { get; set; }

    public byte[] Content { get; set; } = null!;

    public string Name { get; set; } = null!;
}