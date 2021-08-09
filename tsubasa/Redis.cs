using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace tsubasa
{
    public class Redis : IDisposable
    {
        private ConnectionMultiplexer _redis = null;
        private string _server = null;
        private int _dbIndex = 0;

        public Redis(string server) => _server = server;

        public bool Connect()
        {
            _redis = ConnectionMultiplexer.Connect(_server);
            return _redis != null;
        }

        public async Task<bool> ConnectAsync()
        {
            _redis = await ConnectionMultiplexer.ConnectAsync(_server);
            return _redis != null;
        }

        public bool Connected()
        {
            return _redis != null;
        }

        public bool Set(string key, string val)
        {
            var db = _redis.GetDatabase(_dbIndex);
            return db.StringSet(key, val);
        }

        public async Task<bool> SetAsync(string key, string val)
        {
            var db = _redis.GetDatabase(_dbIndex);
            return await db.StringSetAsync(key, val);
        }

        public string Get(string key,Encoding encoding = null)
        {
            var db = _redis.GetDatabase(_dbIndex);
            byte[] value = db.StringGet(key);
            Encoding enc = encoding ?? Encoding.UTF8;
            return enc.GetString(value);
        }

        public async Task<string> GetAsync(string key, Encoding encoding = null)
        {
            var db = _redis.GetDatabase(_dbIndex);
            byte[] value = await db.StringGetAsync(key);
            Encoding enc = encoding ?? Encoding.UTF8;
            return enc.GetString(value);
        }

        public void Dispose()
        {
            _redis.Dispose();
            _redis = null;
        }
    }
}
