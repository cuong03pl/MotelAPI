using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class UserRepository : IUserRepository
    {
        public MotelService _motelService { get; set; }
        public UserRepository(MotelService motelService)
        {
            _motelService = motelService;
        }

        public void DeleteUser(Guid id)
        {
            _motelService.GetUserCollection().DeleteOne(user => user.Id == id);

        }

        public ApplicationUser GetUser(Guid id)
        {

            return _motelService.GetUserCollection().Find(user => user.Id == id).FirstOrDefault();

        }

        public async Task<object> GetUsers(int page, int pageSize)
        {
            var user = _motelService.GetUserCollection().Find(user => true).Skip((page - 1) * pageSize).Limit(pageSize).ToList();
            var totalCount = _motelService.GetUserCollection().CountDocuments(new BsonDocument());
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new
            {
                Data = user,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }

        public void UpdateUser(Guid id, ApplicationUser user)
        {
            var updateDefinition = Builders<ApplicationUser>.Update
                .Set(u => u.PhoneNumber, user.PhoneNumber)
                .Set(u => u.FullName, user.FullName);

            _motelService.GetUserCollection().UpdateOne(u => u.Id == id, updateDefinition);
        }

        public void BlockUser(Guid id, bool is_block)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, id);
            var update = Builders<ApplicationUser>.Update.Set("IsBlock", is_block);
            var result = _motelService.GetUserCollection().UpdateOne(filter, update);
        }

        public long CountPost(Guid id)
        {
            var totalCount = _motelService.GetPostCollection().CountDocuments(post => post.OwnerId == id);
            return totalCount;
        }

        public async Task<bool> AddFavoritePost(Guid id, string postId)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, id);

            var user = await _motelService.GetUserCollection().Find(filter).FirstOrDefaultAsync();

            if (user == null) return false;

            var post = _motelService.GetPostCollection().Find(post => post.Id == postId).FirstOrDefault();
            if (post == null) return false;
            UpdateDefinition<ApplicationUser> update;

            if (user.Favorites.Contains(postId))
            {
                update = Builders<ApplicationUser>.Update.Pull(u => u.Favorites, postId);
            }
            else
            {
                update = Builders<ApplicationUser>.Update.AddToSet(u => u.Favorites, postId);
            }

            var result = await _motelService.GetUserCollection().UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public bool CheckFavorite(Guid userId, string postId)
        {
            var filter = Builders<ApplicationUser>.Filter.And(
                Builders<ApplicationUser>.Filter.Eq(u => u.Id, userId),
                Builders<ApplicationUser>.Filter.AnyEq(u => u.Favorites, postId)
            );
            var user = _motelService.GetUserCollection().Find(filter).FirstOrDefault();
            Console.WriteLine(user.UserName);
            return user != null;
        }

        public async Task<object> GetUserFavorite(Guid userId)
        {
            var user = _motelService.GetUserCollection().Find(u => u.Id == userId).FirstOrDefault();
            if (user == null || user.Favorites == null || !user.Favorites.Any())
            {
                return new List<object>();
            }

            var favoriteIds = user.Favorites.Select(fav => new ObjectId(fav)).ToList();

            var pipeline = new[]
            {
                    new BsonDocument("$match", new BsonDocument("_id", new BsonDocument("$in", new BsonArray(favoriteIds)))),

                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "applicationUsers" },
                        { "localField", "OwnerId" },
                        { "foreignField", "_id" },
                        { "as", "user" }
                    }),
                    new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$user" },
                        { "preserveNullAndEmptyArrays", true }
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

                    new BsonDocument("$project", new BsonDocument
                    {
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
                        { "Slug", 1 },
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" },
                                { "PhoneNumber", "$user.PhoneNumber" },
                                { "CreatedOn", "$user.CreatedOn" },
                                { "Avatar", "$user.Avatar" }
                            }
                        },
                        { "categories", new BsonDocument
                            {
                                { "Name", "$categories.Name" }
                            }
                        }
                    })
            };

            var result = await _motelService.GetPostCollection()
                                            .Aggregate<PostsDTO>(pipeline)
                                            .ToListAsync();

            return result;
        }

        public async Task<object> GetUserPosts(Guid userId)
        {
            var pipeline = new[]
            {
                    new BsonDocument("$match", new BsonDocument("OwnerId", new BsonBinaryData(userId, GuidRepresentation.Standard))),
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "applicationUsers" },
                        { "localField", "OwnerId" },
                        { "foreignField", "_id" },
                        { "as", "user" }
                    }),
                    new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$user" },
                        { "preserveNullAndEmptyArrays", true }
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

                    new BsonDocument("$project", new BsonDocument
                                    {
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
                                        { "Slug", 1 },
                                        { "user", new BsonDocument
                                            {
                                                { "FullName", "$user.FullName" },
                                                { "PhoneNumber", "$user.PhoneNumber" },
                                                { "CreatedOn", "$user.CreatedOn" },
                                                { "Avatar", "$user.Avatar" }
                                            }
                                        },
                                        { "categories", new BsonDocument
                                            {
                                                { "Name", "$categories.Name" }
                                            }
                                        }
                                    })
            };


            var result = await _motelService.GetPostCollection()
                                            .Aggregate<PostsDTO>(pipeline)
                                            .ToListAsync();

            return result;
        }

        public long GetCount()
        {
            return _motelService.GetUserCollection().CountDocuments(new BsonDocument());
        }

        public async Task<List<PostCountByMonthDTO>> GetPostCountsByMonth(int year)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "CreateAt", new BsonDocument
                        {
                            { "$gte", new DateTime(year, 1, 1) },
                            { "$lt", new DateTime(year + 1, 1, 1) }
                        }
                    }
                }),

                new BsonDocument("$project", new BsonDocument
                {
                    { "month", new BsonDocument("$month", "$CreateAt") } // Lấy tháng từ ngày tạo bài viết
                }),

                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$month" },
                    { "count", new BsonDocument("$sum", 1) } // Đếm số bài viết theo tháng
                }),

                new BsonDocument("$sort", new BsonDocument("_id", 1)) // Sắp xếp theo tháng tăng dần
        };

            var aggregationResult = await _motelService.GetPostCollection()
                                                       .Aggregate<BsonDocument>(pipeline)
                                                       .ToListAsync();

            // Tạo danh sách đủ 12 tháng, nếu tháng nào không có thì count = 0
            var fullMonthData = Enumerable.Range(1, 12)
                .Select(m => new PostCountByMonthDTO
                {
                    Month = m,
                    Count = aggregationResult.FirstOrDefault(x => x["_id"].AsInt32 == m)?["count"].AsInt32 ?? 0
                })
                .ToList();

            return fullMonthData;
        }
    }
    }
