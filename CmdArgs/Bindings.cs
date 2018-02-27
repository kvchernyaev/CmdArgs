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


        public void SetVal(char shortName, string[] values)
        {
            if (!Argument.CheckShortName(shortName))
                throw new CmdException($"ShortName [{shortName}] is not allowed");
            Binding binding = bindings.FirstOrDefault(x => x.Is(shortName));
            if (binding == null)
                if (AllowUnknownArgument)
                    Args.UnknownArguments.Add(
                        new Tuple<string, string[]>(shortName.ToString(), values));
                else
                    throw new CmdException($"Unknown parameter: {shortName}");
            else
                binding.SetVal(values);
        }


        public void SetVal(string longName, string[] values)
        {
            Binding binding = bindings.FirstOrDefault(x => x.Is(longName));
            if (binding == null)
                if (AllowUnknownArgument)
                    Args.UnknownArguments.Add(
                        new Tuple<string, string[]>(longName, values));
                else
                    throw new CmdException($"Unknown parameter: {longName}");
            else
                binding.SetVal(values);
        }
    }
}