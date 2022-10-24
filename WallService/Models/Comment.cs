using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WallService.Models;

[Serializable, BsonIgnoreExtraElements]
public class Comment
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public int Id { get; set; }

    [BsonElement("text"), BsonRepresentation(BsonType.String)]
    public string Text { get; set; }

    [BsonElement("create_on"), BsonRepresentation(BsonType.DateTime)]
    public DateTime CreateOn { get; set; }

    [BsonElement("post_id"), BsonRepresentation(BsonType.ObjectId)]
    public int PostId { get; set; }

    [BsonElement("user_id"), BsonRepresentation(BsonType.ObjectId)]
    public int UserId { get; set; }

    [BsonElement("likes")]
    public List<int> Likes { get; set; }
}