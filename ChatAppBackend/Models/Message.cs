using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChatAppBackend.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("senderId")]
        [Required]
        public string SenderId { get; set; }

        [BsonElement("receiverId")]
        [Required]
        public string ReceiverId { get; set; }

        [BsonElement("content")]
        [Required]
        public string Content { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("isRead")] 
        public bool IsRead { get; set; } = false;
    }
}
