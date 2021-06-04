using Newtonsoft.Json;
using Redis.Attribute;
using Redis.Interface;
using Redis.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Repository
{
    public class RedisRepository
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private string ConnectionString
        {
            get
            {
                return "localhost:6379,allowAdmin=true";
            }
        }
        private ConnectionMultiplexer Connection
        {
            get
            {
                if (_lazyConnection == null)
                    _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConnectionString));

                return _lazyConnection.Value;
            }
        }
        private IDatabase Database 
        { 
            get
            {
                return Connection.GetDatabase();
            }
        }
        
        private string GetKey(IRedisModel obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.Key))
                throw new InvalidOperationException("Key must be defined");

            return obj.Key;
        }

        private TimeSpan GetTimeToExpire<T>()
        {
            var toExpire = (typeof(T).GetCustomAttributes(typeof(TimeToExpireAttribute), true).Cast<TimeToExpireAttribute>().FirstOrDefault() ?? new TimeToExpireAttribute());
            return TimeSpan.FromSeconds(toExpire.SecondsToExpire);
        }

        public IEnumerable<string> Find(string keyPattern)
        {
            var server = Connection.GetServer(endpoint: Connection.GetEndPoints().FirstOrDefault());

            foreach (var s in server.Keys(pattern: keyPattern, flags: CommandFlags.PreferSlave))
                yield return s;
        }

        public R Get<T, R>(T model) where T : IRedisModel
        {
           
            var key = GetKey(model);
            var conn = Connection.GetDatabase();

            var result = string.IsNullOrEmpty(model.Field)
                ? conn.StringGet(key, CommandFlags.PreferSlave)
                : conn.HashGet(key, model.Field, CommandFlags.PreferSlave);

            return string.IsNullOrEmpty(result)
                ? default(R)
                : JsonConvert.DeserializeObject<R>(result);
         
        }

        public IEnumerable<R> HGet<R>(string key, RedisValue[] fields = null)
        {
            var result = new List<R>();
            var conn = Connection.GetDatabase();

            var values = fields != null && fields.Any(x => x.HasValue)
                ? conn.HashGet(key, fields, CommandFlags.PreferSlave).Select(x => x)
                : conn.HashGetAll(key, CommandFlags.PreferSlave).Select(x => x.Value);

            foreach (var x in values.Where(x => x.HasValue))
                result.Add(JsonConvert.DeserializeObject<R>(x.ToString()));

            return result;
        }

        public bool Set<T>(T model) where T : IRedisModel
        {            
            var timeToExpire = this.GetTimeToExpire<T>();

            var conn = Connection.GetDatabase();
                
            if (model.Value == null)
                throw new InvalidOperationException("Value must be defined");

            var key = GetKey(model);
            var value = JsonConvert.SerializeObject(model.Value);
            var hasField = !string.IsNullOrEmpty(model.Field);
            
            if (hasField)
            {
                conn.HashSet(key, model.Field, value);
                conn.KeyExpireAsync(key, timeToExpire);
            }                    
            else
                conn.StringSet(key, value, timeToExpire);
  
            return true;
        }

        public bool Del<T>(T model) where T : IRedisModel
        {          
            var conn = Connection.GetDatabase();
            var key = GetKey(model);
            return conn.KeyDelete(key);                       
        }

        public T Eval<T>(string script, RedisKey[] keys) 
        {
            var result = Database.ScriptEvaluate(script, keys);
            if (result.IsNull)
                return default(T);

            return JsonConvert.DeserializeObject<T>(result.ToString());
        }

        public void Expire<T>(TimeSpan time, T model) where T : IRedisModel
        {           
            var conn = Connection.GetDatabase();
            var key = GetKey(model);
            conn.KeyExpire(key, time);       
        }

        public TimeSpan? TimeToLive<T>(T model) where T : IRedisModel
        {
            var key = GetKey(model);
            var conn = Connection.GetDatabase();

            return conn.KeyTimeToLive(key);
        }

    }
}
