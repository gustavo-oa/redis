using Redis.Attribute;
using Redis.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Model
{
    [TimeToExpire(SecondsToExpire = 600)]
    public class SystemParametersRedisModel : IRedisModel
    {
        public string Key => "SystemParameters";

        public string Field => Type.ToString();

        public object Value { get; set; }
        
        private ESystemParameterType Type { get; set; }

        private SystemParametersRedisModel()
        {

        }
        
        public SystemParametersRedisModel(SystemParameterModel model)
        {
            Type = model.Type;
            Value = model;
        }

    }
}
