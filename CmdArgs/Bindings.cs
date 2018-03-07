#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal class Bindings<T> where T : new()
    {
        bool AllowUnknownArgument { get; }


        public Bindings(bool allowUnknownArgument, CmdArgsParser<T> cmdArgsParser)
        {
            AllowUnknownArgument = allowUnknownArgument;
            _cmdArgsParser = cmdArgsParser;
        }


        CmdArgsParser<T> _cmdArgsParser;


        public List<Binding<T>> bindings;

        public Res<T> Args;


        #region find biding
        public Binding<T> FindBinding(char shortName)
        {
            Binding<T> rv = bindings.FirstOrDefault(x => x.Is(shortName));
            return rv;
        }


        public Binding<T> FindBinding(string longName)
        {
            Binding<T> rv = bindings.FirstOrDefault(x => x.Is(longName));
            return rv;
        }


        public Binding<T> FindBinding(string name, bool isLongName) =>
            isLongName ? FindBinding(name) : FindBinding(name[0]);


        public Binding<T> FindBindingMin(string argName)
        {
            int lastI = argName.IndexOf("=");
            if (lastI < 0) lastI = argName.Length - 1;
            else lastI--;

            for (var i = 0; i <= lastI; i++)
            {
                string testArgLongName = argName.Substring(0, i + 1);
                Binding<T> b = FindBinding(testArgLongName);
                if (b != null) return b;
            }
            return null;
        }
        #endregion


        public void SetVal(char shortName, string[] values)
        {
            if (!Argument.CheckShortName(shortName))
                throw new CmdException($"ShortName [{shortName}] is not allowed");
            Binding<T> binding = FindBinding(shortName);
            SetVal(binding, values, shortName.ToString());
        }


        public void SetVal(string longName, string[] values)
        {
            if (!Argument.CheckLongName(longName[0]))
                throw new CmdException($"LongName [{longName}] is not allowed");
            Binding<T> binding = FindBinding(longName);
            SetVal(binding, values, longName);
        }


        public void SetVal(Binding<T> binding, string[] values, string nameUnknown)
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
    }
}