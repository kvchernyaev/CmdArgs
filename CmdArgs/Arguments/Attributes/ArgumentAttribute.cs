#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class ArgumentAttribute : Attribute, IArgument
    {
        public Argument Argument { get; }


        protected ArgumentAttribute(Argument arg)
        {
            Argument = arg;
        }


        #region delegated argument members
        public string LongName
        {
            get => Argument.LongName;
            set => Argument.LongName = value;
        }


        public char? ShortName
        {
            get => Argument.ShortName;
            set => Argument.ShortName = value;
        }


        public string Description
        {
            get => Argument.Description;
            set => Argument.Description = value;
        }


        public string FullDescription
        {
            get => Argument.FullDescription;
            set => Argument.FullDescription = value;
        }


        public bool Mandatory
        {
            get => Argument.Mandatory;
            set => Argument.Mandatory = value;
        }


        public bool AllowMultiple
        {
            get => Argument.AllowMultiple;
            set => Argument.AllowMultiple = value;
        }


        public string Example
        {
            get => Argument.Example;
            set => Argument.Example = value;
        }
        #endregion
    }
}