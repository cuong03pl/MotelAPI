
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Layout.Borders;

namespace Motel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        public MotelService _motelService { get; set; }
        private readonly IWebHostEnvironment _env;
        public BookingRepository(MotelService motelService, IWebHostEnvironment env) { 
            _motelService = motelService;
            _env = env;
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

                decimal bookingPrice;
                if (post.Price < 3) 
                {
                    bookingPrice = 0.1m;
                }
                else if (post.Price <= 5) 
                {
                    bookingPrice = 0.15m;
                }
                else 
                {
                    bookingPrice = 0.2m;
                }

                var bookings = _motelService.GetBookingCollection();
                booking.CreateAt = DateTime.Now;
                booking.Status = 0;
                booking.Price = bookingPrice;
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
                    {"CreateAt", 1 }
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
                    { "Status", 1 },
                    {"CreateAt", 1 }
                })
            };
            var bookings = _motelService.GetBookingCollection().Aggregate<BookingDTO>(pipeline).ToList();
            return new
            {
                Data = bookings
            };
        }

      
        public byte[] GeneratePdfReport()
        {

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            string fontPath = Path.Combine(_env.WebRootPath, "fonts", "Roboto-Regular.ttf");
            PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            document.SetFont(font);
            document.Add(new Paragraph("Báo cáo đăng bài tháng " + month + "/" + year).SetFontSize(18));
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
            var pipeline = new[]
             {
                new BsonDocument("$match", new BsonDocument
                    {
                        { "CreateAt", new BsonDocument
                            {
                                { "$gte", firstDayOfMonth },
                                { "$lt", firstDayOfNextMonth }
                            }
                        }
                    }),
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
                        {"CreateAt", 1 }
                    }),
            };
            var bookings = _motelService.GetBookingCollection().Aggregate<BookingDTO>(pipeline).ToList();


            document.Add(new Paragraph("Tổng số bài đăng: " + bookings.Count()).SetFontSize(15));
            Table table = new Table(5).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Người đăng")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Số điện thoại")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Tên bài viết")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ngày tạo")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Tổng tiền (tr)")).SetBorder(new SolidBorder(1)));
            decimal totalPrice = 0;
            foreach ( var booking in bookings)
            {
                totalPrice += booking.post.Price;
                table.AddCell(new Cell().Add(new Paragraph(booking.user.FullName)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(booking.user.PhoneNumber)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph( booking.post.Title)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph( booking.CreateAt.ToString())).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(booking.post.Price.ToString())).SetBorder(new SolidBorder(1)));
            }
            table.AddCell(new Cell(1,4).Add(new Paragraph()).SetBorder(new SolidBorder(1)));
            table.AddCell(new Cell().Add(new Paragraph("Tổng: " + totalPrice.ToString() + "tr")).SetBorder(new SolidBorder(1)));
            document.Add(table);
            document.Close();
            return stream.ToArray();
        }

        public bool UpdateStatus(Guid userId, string postId)
        {
            var filter = Builders<Bookings>.Filter.Eq(b => b.UserId, userId) &
                 Builders<Bookings>.Filter.Eq(b => b.PostId, postId);

            var update = Builders<Bookings>.Update.Set(b => b.Status, 1); // ví dụ: 1 = approved

            var result = _motelService.GetBookingCollection().UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }

        public bool HasPayed(string postId)
        {
          var post = _motelService.GetBookingCollection().Find(b => b.PostId == postId).FirstOrDefault();

            if(post.Status == 1 )
            {
                return true;
            }
            return false;
        }

        public object GetBookingByPost(string postId)

        {
            var pipeline = new[]
            {
              new BsonDocument("$match", new BsonDocument("PostId", ObjectId.Parse(postId))),

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
                    { "Status", 1 },
                    {"CreateAt", 1 }
                })
            };
            var bookings = _motelService.GetBookingCollection().Aggregate<BookingDTO>(pipeline).ToList();
            return new
            {
                Data = bookings
            };
        }
    }
}
