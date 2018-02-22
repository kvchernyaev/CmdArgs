﻿#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class ValuedArgument : Argument, IValuedArgument
    {
        public Type ValueCollectionType { get; private set; }
        public Type ValueNullableType { get; private set; }
        public override Type ValueType { get; set; }

        public List<Delegate> ValuePredicatesForOne { get; set; }
        public List<Delegate> ValuePredicatesForCollection { get; set; }


        public void SetTypeAndCheck(Type src)
        {
            Type elemtype = GetElemTypeIfCollection(src);
            if (elemtype != null)
            {
                ValueCollectionType = src;
                SetValueType(elemtype);
            }
            else
            {
                ValueCollectionType = null;
                SetValueType(src);
            }

            CheckFieldType(ValueType);
            CheckDefaultAndAllowedTypes();

            void SetValueType(Type t)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    ValueNullableType = t;
                    ValueType = t.GenericTypeArguments[0];
                }
                else
                {
                    ValueNullableType = null;
                    ValueType = t;
                }
            }
        }


        public object CreateSameCollection(List<object> values)
        {
            Array array = null;
            IList list = null;

            if (ValueCollectionType.IsArray)
                array = Array.CreateInstance(GetElemTypeIfCollection(ValueCollectionType),
                    values.Count);
            else
                list = (IList) Activator.CreateInstance(ValueCollectionType);

            for (var i = 0; i < values.Count; i++)
                if (ValueCollectionType.IsArray)
                    array.SetValue(values[i], i);
                else
                    list.Add(values[i]);

            return array ?? list;
        }


        protected static Type GetElemTypeIfCollection(Type t)
        {
            if (t.IsArray) return t.GetElementType();
            if (t.IsGenericType && t.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                return t.GenericTypeArguments[0];

            return null;
        }


        protected static bool IsCollection(object o) =>
            GetElemTypeIfCollection(o.GetType()) != null;


        public object DefaultValue { get; set; }
        public bool UseDefWhenNoArg { get; set; }
        public object DefaultValueEffective { get; set; }
        public object[] AllowedValues { get; set; }
        public string RegularExpression { get; set; }

        public IFormatProvider Culture { get; set; }


        public bool ValueIsCollection => ValueCollectionType != null;


        #region ctors
        public ValuedArgument(char shortName)
            : base(shortName) { }


        public ValuedArgument(string longName)
            : base(longName) { }


        public ValuedArgument(char shortName, string longName)
            : base(shortName,
                longName) { }


        public ValuedArgument(char shortName, string longName, string description) :
            base(shortName, longName, description) { }
        #endregion


        internal override bool CanHaveValue => true;


        static readonly Type[] AllowedTypes =
            {
                typeof(char), typeof(string), typeof(byte), typeof(short), typeof(int),
                typeof(long), typeof(bool), typeof(uint), typeof(ulong), typeof(ushort),
                typeof(decimal), typeof(float), typeof(double), typeof(sbyte)
            };


        public override void CheckFieldType(Type fieldType)
        {
            // тут разрешены все енумы и примитивные типы
            if (!fieldType.IsEnum && !AllowedTypes.Contains(fieldType))
                throw new ConfException(
                    $"Argument [{Name}]: field type {fieldType.Name} is not allowed in {nameof(ValuedArgument)}. Use other argument types");
        }


        public void CheckDefaultAndAllowedTypes()
        {
            if (AllowedValues?.Length > 0)
                foreach (object allowedValue in AllowedValues)
                {
                    if (!CheckAllowedValueType(allowedValue.GetType()))
                        throw new ConfException(
                            $"Argument [{Name}]: allowed value must be of type {ValueType.Name}, but it is of type {allowedValue.GetType().Name}");
                    DeserializeAndCheckValueMaybeString(allowedValue, false);
                }

            CheckDefaultValue();
        }


        protected virtual bool CheckAllowedValueType(Type t) => ValueType.IsAssignableFrom(t);


        void CheckDefaultValue()
        {
            if (DefaultValue == null)
            {
                if (UseDefWhenNoArg)
                    throw new ConfException(
                        $"Argument [{Name}]: {nameof(UseDefWhenNoArg)} is true but DefaultValue is not provided");
                return;
            }

            if (Mandatory && UseDefWhenNoArg)
                throw new ConfException(
                    $"Argument [{Name}]: {nameof(DefaultValue)} is provided, {nameof(UseDefWhenNoArg)} is true, but {nameof(Mandatory)} is true");

            if (!ValueIsCollection && IsCollection(DefaultValue))
                throw new ConfException(
                    $"DefaultValue cannot be a collection if field type is not a collection");

            if (IsCollection(DefaultValue))
            {
                var l = new List<object>();
                foreach (object defValItem in (IEnumerable) DefaultValue)
                {
                    if (!CheckAllowedValueType(defValItem.GetType()))
                        throw new ConfException(
                            $"Argument [{Name}]: DefaultValue must be of type {ValueType.Name} or collection of it, but it is of type {defValItem.GetType().Name}");
                    object o = DeserializeAndCheckValueMaybeString(defValItem, false);
                    l.Add(o);
                }
                DefaultValueEffective = CreateSameCollection(l);
                CheckValuesCollection(DefaultValueEffective);
            }
            else
            {
                if (!CheckAllowedValueType(DefaultValue.GetType()))
                    throw new ConfException(
                        $"Argument [{Name}]: DefaultValue must be of type {ValueType.Name} or collection of it, but it is of type {DefaultValue.GetType().Name}");
                object o = DeserializeAndCheckValueMaybeString(DefaultValue, false);
                DefaultValueEffective =
                    ValueIsCollection ? CreateSameCollection(new List<object> {o}) : o;
            }
        }


        /// <returns>Instance of ValueType</returns>
        protected virtual object DeserializeOne(string valueSrc)
        {
            object obj = ValueType.IsEnum
                ? Enum.Parse(ValueType, valueSrc, true)
                : Convert.ChangeType(valueSrc, ValueType, Culture ?? CultureInfo.InvariantCulture);
            return obj;
        }


        protected object DeserializeOneOrPass(object value) => ValueType.IsInstanceOfType(value)
            ? value
            : DeserializeOne((string) value);


        #region check value or collection of values
        public object DeserializeAndCheckValueMaybeString(object v, bool isFromCmd)
        {
            object o = DeserializeOneOrPass(v);
            CheckValue(o, v.ToString(), isFromCmd);
            return o;
        }


        /// <summary>
        /// Проверить значение, уже десериализованное. Не проверяется тип.
        /// </summary>
        void CheckValue(object value, string valueSrc, bool isFromCmd)
        {
            CheckByAllowedValues(value, valueSrc, isFromCmd);
            CheckByPredicates(value, valueSrc, isFromCmd);
            CheckByRegex(valueSrc, isFromCmd);

            CheckValueAdditional(value, valueSrc, isFromCmd);
        }


        public void CheckValuesCollection(object collectionValue)
        {
            if (ValuePredicatesForCollection?.Count > 0)
                foreach (Delegate predicate in ValuePredicatesForCollection)
                {
                    var ok = (bool) predicate.DynamicInvoke(collectionValue);
                    if (!ok)
                        throw new CmdException(
                            $"Argument [{Name}] value is not allowed by collection predicate");
                }
        }


        protected virtual void
            CheckValueAdditional(object value, string valueSrc, bool isFromCmd) { }


        void CheckByRegex(string valueSrc, bool isFromCmd)
        {
            if (!string.IsNullOrWhiteSpace(RegularExpression) &&
                !Regex.IsMatch(valueSrc, RegularExpression))
            {
                string e =
                    $"Argument [{Name}] value [{valueSrc}] is not allowed by regular expression";
                throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
            }
        }


        void CheckByPredicates(object value, string valueSrc, bool isFromCmd)
        {
            if (ValuePredicatesForOne?.Count > 0)
                foreach (Delegate predicate in ValuePredicatesForOne)
                {
                    var ok = (bool) predicate.DynamicInvoke(value);
                    if (!ok)
                    {
                        string e =
                            $"Argument [{Name}] value [{valueSrc}] is not allowed by predicate";
                        throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
                    }
                }
        }


        /// <summary>
        /// Проверить значение, уже десериализованное, на вхождение в AllowedValues
        /// </summary>
        void CheckByAllowedValues(object value, string valueSrc, bool isFromCmd)
        {
            if (AllowedValues?.Length > 0 &&
                AllowedValues.Where(x => x != null).All(x => !CompareWithAllowedValue(value, x)))
            {
                string e = $"Argument [{Name}]: value [{valueSrc}] is not allowed";
                throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
            }
        }


        protected virtual bool CompareWithAllowedValue(object value, object allowedValue) =>
            object.Equals(allowedValue, value);
        #endregion
    }
}