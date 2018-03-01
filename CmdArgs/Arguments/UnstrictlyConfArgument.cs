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
            Binding b = islong
                ? bindings.FindBindingMin(argName)
                : bindings.FindBinding(argName[0]);

            if (b != null && b.Argument is UnstrictlyConfArgument uca)
            {
                string nameval = argName.Substring(islong ? b.Argument.LongName.Length : 1);
                argName = islong
                    ? argName.Substring(0, b.Argument.LongName.Length)
                    : argName[0].ToString();

                values = new[] {nameval};
                return b;
            }

            values = null;
            return null;
        }
    }
}