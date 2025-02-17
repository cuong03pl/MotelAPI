using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class Categories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Name { get; set; }
        public string? Slug { get; set; }


    }
}
