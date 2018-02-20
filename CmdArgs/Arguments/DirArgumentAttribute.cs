#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class DirArgumentAttribute : ValuedArgumentAttribute
    {
        #region ctors
        public DirArgumentAttribute(char shortName)
            : base(new DirArgument(shortName)) { }


        public DirArgumentAttribute(string longName)
            : base(new DirArgument(longName)) { }


        public DirArgumentAttribute(char shortName, string longName)
            :
            base(new DirArgument(shortName, longName)) { }


        public DirArgumentAttribute(char shortName, string longName, string description)
            : base(new DirArgument(shortName, longName, description)) { }
        #endregion


        public bool MustExists
        {
            get => ((DirArgument) Argument).MustExists;
            set => ((DirArgument) Argument).MustExists = value;
        }
    }
}