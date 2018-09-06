using System;
using System.Collections.Generic;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliveryWService.Services
{
    public class DBConnectionMongo
    {
        private DBConnectionMongo()
        {
        }

        private MongoClient client = null;
        public MongoClient Client
        {
            get { return client; }
        }

        private static DBConnectionMongo _instance = null;
        public static DBConnectionMongo Instance()
        {
            if (_instance == null)
                _instance = new DBConnectionMongo();
            return _instance;
        }

        public MongoClient GetClient()
        {
            if (client == null)
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["mongoDBName"]))
                    return null;
                string connstring = string.Format("mongodb://{0}:{1}@{2}:{3}/{4}", ConfigurationManager.AppSettings["mongoDBuser"], ConfigurationManager.AppSettings["mongoDBPassword"], ConfigurationManager.AppSettings["mongoDBServer"], ConfigurationManager.AppSettings["mongoDBPort"], ConfigurationManager.AppSettings["mongoDBName"]);
                //string connstring = string.Format("mongodb://{0}:{1}@{2}:{3}", ConfigurationManager.AppSettings["mongoDBuser"], ConfigurationManager.AppSettings["mongoDBPassword"], ConfigurationManager.AppSettings["mongoDBServer"], ConfigurationManager.AppSettings["mongoDBPort"]);
                //connstring = "mongodb://test:delivery95@ds243812.mlab.com:43812/deliverymanagerdb";
                
                client = new MongoClient(connstring);
                var server = client.StartSession();
                return client;
            }
            return client;
        }
    }

    public class Sequence
    {
        [BsonId]
        public ObjectId _Id { get; set; }
        public string Name { get; set; }
        public long Value { get; set; }

        public void Insert(IMongoDatabase database)
        {
            var collection = database.GetCollection<Sequence>("sequence");
            collection.InsertOne(this);
        }

        internal static long GetNextSequenceValue(string sequenceName, IMongoDatabase database)
        {
            var collection = database.GetCollection<Sequence>("sequence");
            var filter = Builders<Sequence>.Filter.Eq(a => a.Name, sequenceName);
            var update = Builders<Sequence>.Update.Inc(a => a.Value, 1);
            var sequence = collection.FindOneAndUpdate(filter, update);

            return sequence.Value;
        }
    }

}