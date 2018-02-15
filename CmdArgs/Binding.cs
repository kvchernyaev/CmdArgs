#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal class Binding
    {
        public Argument Argument { get; private set; }
        readonly MemberInfo _miTarget;
        readonly object _targetConfObject;

        public bool AlreadySet { get; private set; } = false;


        public Binding(bool longNameIgnoreCase, Argument argument, MemberInfo miTarget,
            object targetConfObject)
        {
            Check(argument, miTarget);

            _longNameIgnoreCase = longNameIgnoreCase;
            Argument = argument;
            _miTarget = miTarget;
            _targetConfObject = targetConfObject;
        }


        static void Check(Argument arg, MemberInfo mi)
        {
            if (!arg.CanHaveValue)
                Check(mi, typeof(bool), arg);

            if (!(mi is FieldInfo || mi is PropertyInfo))
                throw new ConfException($"Field type [{mi.GetType().Name}] is not accepted");
        }


        static void Check(MemberInfo mi, Type type, Argument arg)
        {
            if (mi is FieldInfo fi)
            {
                if (!Check(type, fi.FieldType))
                    throw new ConfException(
                        $"Argument [{arg.Name}]: field must be of type {type.Name}, but it is of type {fi.FieldType.Name}");
            }
            else if (mi is PropertyInfo pi)
                if (!Check(type, pi.PropertyType))
                    throw new ConfException(
                        $"Argument [{arg.Name}]: field must be of type {type.Name}, but it is of type {pi.PropertyType.Name}");
        }


        /// <summary>
        /// argument type -> allowed other field types
        /// </summary>
        static readonly Dictionary<Type, List<Type>> AllowedTypes =
            new Dictionary<Type, List<Type>>()
                {
                    {typeof(char), new List<Type> {typeof(string)}},
                    {typeof(int), new List<Type> {typeof(long)}},
                    {typeof(short), new List<Type> {typeof(int), typeof(long)}},
                    {typeof(byte), new List<Type> {typeof(short), typeof(int), typeof(long)}},
                    //
                    //{typeof(decimal), new List<Type> {typeof(double)}},
                    {typeof(double), new List<Type> {typeof(decimal)}},
                    {typeof(float), new List<Type> {typeof(decimal), typeof(double)}},
                };


        static bool Check(Type argType, Type fieldType)
        {
            if (argType.IsAssignableFrom(fieldType)) return true;

            if (fieldType.IsGenericType &&
                fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type realFieldType = fieldType.GenericTypeArguments[0];
                return Check(argType, realFieldType);
            }

            if (AllowedTypes.TryGetValue(argType, out List<Type> allowedTypes))
                return allowedTypes.Contains(fieldType);

            return false;
        }


        readonly bool _longNameIgnoreCase;


        public bool Is(string longName)
            => string.Compare(Argument.LongName, 0, longName, 0,
                   int.MaxValue,
                   _longNameIgnoreCase
                       ? StringComparison.InvariantCultureIgnoreCase
                       : StringComparison.InvariantCulture) == 0;


        public bool Is(char shortName) =>
            Argument.ShortName.HasValue && Argument.ShortName.Value == shortName;


        public void ParseAndSetArgumentValues(string[] values)
        {
            if (!Argument.CanHaveValue) // is it Switch
            {
                if (values != null && values.Length > 0)
                    throw new CmdException(
                        $"Argument [{Argument.Name}] can not have value but value [{string.Join(",", values)}] is passed");
                SetValueInternal(true);
                return;
            }

            var va = (ValuedArgument) Argument;
            if (values == null || values.Length == 0)
            {
                if (va.DefaultValue == null)
                    throw new CmdException($"Value for argument [{va.Name}] is needed");
                SetValueInternal(va.DefaultValue);
            }
            else if (va.ValueIsCollection)
                DeserializeAndSetValue(values);
            else if (values.Length == 1)
                DeserializeAndSetValue(values[0]);
            else
                throw new CmdException(
                    $"Argument [{va.Name}] can not be a collection, but passed [{string.Join(",", values)}]");
        }


        void DeserializeAndSetValue(string[] vals)
        {
            var va = (ValuedArgument) Argument;

            Array array = null;
            IList list = null;

            if (va.ValueCollectionType.IsArray)
                array = Array.CreateInstance(va.ValueNullableType ?? va.ValueType,
                    vals.Length);
            else
                list = (IList) Activator.CreateInstance(va.ValueCollectionType);

            for (var i = 0; i < vals.Length; i++)
            {
                string val = vals[i];
                object argVal = DeserializeOne(va, val);

                if (va.ValueCollectionType.IsArray) array.SetValue(argVal, i);
                else list.Add(argVal);
            }

            object collectionValue = array ?? list;
            va.CheckAllowedCollection(collectionValue);
            SetValueInternal(collectionValue);
        }


        void DeserializeAndSetValue(string val)
        {
            var va = (ValuedArgument) Argument;
            object argVal = DeserializeOne(va, val);

            SetValueInternal(argVal);
        }


        static object DeserializeOne(ValuedArgument va, string val)
        {
            object argVal;
            try
            {
                argVal = va.DeserializeOne(val);
            }
            catch (FormatException ex)
            {
                throw new CmdException(
                    $"Value [{val}] can not be converted to type {va.ValueType.Name}",
                    ex);
            }
            return argVal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="argVal">Must be of argument's type</param>
        void SetValueInternal(object argVal)
        {
            if (_miTarget is FieldInfo fi)
                fi.SetValue(_targetConfObject, argVal);
            else if (_miTarget is PropertyInfo pi)
                pi.SetValue(_targetConfObject, argVal);
            else
                throw new ConfException(
                    $"Binding.SetValueInternal(): type [{_miTarget.GetType().Name}] is not accepted");

            AlreadySet = true;
        }
    }
}