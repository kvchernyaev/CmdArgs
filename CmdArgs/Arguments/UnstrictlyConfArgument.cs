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


        public override bool AllowMultiple
        {
            get => true;
            set => throw new Exception(
                $"Argument [{Name}] - argument of type {this.GetType().Name} does not support setter for AllowMultiple")
            ;
        }


        public override Type ValueType
        {
            get => typeof(UnstrictlyConf);
            set => throw new Exception(
                $"{nameof(UnstrictlyConfArgument)}.{nameof(ValueType)} - setter is prohibited");
        }


        internal override bool CanHaveValue => true;


        public override void CheckFieldType(Type fieldType)
        {
            if (fieldType != typeof(UnstrictlyConf))
                throw new ConfException(
                    $"field type must be {ValueType.Name} it but {fieldType.Name} provided");
        }


        internal static Binding IsSyntax<T>(ref string argName, Bindings<T> bindings, bool islong,

            out string[] values)
        {
            Binding b = null;
            if (islong)
            {
                int lastI = argName.IndexOf("=");
                if (lastI < 0) lastI = argName.Length - 1;
                else lastI--;

                for (var i = 0; i <= lastI; i++)
                {
                    string testArgName = argName.Substring(0, i + 1);
                    b = bindings.FindBinding(testArgName);
                    if (b != null) break;
                }
            }
            else
                b = bindings.FindBinding(argName[0]);

            if (b != null && b.Argument is UnstrictlyConfArgument uca)
            {
                string val = argName.Substring(islong ? b.Argument.LongName.Length : 1);
                argName = islong
                    ? argName.Substring(0, b.Argument.LongName.Length)
                    : argName[0].ToString();

                values = new[] {val};
                return b;
            }

            values = null;
            return null;
        }
    }
}