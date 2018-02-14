#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal class Bindings<T>
    {
        public List<Binding> bindings;

        public Res<T> Args;
    }
}