using AvtoMigBussines.RedisResults.Detailing.Interface;
using AvtoMigBussines.RedisResults.RedisCacheDemo.Cache;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace AvtoMigBussines.RedisResults.Detailing.Service
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;

        public CacheService()
        {
            _db = ConnectionHelper.Connection.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public void SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var jsonData = JsonConvert.SerializeObject(value, settings);
            _db.StringSet(key, jsonData, expirationTime - DateTimeOffset.Now);
        }


        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }
    }
}
