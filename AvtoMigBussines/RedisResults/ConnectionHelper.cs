using StackExchange.Redis;

namespace AvtoMigBussines.RedisResults
{
    using StackExchange.Redis;
    using System;

    namespace RedisCacheDemo.Cache
    {
        public static class ConnectionHelper
        {
            private static readonly ConnectionMultiplexer redis;

            static ConnectionHelper()
            {
                redis = ConnectionMultiplexer.Connect("redis-15128.c282.east-us-mz.azure.redns.redis-cloud.com:15128,password=d35sR7ZnX4H3YD4N29q1cDJubE4bO6Bu");
            }

            public static ConnectionMultiplexer Connection
            {
                get
                {
                    return redis;
                }
            }
        }
    }



}
