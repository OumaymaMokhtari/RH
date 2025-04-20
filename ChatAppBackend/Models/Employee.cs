using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatAppBackend.Models
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("id")]
        public string EmployeeId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }

        // Nouveaux champs ajoutés :
        [BsonElement("cin")]
        public string Cin { get; set; }

        [BsonElement("dateNaissance")]
        public string DateNaissance { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("employeIdSql")]
        public string EmployeIdSql { get; set; }

        [BsonElement("tele")]
        public string Tele { get; set; }
    }
}
