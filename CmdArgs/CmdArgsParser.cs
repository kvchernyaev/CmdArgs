#region usings
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class CmdArgsParser<TArgs> where TArgs : new()
    {
        public IFormatProvider Culture { get; private set; } = CultureInfo.InvariantCulture;
        // ReSharper disable MemberCanBePrivate.Global


        #region configuration
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        public bool LongNameIgnoreCase { get; set; } = true;
        public bool AllowUnknownArguments { get; set; } = true;


        public bool UseOnlyEqualitySyntax { get; set; } = false;


        public bool AllowAdditionalArguments { get; set; } = true;
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        #endregion


        public CmdArgsParser() { }


        public CmdArgsParser(IFormatProvider culture)
        {
            Culture = culture;
        }


        public static Res<TArgs> Parse(string[] args)
        {
            var p = new CmdArgsParser<TArgs>();
            Res<TArgs> res = p.ParseCommandLine(args);
            return res;
        }


        public Res<TArgs> ParseCommandLine(string[] args)
        {
            var rv = new Res<TArgs>();
            ParseCommandLine(args, rv);
            return rv;
        }


        public void ParseCommandLine(string[] args, Res<TArgs> res)
        {
            Bindings<TArgs> bindings = ParseCommandLineEgoist(args, res);

            foreach (Binding<TArgs> binding in bindings.bindings.Where(x => !x.AlreadySet))
            {
                Argument a = binding.Argument;
                if (a is ValuedArgument va)
                    if (va.UseDefWhenNoArg && va.DefaultValue != null)
                        binding.SetVal(null);

                if (!binding.AlreadySet && a.Mandatory)
                    throw new CmdException(
                        $"Argument [{a.Name}] is mandatory but is not set");
            }

            if (res.Args is ICheckAndPrepare<TArgs>)
                ((ICheckAndPrepare<TArgs>)res.Args).CheckAndPrepare(res);
        }


        internal Bindings<TArgs> ParseCommandLineEgoist(string[] args, Res<TArgs> res)
        {
            if (res.AdditionalArguments == null)
                res.AdditionalArguments = new List<string>();
            if (res.UnknownArguments == null)
                res.UnknownArguments = new List<Tuple<string, string[]>>();

            var bindings = new Bindings<TArgs>(AllowUnknownArguments, this, res,
                ExtractArgumentAttributes(res));
            CheckCongregate(bindings.bindings);

            for (var i = 0; i < args.Length;)
            {
                string arg = args[i];

                if (!TryParseArgument(args, i, out i, bindings))
                    if (AllowAdditionalArguments)
                        res.AdditionalArguments.Add(arg);
                    else
                        throw new CmdException(
                            $"Unnamed arguments is prohibited ({arg}). Use {nameof(AllowAdditionalArguments)} setting?");
            }
            return bindings;
        }


        bool TryParseArgument(string[] args, int curI, out int nextI, Bindings<TArgs> bindings)
        {
            string arg = args[curI];
            nextI = curI + 1;

            if (IsArgLong(arg))
            {
                string[] values = GetValues(bindings, args, arg, true, ref nextI,
                    out string name, out Binding<TArgs> b);
                bindings.SetVal(b, values, name);
            }
            else if (IsArgShort(arg))
            {
                string[] values = GetValues(bindings, args, arg, false, ref nextI,
                    out string shortNames, out Binding<TArgs> b);
                if (shortNames.Length == 1)
                    bindings.SetVal(b, values, shortNames[0].ToString());
                else if (shortNames.Length > 1)
                {
                    if (values?.Length > 0)
                        throw new CmdException(
                            $"Values after multiple shortnames can not be processed");

                    foreach (char shortName in shortNames)
                        bindings.SetVal(shortName, null);
                }
                else
                    // "-"
                    throw new CmdException($"Argument [{arg}] can not be processed");
            }
            else return false;

            return true;
        }


        static readonly char[] EqualityModeValSeparators = {';', ',', ' '};


        string[] GetValues(Bindings<TArgs> bindings, string[] args, string arg, bool islong,
            ref int nextI,
            out string argName, out Binding<TArgs> b)
        {
            argName = arg.Substring(islong ? 2 : 1);
            string[] rv;
            if ((b = UnstrictlyConfArgument.IsSyntax(ref argName, bindings, islong, out rv))
                != null)
                return rv;

            argName.SplitPairByEquality(out string name, out string valuesString);
            if (valuesString == null)
            {
                b = bindings.FindBinding(argName, islong);
                if (UseOnlyEqualitySyntax)
                    return null;

                if (b == null || b.Argument is ValuedArgument va && va.ValueIsCollection)
                    rv = args.Skip(nextI).TakeWhile(s => !IsArg(s)).ToArray();
                else
                    rv = args.Skip(nextI).Take(1).TakeWhile(s => !IsArg(s)).ToArray();

                nextI += rv.Length;
                return rv;
            }

            argName = name;
            b = bindings.FindBinding(argName, islong);
            if (b != null && b.Argument is ValuedArgument va1 && va1.ValueIsCollection)
                rv = valuesString.Split(EqualityModeValSeparators,
                    StringSplitOptions.RemoveEmptyEntries);
            else rv = new[] {valuesString};
            return rv;
        }


        static bool IsArg(string arg) => IsArgLong(arg) || IsArgShort(arg);


        static bool IsArgLong(string arg) =>
            arg.StartsWith("--") && arg.Length >= 3 && Argument.CheckLongName(arg[2]);


        static bool IsArgShort(string arg) =>
            arg.StartsWith("-") && arg.Length >= 2 && Argument.CheckShortName(arg[1]);


        void CheckCongregate(List<Binding<TArgs>> binds)
        {
            if (binds == null || binds.Count <= 1)
                return;
            List<IGrouping<string, string>> l = binds.Select(x => x.Argument.LongName)
                .Where(x => !string.IsNullOrEmpty(x))
                .GroupBy(x => x, x => x, LongNameIgnoreCase
                    ? StringComparer.InvariantCultureIgnoreCase
                    : StringComparer.InvariantCulture)
                .ToList()
                .Where(g => g.Count() > 1)
                .ToList();
            if (l.Count > 0)
                throw new ConfException(string.Join("; ",
                    l.Select(x => $"LongName [{x.Key}] is given {x.Count()} times")));

            List<IGrouping<char?, char?>> s = binds.Select(x => x.Argument.ShortName)
                .Where(x => x.HasValue)
                .GroupBy(x => x, x => x)
                .ToList()
                .Where(g => g.Count() > 1)
                .ToList();
            if (s.Count > 0)
                throw new ConfException(string.Join("; ",
                    s.Select(x => $"ShortName [{x.Key}] is given {x.Count()} times")));
        }


        List<Binding<TArgs>> ExtractArgumentAttributes(Res<TArgs> res)
        {
            var rv = new List<Binding<TArgs>>();

            Type confType = res.Args.GetType();
            MemberInfo[] mis = confType.GetFieldsAndProps();

            foreach (MemberInfo mi in mis)
            {
                Argument arg = mi.GetArgument();
                if (arg == null) continue;

                arg.InitAndCheck(mi, this, res.Args);

                rv.Add(new Binding<TArgs>(LongNameIgnoreCase, arg, mi, res, this));
            }

            return rv;
        }
    }
}