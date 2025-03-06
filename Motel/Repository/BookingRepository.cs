using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        public MotelService _motelService { get; set; }
        public BookingRepository(MotelService motelService) { 
            _motelService = motelService;
        }
        public void CreateBooking(Bookings booking)
        {
            try
                {
                    var isCheck = _motelService.GetBookingCollection().Find(b => b.UserId == booking.UserId && b.PostId == booking.PostId).FirstOrDefault();
                    if (isCheck != null)
                    {
                    return;
                    }
                    var post = _motelService.GetPostCollection().Find(p => p.Id == booking.PostId).FirstOrDefault();
                    var price = (post.Price * 30)/100;
                    var bookings = _motelService.GetBookingCollection();
                    booking.CreateAt = DateTime.Now;
                    booking.Status = 1;
                    booking.Price = price;
                    bookings.InsertOne(booking);
                }
            catch (Exception ex)
                {
                    Console.WriteLine($"Error creating booking: {ex.Message}");
                }
        }

        public Bookings GetBooking(string id)
        {
           return _motelService.GetBookingCollection().Find(b => b.Id == id).FirstOrDefault();
        }

        public object GetBookings(int page, int pageSize)
        {
            var pipeline = new[]
             {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Posts" },
                    { "localField", "PostId" },
                    { "foreignField", "_id" },
                    { "as", "post" }
                }),
                 new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$post" },

                }),
                new BsonDocument("$project", new BsonDocument
                    {
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" }  ,
                                { "PhoneNumber", "$user.PhoneNumber" },
                            }
                        },
                        {"post", 1 },
                        {"Price", 1 },
                        {"Status", 1 },
                    }),
            };
            var bookings = _motelService.GetBookingCollection().Aggregate<BookingDTO>(pipeline).ToList();
            var totalCount = _motelService.GetBookingCollection().CountDocuments(new BsonDocument());
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new
            {
                Data = bookings,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }

        public long GetCount()
        {
          return  _motelService.GetBookingCollection().CountDocuments(_ => true);
        }

        public object GetUserBookings(Guid userId)
        {
           
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("UserId", new BsonBinaryData(userId, GuidRepresentation.Standard))),
               
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Posts" },
                    { "localField", "PostId" },
                    { "foreignField", "_id" },
                    { "as", "post" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$post" }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    
                    { "post", 1 },
                     { "Price", new BsonDocument("$toDecimal", "$Price") },
                    { "Status", 1 }
                })
            };
            var bookings = _motelService.GetBookingCollection().Aggregate<BookingDTO>(pipeline).ToList();
            return new
            {
                Data = bookings
            };
        }

        public bool HasUserBooked(Guid userId, string postId)
        {
            var filter = Builders<Bookings>.Filter.And(
        Builders<Bookings>.Filter.Eq(b => b.UserId, userId),
        Builders<Bookings>.Filter.Eq(b => b.PostId, postId)
    );

            var exists = _motelService.GetBookingCollection().Find(filter).Any();
            return exists;
        }
    }
}
