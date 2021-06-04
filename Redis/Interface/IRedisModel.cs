using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Interface
{
    public interface IRedisModel
    {
        string Key { get; }
        string Field { get; }
        object Value { get; set; }
    }
}
