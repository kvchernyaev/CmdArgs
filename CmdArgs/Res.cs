#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion



namespace CmdArgs
{
    public class Res<T> where T : new()
    {
        public T Args { get; set; } = new T();


        public List<string> AdditionalArguments { get; set; }
            = new List<string>();


        public List<Tuple<string, string[]>> UnknownArguments { get; set; }
            = new List<Tuple<string, string[]>>();
    }
}