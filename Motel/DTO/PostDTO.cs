using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Motel.Models;

namespace Motel.DTO
{
    public class PostDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid? OwnerId { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        [BsonElement("location")]
        public Location? Location { get; set; }

        public bool Available { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();

        public DateTime? CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; } = DateTime.Now;

        public double Area { get; set; }

        public Dictionary<string, bool> Amenities { get; set; } = new Dictionary<string, bool>();

        public int Is_Browse { set; get; } = 0;

        [BsonRepresentation(BsonType.ObjectId)]
        public string? CategoryId { get; set; }
        public UserDTO? user { get; set; }
        public Categories categories { get; set; }
        public string? VideoURL { set; get; }
        public string? Slug { get; set; }
    }
    
}
