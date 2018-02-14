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
    public class CmdException : CmdArgsException
    {
        public CmdException(string message)
            : base(message) { }


        //[StringFormatMethod("message")]
        public CmdException(string message, params object[] args)
            : this(string.Format(message, args)) { }


        public CmdException(string message, Exception innerException)
            : base(message, innerException) { }


        protected CmdException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }


        /// <exception cref="CmdException"></exception>
        //[StringFormatMethod("message")]
        //[TerminatesProgram]
        public static void Throw(string message, params object[] args)
        {
            throw new CmdException(message, args);
        }
    }
}