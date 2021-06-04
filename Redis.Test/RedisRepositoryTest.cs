using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redis.Model;
using Redis.Repository;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Redis.Test
{
    [TestClass]
    public class RedisRepositoryTest
    {
        private RedisRepository _conn = new RedisRepository();

        [TestMethod]
        public void SetTest()
        {
            var model = new UserModel
            {
                UserId = 123,
                UserName = "USER123"
            };
            var modelRedis = new UserRedisModel(model);

           Assert.IsTrue(_conn.Set(modelRedis));
        }

        [TestMethod]
        public void GetTest()
        {
            var model = new UserModel
            {
                UserId = 123,
                UserName = "USER321"
            };
            var modelRedis = new UserRedisModel(model);

            Assert.IsTrue(_conn.Set(modelRedis));
            var result = _conn.Get<UserRedisModel, UserModel>(modelRedis);
            Assert.AreEqual(model.UserId, result.UserId);
            Assert.AreEqual(model.UserName, result.UserName);
        }

        [TestMethod]
        public void HSETTest()
        {
            var model = new SystemParameterModel
            {
                ParameterId = 1,
                Type = ESystemParameterType.ApiUrl,
                Parametros = new List<string> { "http://localhost", "https://btgpactual.com" }
            };

            var redisModel = new SystemParametersRedisModel(model);
            Assert.IsTrue(_conn.Set(redisModel));
        }

        [TestMethod]
        public void HGETTest()
        {
            var model = new SystemParameterModel
            {
                ParameterId = 2,
                Type = ESystemParameterType.Log,
                Parametros = new List<string> { "folder1", "folder1" }
            };

            var redisModel = new SystemParametersRedisModel(model);
            Assert.IsTrue(_conn.Set(redisModel));
            
            var result = _conn.Get<SystemParametersRedisModel, SystemParameterModel>(redisModel);
            
            Assert.AreEqual(model.ParameterId, result.ParameterId);
            Assert.AreEqual(model.Type, result.Type);
        }

        [TestMethod]
        public void EvalTest()
        {
            var model = new UserModel
            {
                UserId = 123,
                UserName = "GUSTAVO"
            };
            var modelRedis = new UserRedisModel(model);

            Assert.IsTrue(_conn.Set(modelRedis));
            var script = "return redis.call('get', KEYS[1])";
            var result = _conn.Eval<UserModel>(script, new RedisKey[] { modelRedis.Key });
            Assert.AreEqual(model.UserId, result.UserId);
            Assert.AreEqual(model.UserName, result.UserName);
        }

    }
}
