#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class UnstrictlyConfArgument : Argument
    {
        #region ctors
        public UnstrictlyConfArgument(char shortName)
            : base(shortName) { }


        public UnstrictlyConfArgument(string longName)
            : base(longName) { }


        public UnstrictlyConfArgument(char shortName, string longName)
            : base(shortName, longName) { }


        public UnstrictlyConfArgument(char shortName, string longName, string description)
            : base(shortName, longName, description) { }
        #endregion


        public override Type ValueType { get; set; }


        internal override bool CanHaveValue => true;


        public override void CheckFieldType(Type fieldType)
        {
            throw new NotImplementedException();
        }
    }
}