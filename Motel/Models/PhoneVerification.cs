using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Motel.Models
{
    public class PhoneVerification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("otp")]
        public string OTP { get; set; }

        [BsonElement("expirationTime")]
        public DateTime ExpirationTime { get; set; }

        [BsonElement("isVerified")]
        public bool IsVerified { get; set; }
    }
}
