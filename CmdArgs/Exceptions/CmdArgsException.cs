#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public abstract class CmdArgsException : Exception
    {
        public CmdArgsException(string message)
            : base(message) { }


        //[StringFormatMethod("message")]
        public CmdArgsException(string message, params object[] args)
            : this(string.Format(message, args)) { }


        public CmdArgsException(string message, Exception innerException)
            : base(message, innerException) { }


        protected CmdArgsException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}