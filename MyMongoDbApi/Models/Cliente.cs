using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MyMongoDbApi.Models;

public class Cliente
{
    
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    [BsonElement("items")]
    [JsonPropertyName("items")]
    public List<string> ComisionesIds { get; set; } = null!;
}