﻿#region usings
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
    public class CmdArgsParser
    {
        readonly IFormatProvider _culture = CultureInfo.InvariantCulture;
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
            _culture = culture;
        }


        public Res<T> ParseCommandLine<T>(string[] args) where T : new()
        {
            var rv = new Res<T> {Args = new T()};
            ParseCommandLine(args, rv);
            return rv;
        }


        public void ParseCommandLine<T>(string[] args, Res<T> res)
        {
            if (res.AdditionalArguments == null)
                res.AdditionalArguments = new List<string>();
            if (res.UnknownArguments == null)
                res.UnknownArguments = new List<Tuple<string, string[]>>();
            /*
             * 
            --defaultsFile="core/liquibase_CIS-DB.CIS_custom.properties" ^
            --labels="bus_data OR bus_longtime_data" ^
            --contexts="Light,prod, sync_prod,sync" ^
                update ^
             */
            var bindings = new Bindings<T>(AllowUnknownArguments)
                {
                    Args = res,
                    bindings = ExtractArgumentAttributes(res.Args)
                };
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

            foreach (Binding binding in bindings.bindings.Where(x => !x.AlreadySet))
            {
                Argument a = binding.Argument;
                if (a is ValuedArgument va)
                    if (va.UseDefWhenNoArg && va.DefaultValue != null)
                        binding.SetVal(null);

                if (!binding.AlreadySet && a.Mandatory)
                    throw new CmdException(
                        $"Argument [{a.Name}] is mandatory but is not set");
            }
        }


        bool TryParseArgument<T>(string[] args, int curI, out int nextI, Bindings<T> bindings)
        {
            string arg = args[curI];
            nextI = curI + 1;

            if (IsArgLong(arg))
            {
                string[] values = GetValues(bindings, args, arg, true, ref nextI,
                    out string name, out Binding b);
                bindings.SetVal(b, values, name);
            }
            else if (IsArgShort(arg))
            {
                string[] values = GetValues(bindings, args, arg, false, ref nextI,
                    out string shortNames, out Binding b);
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


        string[] GetValues<T>(Bindings<T> bindings, string[] args, string arg, bool islong,
            ref int nextI,
            out string argName, out Binding b)
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


        void CheckCongregate(List<Binding> binds)
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


        List<Binding> ExtractArgumentAttributes(object target)
        {
            var rv = new List<Binding>();

            Type confType = target.GetType();

            MemberInfo[] fields = confType.GetFields();
            MemberInfo[] properties = confType.GetProperties();

            List<MemberInfo> mis = fields.Concat(properties).ToList();
            List<MemberInfo> miPredicates = mis.Where(x =>
            {
                Type miType = GetFieldType(x);
                return miType.IsGenericType &&
                       miType.GetGenericTypeDefinition() == typeof(Predicate<>);
            }).ToList();

            foreach (MemberInfo mi in mis)
            {
                var attr = (ArgumentAttribute) mi
                    .GetCustomAttributes(typeof(ArgumentAttribute), true)
                    .FirstOrDefault();
                if (attr == null)
                    continue;

                if (attr.Argument is ValuedArgument va)
                {
                    if (va.Culture == null) va.Culture = this._culture;

                    Type fieldType = GetFieldType(mi);
                    va.SetTypeAndCheck(fieldType);

                    // predicates are dependent on type
                    Tuple<List<Delegate>, List<Delegate>> predicates =
                        GetPredicates(miPredicates, mi.Name, va, target);

                    va.ValuePredicatesForCollection = predicates?.Item1;
                    va.ValuePredicatesForOne = predicates?.Item2;

                    // check allowed for predicateone
                    // check def for predicateone, predicatecol
                    va.CheckDefaultAndAllowedValues();
                }
                else
                    attr.Argument.CheckFieldType(GetFieldType(mi));

                rv.Add(new Binding(LongNameIgnoreCase, attr.Argument, mi, target));
            }

            return rv;
        }


        static bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object))
                return false;
            if (typeToCheck == null)
                return false;
            if (typeToCheck.IsGenericType &&
                typeToCheck.GetGenericTypeDefinition() == genericType)
                return true;
            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }


        static Tuple<List<Delegate>, List<Delegate>>
            GetPredicates(List<MemberInfo> allPredicates, string fieldName,
                ValuedArgument va, object target)
        {
            var rv = new Tuple<List<Delegate>, List<Delegate>>(
                new List<Delegate>(), new List<Delegate>());
            foreach (MemberInfo mi in allPredicates)
            {
                if (!mi.Name.StartsWith(fieldName + "_Predicate")) // it is for other field
                    continue;

                Type predicateParType = GetFieldType(mi).GenericTypeArguments[0];

                List<Type> possibleTypes = new[]
                            {va.ValueCollectionType, va.ValueNullableType ?? va.ValueType}
                    .Where(x => x != null).ToList();

                if (!possibleTypes.Contains(predicateParType))
                    throw new ConfException(
                        $"Argument [{va.Name}]: predicate [{mi.Name}] parameter must be of {string.Join(" or ", possibleTypes.Select(x => x.Name))}, but it is of type {predicateParType.Name}");

                var predicate = (Delegate) GetValue(mi, target);
                if (va.ValueCollectionType == predicateParType) rv.Item1.Add(predicate);
                else rv.Item2.Add(predicate);
            }
            return rv;
        }


        public static Type GetFieldType(MemberInfo mi)
        {
            if (mi is FieldInfo fi) return fi.FieldType;
            if (mi is PropertyInfo pi) return pi.PropertyType;
            throw new ConfException(mi.GetType().Name);
        }


        static object GetValue(MemberInfo mi, object target)
        {
            if (mi is FieldInfo fi) return fi.GetValue(target);
            if (mi is PropertyInfo pi) return pi.GetValue(target);
            throw new ConfException(mi.GetType().Name);
        }
    }
}