using Redis.Attribute;
using Redis.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Model
{
    [TimeToExpire(SecondsToExpire = 60)]
    public class UserRedisModel : IRedisModel
    {
        public string Key => $"user:{UserId}";

        public string Field => null;

        public object Value { get; set; }
                
        private int UserId { get; set; }

        private UserRedisModel()
        {
        }

        public UserRedisModel(UserModel model)
        {
            UserId = model.UserId;
            Value = model;
        }
    }
}
