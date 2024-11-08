﻿using MongoDB.Driver;
using Shared.Services.Abstractions;

namespace Shared.Services.Concretes
{
    public class MongoDBService : IMongoDBService
    {
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            IMongoDatabase database = GetDatabase();
            return database.GetCollection<T>(collectionName);
        }

        public IMongoDatabase GetDatabase(string databaseName = "ProductDB", string connectionString = "mongodb://localhost:27017")
        {
            MongoClient client = new(connectionString);
            return client.GetDatabase(databaseName);
        }
    }
}