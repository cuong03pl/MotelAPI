using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class ReportRepository : IReportRepository
    {
        public MotelService _motelService;
        private readonly IWebHostEnvironment _env;
        public ReportRepository(MotelService motelService, IWebHostEnvironment env) {
            _motelService = motelService;
            _env = env;
        }

        public bool Browse(string id)
        {
            var filter = Builders<Reports>.Filter.Eq(p => p.Id, id);
            var current_status = _motelService.GetReportCollection().Find(filter).Project(p => p.Status).FirstOrDefault();
            var new_sattus = (current_status == 1 ? 0 : 1);
            var update = Builders<Reports>.Update.Set("Status", new_sattus);
            var result = _motelService.GetReportCollection().UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }

        public long CountReport()
        {
           return _motelService.GetReportCollection().CountDocuments(FilterDefinition<Reports>.Empty);
        }

        public void CreateReport(Reports report)
        {
            var reports = _motelService.GetReportCollection();
            report.CreateAt = DateTime.Now;
            reports.InsertOne(report);
        }

        public bool DeleteReport(string id)
        {
           var result = _motelService.GetReportCollection().DeleteOne(report => report.Id == id);

            return result.DeletedCount > 0;
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
            document.Add(new Paragraph("Báo cáo tố cáo tháng " + month + "/" + year).SetFontSize(18));
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
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
                     { "preserveNullAndEmptyArrays", true }

                }),
                 new BsonDocument("$project", new BsonDocument
                    {
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" }  ,
                                { "PhoneNumber", "$user.PhoneNumber" },
                            }
                        },
                     {"Reason", 1 },
                     {"Note", 1 },
                     {"Status", 1 },
                     {"PostId", 1 },
                     {"UserId", 1 },
                     {"post", 1 }

                    }),

            };
            var reports = _motelService.GetReportCollection().Aggregate<ReportDTO>(pipeline).ToList();


            document.Add(new Paragraph("Tổng số báo cáo: " + reports.Count()).SetFontSize(15));
            Table table = new Table(6).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Người báo cáo")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Số điện thoại")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Tên bài viết")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Lý do")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Nội dùng")).SetBorder(new SolidBorder(1)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ngày tạo")).SetBorder(new SolidBorder(1)));
            foreach (var report in reports)
            {
                table.AddCell(new Cell().Add(new Paragraph(report.user.FullName)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(report.user.PhoneNumber)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(report.post.Title)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(report.Reason)).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(report.Note.ToString())).SetBorder(new SolidBorder(1)));
                table.AddCell(new Cell().Add(new Paragraph(report.CreateAt.ToString())).SetBorder(new SolidBorder(1)));
            }
            
            document.Add(table);
            document.Close();
            return stream.ToArray();
        }

        public ReportDTO GetReport(string id)
        {
            var pipeline = new[]
             {
              new BsonDocument("$match", new BsonDocument("_id", new ObjectId(id))),
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
                       
                    })

            };
            return _motelService.GetReportCollection().Aggregate<ReportDTO>(pipeline).FirstOrDefault();
        }

        public async Task<object> GetReports(int page, int pageSize)
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
                     { "preserveNullAndEmptyArrays", true }

                }),
                 new BsonDocument("$project", new BsonDocument
                    {
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" }  ,
                                { "PhoneNumber", "$user.PhoneNumber" },
                            }
                        },
                     {"Reason", 1 },
                     {"Note", 1 },
                     {"Status", 1 },
                     {"PostId", 1 },
                     {"UserId", 1 },
                     {"post", 1 }

                    }),
                    new BsonDocument("$skip", (page-1) * pageSize),
                    new BsonDocument("$limit", pageSize),

            };
            var reports = _motelService.GetReportCollection().Aggregate<ReportDTO>(pipeline).ToList();
            var totalCount = _motelService.GetReportCollection().CountDocuments(new BsonDocument());
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = reports
            };
        }

        public bool UpdateReport(string id, Reports report)
        {
            report.UpdateAt = DateTime.Now;

            var result =  _motelService.GetReportCollection().ReplaceOne(report => report.Id == id, report);
            return result.ModifiedCount > 0;
        }

        public async Task<List<ReportReasonCount>> GetReportsByReason()
        {
            var reasonsList = new List<string>
            {
                "Tin có dấu hiệu lừa đảo",
                "Tin trùng lặp nội dung",
                "Không liên hệ được chủ tin đăng",
                "Thông tin không đúng thực tế (giá, diện tích, hình ảnh...)",
                "Lý do khác"
            };

            var reasonsCount = reasonsList.Select(reason => new ReportReasonCount
            {
                Reason = reason,
                Count = 0
            }).ToList();

            // Lấy tất cả báo cáo từ collection
            var reports = await _motelService.GetReportCollection().Find(FilterDefinition<Reports>.Empty).ToListAsync();
            
            // Đếm số lượng theo từng loại lý do
            foreach (var report in reports)
            {
                var existingReason = reasonsCount.FirstOrDefault(r => r.Reason == report.Reason);
                if (existingReason != null)
                {
                    existingReason.Count++;
                }
                else
                {
                    // Nếu lý do không thuộc các loại được định nghĩa trước, gán vào "Lý do khác"
                    var otherReason = reasonsCount.FirstOrDefault(r => r.Reason == "Lý do khác");
                    if (otherReason != null)
                    {
                        otherReason.Count++;
                    }
                }
            }

            return reasonsCount;
        }
       
    }
}
