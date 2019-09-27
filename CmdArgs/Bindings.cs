#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal class Bindings<TArgs> where TArgs : new()
    {
        bool AllowUnknownArgument { get; }


        public Bindings(bool allowUnknownArgument, CmdArgsParser<TArgs> cmdArgsParser, Res<TArgs> args,
            List<Binding<TArgs>> __bindings)
        {
            AllowUnknownArgument = allowUnknownArgument;
            _cmdArgsParser = cmdArgsParser;

            Args = args;
            bindings = __bindings;
            foreach (Binding<TArgs> b in bindings) b.bs = this;
        }


        CmdArgsParser<TArgs> _cmdArgsParser;


        public readonly List<Binding<TArgs>> bindings;

        public readonly Res<TArgs> Args;


        #region find biding
        public Binding<TArgs> FindBinding(char shortName)
        {
            Binding<TArgs> rv = bindings.FirstOrDefault(x => x.Is(shortName));
            return rv;
        }


        public Binding<TArgs> FindBinding(string longName)
        {
            Binding<TArgs> rv = bindings.FirstOrDefault(x => x.Is(longName));
            return rv;
        }


        public Binding<TArgs> FindBinding(string name, bool isLongName) =>
            isLongName ? FindBinding(name) : FindBinding(name[0]);


        public Binding<TArgs> FindBindingMin(string argName)
        {
            int lastI = argName.IndexOf("=");
            if (lastI < 0) lastI = argName.Length - 1;
            else lastI--;

            for (var i = 0; i <= lastI; i++)
            {
                string testArgLongName = argName.Substring(0, i + 1);
                Binding<TArgs> b = FindBinding(testArgLongName);
                if (b != null) return b;
            }
            return null;
        }
        #endregion


        #region set val
        public void SetVal(char shortName, string[] values)
        {
            if (!Argument.CheckShortName(shortName))
                throw new CmdException($"ShortName [{shortName}] is not allowed");
            Binding<TArgs> binding = FindBinding(shortName);
            SetVal(binding, values, shortName.ToString());
        }


        public void SetVal(string longName, string[] values)
        {
            if (!Argument.CheckLongName(longName[0]))
                throw new CmdException($"LongName [{longName}] is not allowed");
            Binding<TArgs> binding = FindBinding(longName);
            SetVal(binding, values, longName);
        }


        public void SetVal(Binding<TArgs> binding, string[] values, string nameUnknown)
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


        void SetParsedVal(Binding<TArgs> bSource)
        {
            Binding<TArgs> bTarget = this.bindings.FirstOrDefault(b => b.IsSame(bSource));
            if (bTarget == null) throw new Exception($"SetParsedVal");

            bTarget.SetParsedVal(bSource);
        }
        #endregion


        internal void Merge(Bindings<TArgs> bsSource)
        {
            if (bsSource.Args.AdditionalArguments != null)
                this.Args.AdditionalArguments.AddRange(bsSource.Args.AdditionalArguments);
            if (bsSource.Args.UnknownArguments != null)
                this.Args.UnknownArguments.AddRange(bsSource.Args.UnknownArguments);

            foreach (Binding<TArgs> bSource in bsSource.bindings.Where(b => b.AlreadySet))
                this.SetParsedVal(bSource);
            // todo collections, UnstrictlyConf
        }
    }
}