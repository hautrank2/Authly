using Authly.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authly.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("birthday")]
        public string Birthday { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

        [BsonElement("avtUrl")]
        public string? AvtUrl { get; set; }

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; } = UserRole.Dev;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("latestAccess")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LatestAccess { get; set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
