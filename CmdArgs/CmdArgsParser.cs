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
    public class CmdArgsParser
    {
        readonly IFormatProvider _culture = CultureInfo.InvariantCulture;
        // ReSharper disable MemberCanBePrivate.Global


        #region configuration
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        public bool LongNameIgnoreCase { get; set; } = true;
        public bool AllowUnknownArgument { get; set; } = true;


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
            var bindings = new Bindings<T>
                {
                    Args = res,
                    bindings = ExtractArgumentAttributes(res.Args)
                };
            CheckCongregate(bindings.bindings);

            for (var i = 0; i < args.Length;)
            {
                string arg = args[i];
                ++i;

                string[] values = GetValues(args, ref i);
                if (IsArgLong(arg))
                {
                    string longName = arg.Substring(2);
                    SetVal(bindings, longName, values);
                }
                else if (IsArgShort(arg))
                {
                    string shortNames = arg.Substring(1);
                    if (shortNames.Length == 1)
                    {
                        char shortName = shortNames[0];
                        SetVal(bindings, shortName, values);
                    }
                    else if (shortNames.Length > 1)
                    {
                        if (values.Length > 0)
                            throw new CmdException(
                                $"Values after multiple shortnames can not be processed");

                        foreach (char shortName in shortNames)
                            SetVal(bindings, shortName, values);
                    }
                    else
                        // "-"
                        throw new CmdException($"Argument [{arg}] can not be processed");
                }
                else
                {
                    if (AllowAdditionalArguments)
                        res.AdditionalArguments.Add(arg);
                    else
                        throw new CmdException(
                            $"Unnamed arguments is prohibited ({arg}). Use {nameof(AllowAdditionalArguments)} setting?");
                }
            }

            foreach (Binding binding in bindings.bindings)
                if (binding.Argument.Mandatory && !binding.AlreadySet)
                    throw new CmdException(
                        $"Argument [{binding.Argument.Name}] is mandatory but is not set");
        }


        void SetVal<T>(Bindings<T> bindings, char shortName, string[] values)
        {
            if (!Argument.CheckShortName(shortName))
                throw new CmdException($"ShortName [{shortName}] is not allowed");
            Binding binding = bindings.bindings.FirstOrDefault(x => x.Is(shortName));
            SetVal(bindings, binding, shortName.ToString(), values);
        }


        void SetVal<T>(Bindings<T> bindings, string longName, string[] values)
        {
            Binding binding = bindings.bindings.FirstOrDefault(x => x.Is(longName));
            SetVal(bindings, binding, longName, values);
        }


        void SetVal<T>(Bindings<T> bindings, Binding binding, string name, string[] values)
        {
            if (binding == null)
                if (AllowUnknownArgument)
                    bindings.Args.UnknownArguments.Add(new Tuple<string, string[]>(name, values));
                else
                    throw new CmdException($"Unknown parameter: {name}");
            else
            {
                if (binding.AlreadySet && !binding.Argument.AllowMultiple)
                    throw new CmdException(
                        $"Argument [{binding.Argument.Name}] can not be set multiple times");

                binding.ParseAndSetArgumentValues(values);
            }
        }


        static string[] GetValues(string[] args, ref int i)
        {
            string[] rv = args.Skip(i).TakeWhile(s => !IsArg(s)).ToArray();
            i += rv.Length;
            return rv;
        }


        static bool IsArg(string arg) => IsArgLong(arg) || IsArgShort(arg);


        static bool IsArgLong(string arg) => arg.StartsWith("--");


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
                    l.Select(x => $"LongName {x.Key} is given {x.Count()} times")));

            List<IGrouping<char?, char?>> s = binds.Select(x => x.Argument.ShortName)
                .Where(x => x.HasValue)
                .GroupBy(x => x, x => x)
                .ToList()
                .Where(g => g.Count() > 1)
                .ToList();
            if (s.Count > 0)
                throw new ConfException(string.Join("; ",
                    l.Select(x => $"ShortName {x.Key} is given {x.Count()} times")));
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
                    Type fieldType = GetFieldType(mi);
                    va.SetType(fieldType);

                    // todo все эти проверки должны быть в самом аргументе при его создании 
                    if (va.DefaultValue != null && !va.ValueType.IsInstanceOfType(va.DefaultValue))
                        throw new ConfException(
                            $"Argument [{va.Name}]: {nameof(va.DefaultValue)} must be of type {va.ValueType.Name}, but it is of type {va.DefaultValue.GetType().Name}");
                    if (va.AllowedValues?.Length > 0)
                    {
                        foreach (object allowedValue in va.AllowedValues)
                            if (!va.ValueType.IsInstanceOfType(allowedValue))
                                throw new ConfException(
                                    $"Argument [{va.Name}]: allowed value [{allowedValue}] must be of type {va.ValueType.Name}, but it is of type {allowedValue.GetType().Name}");

                        if (va.DefaultValue != null && !va.AllowedValues.Contains(va.DefaultValue))
                            throw new ConfException(
                                $"Argument [{va.Name}]: default value [{va.DefaultValue}[ is not allowed");
                    }
                    Tuple<List<Delegate>, List<Delegate>> predicates =
                        GetPredicates(miPredicates, mi.Name, va, target);

                    va.ValuePredicatesForCollection = predicates?.Item1;
                    va.ValuePredicatesForOne = predicates?.Item2;

                    if (va.Culture == null) va.Culture = this._culture;
                }
                rv.Add(new Binding(LongNameIgnoreCase, attr.Argument, mi, target));
            }

            return rv;
        }


        Tuple<List<Delegate>, List<Delegate>>
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

                Delegate predicate = (Delegate)GetValue(mi, target);
                if (va.ValueCollectionType == predicateParType) rv.Item1.Add(predicate);
                else rv.Item2.Add(predicate);
            }
            return rv;
        }


        static Type GetFieldType(MemberInfo mi)
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