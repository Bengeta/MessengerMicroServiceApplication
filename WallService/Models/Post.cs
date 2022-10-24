using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WallService.Models;
[Serializable,BsonIgnoreExtraElements]
public class Post
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public int Id { get; set; }
    [BsonElement("owner_id"), BsonRepresentation(BsonType.Int32)]
    public int OwnerId { get; set; }
    [BsonElement("text"), BsonRepresentation(BsonType.String)]
    public string Text { get; set; }
    [BsonElement("create_on"), BsonRepresentation(BsonType.DateTime)]
    public DateTime CreateOn { get; set; }
    [BsonElement("likes")]
    public List<int> Likes { get; set; }
    [BsonElement("reposts_count"), BsonRepresentation(BsonType.Int32)]
    public int Reposts { get; set; }
    [BsonElement("views_count"), BsonRepresentation(BsonType.Int32)]
    public int Views { get; set; }
    [BsonElement("comments")]
    public List<Comment> Comments { get; set; }
}