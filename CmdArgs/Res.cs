#region usings
using System;
using System.Collections.Generic;
#endregion



namespace CmdArgs
{
    public class Res<T> where T : new()
    {
        public T Args { get; set; }


        public List<string> AdditionalArguments { get; set; }
            = new List<string>();


        public List<Tuple<string, string[]>> UnknownArguments { get; set; }
            = new List<Tuple<string, string[]>>();


        public void Merge(Res<T> source)
        {

        }
    }
}