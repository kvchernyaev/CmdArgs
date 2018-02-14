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
    public class ConfException : CmdArgsException
    {
        public ConfException(string message)
            : base(message) { }


        //[StringFormatMethod("message")]
        public ConfException(string message, params object[] args)
            : this(string.Format(message, args)) { }


        public ConfException(string message, Exception innerException)
            : base(message, innerException) { }


        protected ConfException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }


        /// <exception cref="ConfException"></exception>
        //[StringFormatMethod("message")]
        //[TerminatesProgram]
        public static void Throw(string message, params object[] args)
        {
            throw new ConfException(message, args);
        }
    }
}