using MongoDB.Bson.Serialization.Attributes;

namespace Motel.DTO
{
    public class PostCountByMonthDTO
    {
        [BsonElement("_id")]
        public int Month { get; set; }
        [BsonElement("count")]
        public int Count { get; set; }  
    }
}
