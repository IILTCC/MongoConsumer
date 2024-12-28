﻿using MongoConsumerLibary.MongoConnection.Enums;
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
        public CollectionEntity(IMongoDatabase database,string collectionName)
        {
            _collection = database.GetCollection<CollectionType>(collectionName);
            AddIndex();
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
        private async Task<List<CollectionType>> GetDocumentAbs(int limit,int skip, FilterDefinition<CollectionType> filter)
        {
            var projection = Builders<CollectionType>.Projection.Exclude(Consts.ARCHIVE_ID_EXCLUDE);
            return await _collection.Find(filter).Limit(limit).Skip(skip).Project<CollectionType>(projection).ToListAsync();
        }
        public async Task<List<CollectionType>> GetDocument(int limit,int skip, DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
            Builders<CollectionType>.Filter.Gte(document => document.InsertTime, startDate),
            Builders<CollectionType>.Filter.Lt(document => document.InsertTime, endDate)
            );
            return await GetDocumentAbs(limit, skip,filter);
        }        

        public async Task<List<CollectionType>> GetDocument(IcdType type,int limit,DateTime startDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, type.ToString() + Consts.ARCHIVE_ICD_ADDON),
                Builders<CollectionType>.Filter.Gte(document => document.InsertTime,startDate));
            return await GetDocumentAbs(limit,0,filter);
        }

        public async Task<List<CollectionType>> GetDocument(IcdType type, int limit, DateTime startDate,DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.InsertTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.InsertTime, endDate),
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, type.ToString() + Consts.ARCHIVE_ICD_ADDON)
                );
            return await GetDocumentAbs(limit,0,filter);
        }

        public async Task<List<CollectionType>> GetDocument(IcdType type, int limit,int skip, DateTime startDate, DateTime endDate)
        {
            FilterDefinition<CollectionType> filter = Builders<CollectionType>.Filter.And(
                Builders<CollectionType>.Filter.Gte(document => document.InsertTime, startDate),
                Builders<CollectionType>.Filter.Lt(document => document.InsertTime, endDate),
                Builders<CollectionType>.Filter.Eq(Consts.ARCHIVE_ICD_PARAMETER, type.ToString() + Consts.ARCHIVE_ICD_ADDON)
                );
            return await GetDocumentAbs(limit, skip, filter);
        }
    }
}
