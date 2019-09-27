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


        public override void CheckFieldType(Type fieldType)
        {
            if (fieldType != typeof(UnstrictlyConf))
                throw new ConfException(
                    $"field type must be {ValueType.Name} it but {fieldType.Name} provided");
        }


        internal static Binding<TArgs> IsSyntax<TArgs>(ref string argName, Bindings<TArgs> bindings,
            bool islong, out string[] values)
            where TArgs : new()
        {
            Binding<TArgs> b = islong
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


        public override bool Parse(object prevValue, string[] values, out object argVal)
        {
            var rv = false;
            var conf = (UnstrictlyConf) prevValue;
            if (conf == null)
            {
                rv = true;
                conf = new UnstrictlyConf();
            }

            values[0].SplitPairByEquality(out string name, out string value);
            if (string.IsNullOrEmpty(name))
                throw new CmdException(
                    $"Argument [{Name}] : {values[0]} must be with name part");

            conf.Add(new UnstrictlyConf.UnstrictlyConfItem(name, value));

            argVal = conf;
            return rv;
        }


        public void Append(UnstrictlyConf target, UnstrictlyConf source)
        {
            target.AddRange(source);
        }
    }
}