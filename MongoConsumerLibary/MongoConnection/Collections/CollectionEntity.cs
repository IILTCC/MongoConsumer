using Archive.Logs;
using MongoConsumerLibary.MongoConnection.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoConsumerLibary.MongoConnection.Collections
{
    class CollectionEntity<CollectionType> where CollectionType : ExpirableCollection
    {
        private readonly IMongoCollection<CollectionType> _collection;
        private readonly ArchiveLogger _logger;
        public CollectionEntity(IMongoDatabase database,string collectionName,ArchiveLogger logger)
        {
            _collection = database.GetCollection<CollectionType>(collectionName);
            AddIndex();
        }
        public CollectionEntity(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<CollectionType>(collectionName);
            AddIndex();
            _logger = null;
        }
        public void AddIndex()
        {
            CreateIndexOptions indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            IndexKeysDefinition<CollectionType> indexKeys = Builders<CollectionType>.IndexKeys.Ascending(document => document.ExpirationTime);
            _collection.Indexes.CreateOne(new CreateIndexModel<CollectionType>(indexKeys, indexOptions));
        }

        public void AddDocument(CollectionType document, int expireAfter)
        {
            document.InsertTime = DateTime.Now;
            document.ExpirationTime = DateTime.Now.AddSeconds(expireAfter);
            _collection.InsertOne(document);
        }
        private async Task<List<CollectionType>> GetDocumentBase(int limit,int skip, FilterDefinition<CollectionType> filter)
        {
            ProjectionDefinition<CollectionType> projection = Builders<CollectionType>.Projection.Exclude(Consts.ARCHIVE_ID_EXCLUDE);
            try
            {
                IFindFluent<CollectionType,CollectionType> query = _collection.Find(filter).Project<CollectionType>(projection) ;
                if (skip > 0)
                    query = query.Skip(skip);                
                if (limit > 0)
                    query = query.Limit(limit);
                return await query.ToListAsync();
            }
            catch (Exception e)
            {
                if(_logger != null)
                    _logger.LogError("Tried pulling from mongo "+e.Message,LogId.FatalMongoPull);
            }
            return new List<CollectionType>();
        }
        public async Task<long> GetDocumentCount(DateTime startDate, DateTime endDate, IcdType icdType)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate),
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, icdType.ToString() + Consts.ARCHIVE_ICD_ADDON)
                );
            ProjectionDefinition<CollectionType> projection = Builders<CollectionType>.Projection.Exclude(Consts.ARCHIVE_ID_EXCLUDE);
            try
            {
                return await _collection.CountDocumentsAsync(filter);
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.LogError("Tried pulling from mongo " + e.Message, LogId.FatalMongoPull);
            }
            return 0;
        }
        public async Task<long> GetStatisticsCount(DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate)
                );
            ProjectionDefinition<CollectionType> projection = Builders<CollectionType>.Projection.Exclude(Consts.ARCHIVE_ID_EXCLUDE);
            try
            {
                return await _collection.CountDocumentsAsync(filter);
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.LogError("Tried pulling from mongo " + e.Message, LogId.FatalMongoPull);
            }
            return 0;
        }
        public async Task<List<CollectionType>> GetDocument(int limit,int skip, DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
            Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
            Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate)
            );
            return await GetDocumentBase(limit, skip,filter);
        }        

        public async Task<List<CollectionType>> GetDocument(IcdType type,int limit,DateTime startDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, type.ToString() + Consts.ARCHIVE_ICD_ADDON),
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate));
            return await GetDocumentBase(limit,0,filter);
        }

        public async Task<List<CollectionType>> GetDocument(IcdType type, int limit, DateTime startDate,DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate),
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, type.ToString() + Consts.ARCHIVE_ICD_ADDON)
                );
            return await GetDocumentBase(limit,0,filter);
        }

        public async Task<List<CollectionType>> GetDocument(IcdType type, int limit,int skip, DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate),
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, type.ToString() + Consts.ARCHIVE_ICD_ADDON)
                );
            return await GetDocumentBase(limit, skip, filter);
        }       
        public async Task<List<CollectionType>> GetDocument( DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate));
            return await GetDocumentBase(0, 0, filter);
        }       
        public async Task<List<CollectionType>> GetDocument( IcdType icdType,DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.RealTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.RealTime, endDate),
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, icdType.ToString() + Consts.ARCHIVE_ICD_ADDON));

            return await GetDocumentBase(0, 0, filter);
        }
        public async Task<(DateTime, DateTime)> GetDateRange(IcdType icdType)
        {
            ProjectionDefinition<CollectionType> projection = Builders<CollectionType>.Projection.Exclude(Consts.ARCHIVE_ID_EXCLUDE);
            CollectionType firstDate =  await _collection.Find(Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, icdType.ToString() + Consts.ARCHIVE_ICD_ADDON)).Limit(1).Sort(Builders<CollectionType>.Sort.Ascending("RealTime")).Project<CollectionType>(projection).FirstOrDefaultAsync();
            CollectionType lastDate =  await _collection.Find(Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, icdType.ToString() + Consts.ARCHIVE_ICD_ADDON)).Limit(1).Sort(Builders<CollectionType>.Sort.Descending("RealTime")).Project<CollectionType>(projection).FirstOrDefaultAsync();

            return (firstDate.RealTime, lastDate.RealTime);
        }        
        public async Task<(DateTime, DateTime)> GetDateRange()
        {
            ProjectionDefinition<CollectionType> projection = Builders<CollectionType>.Projection.Exclude(Consts.ARCHIVE_ID_EXCLUDE);
            CollectionType firstDate =  await _collection.Find(FilterDefinition<CollectionType>.Empty).Limit(1).Sort(Builders<CollectionType>.Sort.Ascending("RealTime")).Project<CollectionType>(projection).FirstOrDefaultAsync();
            CollectionType lastDate =  await _collection.Find(FilterDefinition<CollectionType>.Empty).Limit(1).Sort(Builders<CollectionType>.Sort.Descending("RealTime")).Project<CollectionType>(projection).FirstOrDefaultAsync();

            return (firstDate.RealTime, lastDate.RealTime);
        }
    }
}
