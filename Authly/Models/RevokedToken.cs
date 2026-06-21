using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authly.Models
{
    public class RevokedToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("jti")]
        public string Jti { get; set; } = string.Empty;

        [BsonElement("expiresAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExpiresAt { get; set; }
    }
}
