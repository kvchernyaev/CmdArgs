using System;
using System.Collections.Generic;
using System.Text;

namespace CmdArgs
{
    public interface ICheckAndPrepare<TArgs> where TArgs : new()
    {
        /// <summary>
        /// Should throw CmdException if validation is failed.
        /// </summary>
        void CheckAndPrepare(Res<TArgs> parsed);
    }
}
