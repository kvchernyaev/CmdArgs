#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion



namespace CmdArgs
{
    public class Res<TArgs> where TArgs : new()
    {
        public TArgs Args { get; set; } = new TArgs();


        public List<string> AdditionalArguments { get; set; }
            = new List<string>();


        public List<Tuple<string, string[]>> UnknownArguments { get; set; }
            = new List<Tuple<string, string[]>>();
    }
}