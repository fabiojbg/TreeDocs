using Apps.Sdk.Extensions;
using Apps.Sdk.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Repository.MongoDb.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDb.Repositories
{
    public class FileRepository<TEntity> : IFileRepository<TEntity> where TEntity : DbFileModel
    {
        IUnitOfWork _db;
        GridFSBucket __bucket;

        GridFSBucket _bucket
        {
            get
            {
                if (__bucket != null)
                    return __bucket;

                __bucket = new GridFSBucket(_db.Database, new GridFSBucketOptions
                {
                    BucketName = typeof(DbFileModel).Name,
                    ChunkSizeBytes = 100000,
                    WriteConcern = WriteConcern.WMajority,
                    ReadPreference = ReadPreference.Secondary
                });
                return __bucket;
            }
        }


        public FileRepository(IUnitOfWork db)
        {
            _db = db;
        }

        public async Task<ObjectId> CreateAsync(TEntity obj)
        {
            var contentsHash = HashingHelper.ToSHA1Hash(obj.Contents);
            var options = new GridFSUploadOptions
            {
                Metadata = new MongoDB.Bson.BsonDocument
                {
                    { "CreatedOn", DateTime.UtcNow },
                    { "ContentsHash",  contentsHash }
                }
            };

            obj.ContentsHash = contentsHash;
            var id = await _bucket.UploadFromBytesAsync(obj.Name ?? Guid.NewGuid().ToString(),
                                                         obj.Contents,
                                                         options);
            obj.Id = id;
            return id;
        }

        public virtual async Task<TEntity> GetByIdAsync(ObjectId id, params string[] fieldsToReturn)
        {
            if (id == ObjectId.Empty)
                return null;
           
            var result = (TEntity)Activator.CreateInstance(typeof(TEntity));
            result.Id = id;

            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, id);
            using (var cursor = await _bucket.FindAsync(filter, new GridFSFindOptions { Limit = 1}))
            {
                var fileInfo = cursor.ToList().FirstOrDefault();
                result.Name = fileInfo.Filename;
                result.CreatedOn = fileInfo.Metadata["CreatedOn"].ToLocalTime();
            }

            var data = await _bucket.DownloadAsBytesAsync(id);

            result.Contents = data;
            result.ContentsHash = HashingHelper.ToSHA1Hash(data);

            return result; 
        }

        public async Task<TEntity> GetInfoByIdWithoutContentsAsync(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return null;

            var result = (TEntity)Activator.CreateInstance(typeof(TEntity));
            result.Id = id;

            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, id);
            using (var cursor = await _bucket.FindAsync(filter, new GridFSFindOptions { Limit = 1 }))
            {
                var fileInfo = cursor.ToList().FirstOrDefault();
                result.Name = fileInfo.Filename;
                result.CreatedOn = fileInfo.Metadata["CreatedOn"].ToLocalTime();
                result.ContentsHash = fileInfo.Metadata["ContentsHash"].ToString();
            }

            return result;
        }


        public virtual Task<IEnumerable<TEntity>> GetListAsync(ObjectId? folderId)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateAsync(TEntity obj)
        {
            throw new NotImplementedException();
        }

        public virtual async Task DeleteAsync(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return;

            await _bucket.DeleteAsync(id);
        }

    }
}
