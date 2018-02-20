#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public interface IValuedArgument : IArgument
    {
        object DefaultValue { get; }
        bool UseDefWhenNoArg { get; }
        object[] AllowedValues { get; }
        string RegularExpression { get; }


        IFormatProvider Culture { get; }
    }
}