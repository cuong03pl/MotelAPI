using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class Posts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid? OwnerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? CategoryId { get; set; }
        public string? Title { get; set; }  

        public string? Description { get; set; }

        public decimal Price { get; set; } 

        [BsonElement("location")]
        public Location? Location { get; set; }

        public bool Available { get; set; }  

        public List<string> ImageUrls { get; set; } = new List<string>();

        public DateTime? CreateAt { get; set; } 

        public DateTime? UpdateAt { get; set; } 

        public double Area { get; set; } 

        public Dictionary<string, bool> Amenities { get; set; } = new Dictionary<string, bool>();

        public int Is_Browse { set; get; } = 0;
        public string? VideoURL { set; get; }
        public string? Slug { get; set; }


    }
    public class Location
    {
        public string? Province { get; set; } 
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? AddressLine { get; set; }
        public string? ProvinceSlug { get; set; }
        public string? DistrictSlug { get; set; }
        public int DistrictId { get; set; } 
        public int ProvinceId { get; set; }
        public int WardId { get; set; }
    }
}
