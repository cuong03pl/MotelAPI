using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class PostRepository : IPostRepository
    {
        public MotelService _motelService { get; set; }
        private readonly IWebHostEnvironment _evn;
        private readonly GenerateSlug _generateSlug;
        public PostRepository(MotelService motelService, IWebHostEnvironment evn, GenerateSlug generateSlug) { 
            _motelService = motelService;
            _evn = evn;
            _generateSlug = generateSlug;
        }
        public async Task<object> GetPosts(int page, int pageSize, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, string categoryId, string provinceSlug, string districtSlug, int? isBrowse = null)
        {
            var matchConditions = new BsonArray();

            if (minPrice.HasValue)
                matchConditions.Add(new BsonDocument("Price", new BsonDocument("$gte", minPrice.Value)));

            if (maxPrice.HasValue)
                matchConditions.Add(new BsonDocument("Price", new BsonDocument("$lte", maxPrice.Value)));

            if (minArea.HasValue)
                matchConditions.Add(new BsonDocument("Area", new BsonDocument("$gte", minArea.Value)));

            if (maxArea.HasValue)
                matchConditions.Add(new BsonDocument("Area", new BsonDocument("$lte", maxArea.Value)));

            if (!string.IsNullOrEmpty(categoryId))
                matchConditions.Add(new BsonDocument("CategoryId", new ObjectId(categoryId)));

            if (!string.IsNullOrEmpty(provinceSlug))
                matchConditions.Add(new BsonDocument("location.ProvinceSlug", _generateSlug.TextToSlug(provinceSlug)));

            if (!string.IsNullOrEmpty(districtSlug))
                matchConditions.Add(new BsonDocument("location.DistrictSlug", _generateSlug.TextToSlug(districtSlug)));

            if (isBrowse.HasValue)
                matchConditions.Add(new BsonDocument("Is_Browse", isBrowse));

            var matchStage = new BsonDocument("$match", new BsonDocument("$and", matchConditions));

            var pipeline = new[]
            {
               matchStage,
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "OwnerId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Categories" },
                    { "localField", "CategoryId" },
                    { "foreignField", "_id" },
                    { "as", "categories" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$categories" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "user", new BsonDocument
                        {
                            { "FullName", "$user.FullName" }  ,
                            { "PhoneNumber", "$user.PhoneNumber" },
                        {"CreatedOn", "$user.CreatedOn" },
                        {"Avatar", "$user.Avatar" },
                        }
                    },
                    {
                    "categories", new BsonDocument
                                {
                                    { "Name", "$categories.Name" }  ,
                                }
                    },
                    { "_id", 1 },
                    { "OwnerId", 1 },
                    { "Title", 1 },
                    { "Description", 1 },
                    { "Price", 1 },
                    { "location", 1 },
                    { "Available", 1 },
                    { "ImageUrls", 1 },
                    { "CreateAt", 1 },
                    { "UpdateAt", 1 },
                    { "Area", 1 },
                    { "Amenities", 1 },
                    { "Is_Browse", 1 },
                     { "CategoryId", 1 },
                     { "Slug", 1 }
                }),

                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var posts = await _motelService.GetPostCollection()
                          .Aggregate<PostDTO>(pipeline)
                          .ToListAsync();

            var totalCount = await _motelService.GetPostCollection()
                 .CountDocumentsAsync(new BsonDocument("$and", matchConditions));


            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = posts
            };
        }


        public async Task<object> GetApprovedPosts(int page, int pageSize, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, string categoryId, string provinceSlug, string districtSlug)
        {
            var matchConditions = new BsonArray();

            if (minPrice.HasValue)
                matchConditions.Add(new BsonDocument("Price", new BsonDocument("$gte", minPrice.Value)));

            if (maxPrice.HasValue)
                matchConditions.Add(new BsonDocument("Price", new BsonDocument("$lte", maxPrice.Value)));

            if (minArea.HasValue)
                matchConditions.Add(new BsonDocument("Area", new BsonDocument("$gte", minArea.Value)));

            if (maxArea.HasValue)
                matchConditions.Add(new BsonDocument("Area", new BsonDocument("$lte", maxArea.Value)));

            if (!string.IsNullOrEmpty(categoryId))
                matchConditions.Add(new BsonDocument("CategoryId", new ObjectId(categoryId)));

            if (!string.IsNullOrEmpty(provinceSlug))
                matchConditions.Add(new BsonDocument("location.ProvinceSlug", _generateSlug.TextToSlug(provinceSlug)));

            if (!string.IsNullOrEmpty(districtSlug))
                matchConditions.Add(new BsonDocument("location.DistrictSlug", _generateSlug.TextToSlug(districtSlug)));
            matchConditions.Add( new BsonDocument("Is_Browse", 1));
           
            var matchStage = new BsonDocument("$match", new BsonDocument("$and", matchConditions));

            var pipeline = new[]
            {
               matchStage,
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "OwnerId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Categories" },
                    { "localField", "CategoryId" },
                    { "foreignField", "_id" },
                    { "as", "categories" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$categories" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "user", new BsonDocument
                        {
                            { "FullName", "$user.FullName" }  ,
                            { "PhoneNumber", "$user.PhoneNumber" },
                        {"CreatedOn", "$user.CreatedOn" },
                        {"Avatar", "$user.Avatar" },
                        }
                    },
                    {
                    "categories", new BsonDocument
                                {
                                    { "Name", "$categories.Name" }  ,
                                }
                    },
                    { "_id", 1 },
                    { "OwnerId", 1 },
                    { "Title", 1 },
                    { "Description", 1 },
                    { "Price", 1 },
                    { "location", 1 },
                    { "Available", 1 },
                    { "ImageUrls", 1 },
                    { "CreateAt", 1 },
                    { "UpdateAt", 1 },
                    { "Area", 1 },
                    { "Amenities", 1 },
                    { "Is_Browse", 1 },
                     { "CategoryId", 1 },
                     { "Slug", 1 }
                }),

                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var posts = await _motelService.GetPostCollection()
                          .Aggregate<PostDTO>(pipeline)
                          .ToListAsync();

            var totalCount = await _motelService.GetPostCollection()
                 .CountDocumentsAsync(new BsonDocument("$and", matchConditions));


            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = posts
            };
        }
        public async Task<Posts> CreatePost(Posts post, List<IFormFile> imageFiles)
        {
            try
            {
                var imageUrls = await UploadImagesAsync(imageFiles);
                post.ImageUrls = imageUrls;
                var posts = _motelService.GetPostCollection();
                post.Slug = _generateSlug.CreateSlug(post.Title);
                post.Location.ProvinceSlug = _generateSlug.TextToSlug(post.Location.Province);
                post.Location.DistrictSlug = _generateSlug.TextToSlug(post.Location.District);
                post.Slug = _generateSlug.CreateSlug(post.Title);
                post.CreateAt = DateTime.Now;
                posts.InsertOne(post);
                return post;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
                return null;
            }
        }

        public void UpdatePost(string id , Posts post)
        {
            var collection = _motelService.GetPostCollection();
            var current_post = _motelService.GetPostCollection().Find(p => p.Id == id).FirstOrDefault();
            post.ImageUrls = current_post.ImageUrls;
            post.Slug = current_post.Slug;
            post.Location.ProvinceSlug = _generateSlug.TextToSlug(post.Location.Province);
            post.Location.DistrictSlug = _generateSlug.TextToSlug(post.Location.District);
            post.Id = id;
            post.UpdateAt = DateTime.Now;
            var filter = Builders<Posts>.Filter.Eq(p => p.Id, id);
            collection.ReplaceOne(filter, post);
        }

        public void DeletePost(string id)
        {
           _motelService.GetPostCollection().DeleteOne(post => post.Id == id);
        }

        public PostDTO GetPost(string slug)
        {
            var pipeline = new[]
             {
               new BsonDocument("$match", new BsonDocument("Slug", slug)),
                 new BsonDocument("$lookup", new BsonDocument
                 {
                     { "from", "applicationUsers" },
                     { "localField", "OwnerId" },
                     { "foreignField", "_id" },
                     { "as", "user" }
                 }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Categories" },
                    { "localField", "CategoryId" },
                    { "foreignField", "_id" },
                    { "as", "categories" }
                }),
                
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$categories" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                  new BsonDocument("$unwind", new BsonDocument
                 {
                     { "path", "$user" },
                     { "preserveNullAndEmptyArrays", true }
                 }),
                  new BsonDocument("$project", new BsonDocument
                    {
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" }  ,
                                { "PhoneNumber", "$user.PhoneNumber" },
                                {"CreatedOn", "$user.CreatedOn" },
                                {"Avatar", "$user.Avatar" },
                            }
                        },
                          {
                              "categories", new BsonDocument
                                {
                                    { "Name", "$categories.Name" }  ,
                                }
                          },
                      { "CategoryId", 1 },
                        { "_id", 1 },
                        { "OwnerId", 1 },
                        { "Title", 1 },
                        { "Description", 1 },
                        { "Price", 1 },
                        { "location", 1 },
                        { "Available", 1 },
                        { "ImageUrls", 1 },
                        { "CreateAt", 1 },
                        { "UpdateAt", 1 },
                        { "Area", 1 },
                        { "Amenities", 1 },
                        { "Is_Browse", 1 },
                        { "Slug", 1 }
                    })
             };
            return _motelService.GetPostCollection().Aggregate<PostDTO>(pipeline).FirstOrDefault();
        }

        public long GetCount()
        {
            return _motelService.GetPostCollection().CountDocuments(FilterDefinition<Posts>.Empty);
        }

        public bool Browse(string id)
        {
            var filter = Builders<Posts>.Filter.Eq(p => p.Id, id);
            var current_status = _motelService.GetPostCollection().Find(filter).Project(p => p.Is_Browse).FirstOrDefault();
           var new_sattus = (current_status == 1 ? 0 : 1);
            var update = Builders<Posts>.Update.Set("Is_Browse", new_sattus);
            var result = _motelService.GetPostCollection().UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }

        public List<Posts> SearchByLocation(Location location)
        {
            return _motelService.GetPostCollection().
                Find(p => p.Location.Province == location.Province && p.Location.District == location.District && p.Is_Browse == 1).ToList();

        }

        //public List<Posts> GetFiltered(decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, string categoryId, string provinceSlug, string? districtSlug)
        //{
        //    var filterBuilder = Builders<Posts>.Filter;
        //    var filters = new List<FilterDefinition<Posts>>();
        //    if (minArea.HasValue) filters.Add(filterBuilder.Gte(p => p.Area, minArea.Value));
        //    if (maxArea.HasValue) filters.Add(filterBuilder.Lte(p => p.Area, maxArea.Value));
        //    if (minPrice.HasValue) filters.Add(filterBuilder.Gte(p => p.Price, minPrice.Value));
        //    if (maxPrice.HasValue) filters.Add(filterBuilder.Lte(p => p.Price, maxPrice.Value));
        //    if (categoryId != null) filters.Add(filterBuilder.Eq(p => p.CategoryId, categoryId));
        //    if (provinceSlug != null) filters.Add(filterBuilder.Eq(p => p.Location.ProvinceSlug, _generateSlug.TextToSlug(provinceSlug)));
        //    if (districtSlug != null) filters.Add(filterBuilder.Eq(p => p.Location.DistrictSlug, _generateSlug.TextToSlug(districtSlug)));
        //    filters.Add(filterBuilder.Eq(p => p.Is_Browse, 1));
        //    var filter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;
            

        //    return _motelService.GetPostCollection().Find(filter).ToList();
        //}

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
        {
            var imagePaths = new List<string>();

            // Đường dẫn thư mục lưu trữ ảnh
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Kiểm tra nếu thư mục chưa tồn tại, tạo nó
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file.FileName); // Lấy tên file
                    var filePath = Path.Combine(imageFolder, fileName);

                    // Kiểm tra nếu file đã tồn tại thì thay đổi tên (có thể bổ sung số đếm)
                    var count = 1;
                    while (File.Exists(filePath))
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                        var extension = Path.GetExtension(file.FileName);
                        fileName = $"{fileNameWithoutExtension}_{count}{extension}";
                        filePath = Path.Combine(imageFolder, fileName);
                        count++;
                    }

                    // Mở stream và sao chép dữ liệu từ IFormFile vào file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn ảnh vào danh sách
                    imagePaths.Add($"/images/{fileName}"); // Lưu đường dẫn ảnh (dùng cho frontend)
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có, ví dụ như lưu log hoặc thông báo cho người dùng
                    Console.WriteLine($"Error uploading image {file.FileName}: {ex.Message}");
                }
            }

            return imagePaths;
        }

        public List<Posts> GetLatestPosts(int count = 5)
        {
            return _motelService.GetPostCollection().Find(p => p.Is_Browse == 1).SortByDescending(post => post.CreateAt).Limit(count).ToList();
        }

        public async Task<object> GetPostsByCategory(string slug, decimal? minPrice, decimal? maxPrice, double? minArea, double? maxArea, int page = 1, int pageSize = 1)
        {
            var category = _motelService.GetCategoryCollection().Find(cate => cate.Slug == slug).FirstOrDefault();
            var matchConditions = new BsonArray();

            if (minPrice.HasValue)
                matchConditions.Add(new BsonDocument("Price", new BsonDocument("$gte", minPrice.Value)));

            if (maxPrice.HasValue)
                matchConditions.Add(new BsonDocument("Price", new BsonDocument("$lte", maxPrice.Value)));

            if (minArea.HasValue)
                matchConditions.Add(new BsonDocument("Area", new BsonDocument("$gte", minArea.Value)));

            if (maxArea.HasValue)
                matchConditions.Add(new BsonDocument("Area", new BsonDocument("$lte", maxArea.Value)));

                matchConditions.Add( new BsonDocument("Is_Browse", 1));
            var matchStage = new BsonDocument("$match", new BsonDocument("$and", matchConditions));
            var pipeline = new[]
            {
                matchStage,
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "OwnerId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Categories" },
                    { "localField", "CategoryId" },
                    { "foreignField", "_id" },
                    { "as", "categories" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$categories" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                   new BsonDocument("$match", new BsonDocument("categories.Slug", slug)),
                new BsonDocument("$project", new BsonDocument
                {
                    { "user", new BsonDocument
                        {
                            { "FullName", "$user.FullName" }  ,
                            { "PhoneNumber", "$user.PhoneNumber" },
                        {"CreatedOn", "$user.CreatedOn" },
                        {"Avatar", "$user.Avatar" },
                        }
                    },
                    {
                    "categories", new BsonDocument
                                {
                                    { "Name", "$categories.Name" }  ,
                                }
                    },
                    { "_id", 1 },
                    { "OwnerId", 1 },
                    { "Title", 1 },
                    { "Description", 1 },
                    { "Price", 1 },
                    { "location", 1 },
                    { "Available", 1 },
                    { "ImageUrls", 1 },
                    { "CreateAt", 1 },
                    { "UpdateAt", 1 },
                    { "Area", 1 },
                    { "Amenities", 1 },
                    { "Is_Browse", 1 },
                     { "CategoryId", 1 },
                    { "Slug", 1 }
                }),

                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var posts = await _motelService.GetPostCollection()
                          .Aggregate<PostDTO>(pipeline)
                          .ToListAsync();

            var totalCount = await _motelService.GetPostCollection()
                             .CountDocumentsAsync(post => post.CategoryId == category.Id );

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = posts
            };
        }
        public async Task<object> GetPostsByProvinceSlug(string id, int page, int pageSize)
        {
            var post = _motelService.GetPostCollection().Find(post => post.Id == id).FirstOrDefault();
            var pipeline = new[]
            {
               new BsonDocument("$match", new BsonDocument
        {
            { "location.ProvinceSlug", post.Location.ProvinceSlug },
            { "_id", new BsonDocument("$ne", new ObjectId(id)) },
                   {"Is_Browse", 1 }
        }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "OwnerId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Categories" },
                    { "localField", "CategoryId" },
                    { "foreignField", "_id" },
                    { "as", "categories" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$categories" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                   
                new BsonDocument("$project", new BsonDocument
                {
                    { "user", new BsonDocument
                        {
                            { "FullName", "$user.FullName" }  ,
                            { "PhoneNumber", "$user.PhoneNumber" },
                        {"CreatedOn", "$user.CreatedOn" },
                        {"Avatar", "$user.Avatar" },
                        }
                    },
                    {
                    "categories", new BsonDocument
                                {
                                    { "Name", "$categories.Name" }  ,
                                }
                    },
                    { "_id", 1 },
                    { "OwnerId", 1 },
                    { "Title", 1 },
                    { "Description", 1 },
                    { "Price", 1 },
                    { "location", 1 },
                    { "Available", 1 },
                    { "ImageUrls", 1 },
                    { "CreateAt", 1 },
                    { "UpdateAt", 1 },
                    { "Area", 1 },
                    { "Amenities", 1 },
                    { "Is_Browse", 1 },
                     { "CategoryId", 1 },
                    { "Slug", 1 }
                }),

                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };

            var posts = await _motelService.GetPostCollection()
                          .Aggregate<PostDTO>(pipeline)
                          .ToListAsync();
            return new
            {
                Data = posts
            };
        }

        public async Task<List<Location>> GetLocations()
        {
            var pipeline = new[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "location.ProvinceSlug", 1 },
                    { "location.Province", 1 },
                    { "_id", 0 }
                }),
               new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$location.ProvinceSlug" },
                        { "Province", new BsonDocument("$first", "$location.Province") }
                    }),

              new BsonDocument("$project", new BsonDocument
            {
                { "_id", 0 },
                { "ProvinceSlug", "$_id" }, 
                { "Province", 1 }
            })

            };

            var result = await _motelService.GetPostCollection()
                .Aggregate<BsonDocument>(pipeline)
                .ToListAsync();

            var locations = result.Select(doc => new Location
            {
                ProvinceSlug = doc["ProvinceSlug"].AsString,
                Province = doc["Province"].AsString
            }).ToList();

            return locations;
        }

        public async Task<object> GetPostsByUser(string userId)
        {
            var pipeline = new[]
            {
               new BsonDocument("$match", new BsonDocument
                {
                           {"OwnerId", userId }
                }),
                        new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "OwnerId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Categories" },
                    { "localField", "CategoryId" },
                    { "foreignField", "_id" },
                    { "as", "categories" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$categories" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new BsonDocument("$project", new BsonDocument
                {
                    { "user", new BsonDocument
                        {
                            { "FullName", "$user.FullName" }  ,
                            { "PhoneNumber", "$user.PhoneNumber" },
                        {"CreatedOn", "$user.CreatedOn" },
                        {"Avatar", "$user.Avatar" },
                        }
                    },
                    {
                    "categories", new BsonDocument
                                {
                                    { "Name", "$categories.Name" }  ,
                                }
                    },
                    { "_id", 1 },
                    { "OwnerId", 1 },
                    { "Title", 1 },
                    { "Description", 1 },
                    { "Price", 1 },
                    { "location", 1 },
                    { "Available", 1 },
                    { "ImageUrls", 1 },
                    { "CreateAt", 1 },
                    { "UpdateAt", 1 },
                    { "Area", 1 },
                    { "Amenities", 1 },
                    { "Is_Browse", 1 },
                     { "CategoryId", 1 },
                    { "Slug", 1 }
                }),
            };

            var posts = await _motelService.GetPostCollection()
                          .Aggregate<PostDTO>(pipeline)
                          .ToListAsync();
            return new
            {
                Data = posts
            };
        }
    }
}
