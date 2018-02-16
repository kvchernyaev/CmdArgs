#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class SwitchArgument : Argument
    {
        #region ctors
        public SwitchArgument(char shortName)
            : base(shortName) { }


        public SwitchArgument(string longName)
            : base(longName) { }


        public SwitchArgument(char shortName, string longName)
            : base(shortName, longName) { }


        public SwitchArgument(char shortName, string longName, string description)
            : base(shortName, longName, description) { }
        #endregion


        public override Type ValueType
        {
            get => typeof(bool);
            set => throw new Exception(
                $"{nameof(SwitchArgument)}.{nameof(ValueType)} - setter is prohibited");
        }


        internal override bool CanHaveValue => false;


        public override void CheckFieldType(Type fieldType)
        {
            if (fieldType != typeof(bool))
                throw new ConfException(
                    $"Argument [{Name}]: field must be of type bool, but it is of type {fieldType.Name}");
        }
    }
}