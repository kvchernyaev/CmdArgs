#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal class Bindings<T>
    {
        bool AllowUnknownArgument { get; }


        public Bindings(bool allowUnknownArgument)
        {
            AllowUnknownArgument = allowUnknownArgument;
        }


        public List<Binding> bindings;

        public Res<T> Args;


        public Binding FindBinding(char shortName)
        {
            Binding rv = bindings.FirstOrDefault(x => x.Is(shortName));
            return rv;
        }


        public Binding FindBinding(string name, bool isLongName) =>
            isLongName ? FindBinding(name) : FindBinding(name[0]);


        public Binding FindBinding(string longName)
        {
            Binding rv = bindings.FirstOrDefault(x => x.Is(longName));
            return rv;
        }


        public void SetVal(char shortName, string[] values)
        {
            if (!Argument.CheckShortName(shortName))
                throw new CmdException($"ShortName [{shortName}] is not allowed");
            Binding binding = FindBinding(shortName);
            SetVal(binding, values, shortName.ToString());
        }


        public void SetVal(Binding binding, string[] values, string nameUnknown)
        {
            if (binding == null)
                if (AllowUnknownArgument)
                    Args.UnknownArguments.Add(
                        new Tuple<string, string[]>(nameUnknown, values));
                else
                    throw new CmdException($"Unknown parameter: {nameUnknown}");
            else
                binding.SetVal(values);
        }


        public void SetVal(string longName, string[] values)
        {
            if (!Argument.CheckLongName(longName[0]))
                throw new CmdException($"LongName [{longName}] is not allowed");
            Binding binding = FindBinding(longName);
            SetVal(binding, values, longName);
        }
    }
}