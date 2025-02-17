using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        public MotelService _motelService { get; set; }
        public IMapper _mapper;
        public ReviewRepository(MotelService motelService, IMapper mapper) {
            _motelService  = motelService;
            _mapper = mapper;
        }
        public void CreateReview(Reviews review)
        {
            var reviews = _motelService.GetReviewCollection();
            review.CreateAt = DateTime.Now;
            reviews.InsertOne(review);
        }

        public bool DeleteReview(string id)
        {
            var result = _motelService.GetReviewCollection().DeleteOne(review => review.Id == id);
            return result.DeletedCount > 0;
        }

       
        public ReviewDTO GetReview(string id)
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
                     {"Comment", 1 },
                     {"PostId", 1 },
                     {"UserId", 1 },
                     {"post", 1 },
                      { "CreateAt", 1 },
                    { "UpdateAt", 1 },

                    })

            };
            return _motelService.GetReviewCollection().Aggregate<ReviewDTO>(pipeline).FirstOrDefault();
        }

        public async Task<object> GetReviews(int page, int pageSize)
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
                     {"Comment", 1 },
                     {"PostId", 1 },
                     {"UserId", 1 },
                     {"post", 1 },
                     { "CreateAt", 1 },
                    { "UpdateAt", 1 },

                    }),
                   new BsonDocument("$skip", (page-1) * pageSize),
                    new BsonDocument("$limit", pageSize),

            };

            var reviews =  _motelService.GetReviewCollection().Aggregate<ReviewDTO>(pipeline).ToList();
            var totalCount = _motelService.GetReviewCollection().CountDocuments(new BsonDocument());
           var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new {
                Data = reviews,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }


        public bool UpdateReview(string id, Reviews review)
        {
            review.UpdateAt = DateTime.Now;

            var result = _motelService.GetReviewCollection().ReplaceOne(review => review.Id == id, review);
            return result.ModifiedCount > 0;
        }

        public List<ReviewDTO> GetReviewsByPost(string slug)
        {
            var post = _motelService.GetPostCollection().Find(p => p.Slug == slug).FirstOrDefault();
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("PostId", new ObjectId(post.Id))),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
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
                                {"Avatar", "$user.Avatar" }
                            }
                        },
                     {"Comment", 1 },
                     {"PostId", 1 },
                     {"UserId", 1 },
                     {"post", 1 },
                     { "CreateAt", 1 },
                    { "UpdateAt", 1 },

                    }),
                new BsonDocument("$sort", new BsonDocument("CreateAt", -1)),


            };

            var reviews =  _motelService.GetReviewCollection().Aggregate<ReviewDTO>(pipeline).ToList();

            return reviews;
        }
    }
}
