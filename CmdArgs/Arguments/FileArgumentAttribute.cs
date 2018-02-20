#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class FileArgumentAttribute : ValuedArgumentAttribute
    {
        #region ctors
        public FileArgumentAttribute(char shortName)
            : base(new FileArgument(shortName)) { }


        public FileArgumentAttribute(string longName)
            : base(new FileArgument(longName)) { }


        public FileArgumentAttribute(char shortName, string longName)
            :
            base(new FileArgument(shortName, longName)) { }


        public FileArgumentAttribute(char shortName, string longName, string description)
            : base(new FileArgument(shortName, longName, description)) { }
        #endregion


        public bool MustExists
        {
            get => ((FileArgument) Argument).MustExists;
            set => ((FileArgument) Argument).MustExists = value;
        }
    }
}