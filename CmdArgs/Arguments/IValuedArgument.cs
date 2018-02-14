﻿#region usings
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
        bool ValueIsCollection { get; }

        IFormatProvider Culture { get; }
    }
}