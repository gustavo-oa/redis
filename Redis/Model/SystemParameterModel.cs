using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Model
{
    public enum ESystemParameterType { Log = 0, ApiUrl = 1, Folders = 2 }
    public class SystemParameterModel
    {
        public int ParameterId { get; set; }
        public ESystemParameterType Type { get; set; }
        public List<string> Parametros { get; set; }
    }
}
