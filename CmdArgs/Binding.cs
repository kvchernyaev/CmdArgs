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
        public Argument Argument { get; }
        readonly MemberInfo _miTarget;
        readonly object _targetConfObject;

        public bool AlreadySet { get; private set; } = false;


        public Binding(bool longNameIgnoreCase, Argument argument, MemberInfo miTarget,
            object targetConfObject)
        {
            _longNameIgnoreCase = longNameIgnoreCase;
            Argument = argument;
            _miTarget = miTarget;
            _targetConfObject = targetConfObject;
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
                if (va.DefaultValueEffective == null)
                    throw new CmdException($"Value for argument [{va.Name}] is needed");
                SetValueInternal(va.DefaultValueEffective);
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

            var l = new List<object>(vals.Length);
            foreach (string val in vals)
            {
                object argVal = DeserializeOne(va, val);
                l.Add(argVal);
            }
            object collectionValue = va.CreateSameCollection(l);

            va.CheckValuesCollection(collectionValue, true);
            SetValueInternal(collectionValue);
        }


        void DeserializeAndSetValue(string val)
        {
            var va = (ValuedArgument) Argument;
            object argVal = DeserializeOne(va, val);

            SetValueInternal(argVal);
        }


        /// <summary>
        /// and check
        /// </summary>
        static object DeserializeOne(ValuedArgument va, string val)
        {
            object argVal;
            try
            {
                argVal = va.DeserializeAndCheckValueMaybeString(val, true);
            }
            catch (FormatException ex)
            {
                throw new CmdException(
                    $"Value [{val}] can not be converted to type {va.ValueType.Name}",
                    ex);
            }
            catch (NotSupportedException ex)
            {
                throw new CmdException(
                    $"Value [{val}] can not be converted to type {va.ValueType.Name}",
                    ex);
            }
            catch (ArgumentException ex)
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