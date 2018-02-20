#region usings
using System;
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
            if (src.IsArray)
            {
                ValueCollectionType = src;
                SetValueType(src.GetElementType());
            }
            //if (typeof(IEnumerable<>).IsAssignableFrom(src)) // open vs closed generic =(
            else if (src.IsGenericType && src.GetInterfaces()
                         .Any(t => t.IsGenericType &&
                                   t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                ValueCollectionType = src;
                SetValueType(src.GenericTypeArguments[0]);
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


        public object DefaultValue { get; set; }
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
                    $"Argument [{Name}]: field type {fieldType.Name} is not allowed");
        }


        public void CheckDefaultAndAllowedTypes()
        {
            if (AllowedValues?.Length > 0)
                foreach (object allowedValue in AllowedValues)
                    CheckAllowedValueType(allowedValue, "allowed value");

            CheckDefaultValue();
        }


        protected virtual void CheckAllowedValueType(object allowedValue, string hint)
        {
            if (allowedValue == null) return;

            if (!ValueType.IsInstanceOfType(allowedValue))
                throw new ConfException(
                    $"Argument [{Name}]: {hint} [{allowedValue}] must be of type {ValueType.Name}, but it is of type {allowedValue.GetType().Name}");
            CheckByPredicates(allowedValue, allowedValue.ToString(), false);
            if (allowedValue is string s)
                CheckByRegex(s, false);
        }


        void CheckDefaultValue()
        {
            CheckAllowedValueType(DefaultValue, nameof(DefaultValue));

            if (DefaultValue != null)
                if (DefaultValue is string s)
                    DefaultValueEffective = DeserializeOne(s);
                else
                {
                    CheckValue(DefaultValue, DefaultValue.ToString(), false);
                    DefaultValueEffective = DefaultValue;
                }
        }


        /// <returns>Instance of ValueType</returns>
        public virtual object DeserializeOne(string valueSrc)
        {
            object obj;
            if (ValueType.IsEnum)
                obj = Enum.Parse(ValueType, valueSrc, true);
            else
                obj = Convert.ChangeType(valueSrc, ValueType,
                    Culture ?? CultureInfo.InvariantCulture);

            CheckValue(obj, valueSrc, true);
            return obj;
        }


        #region check value
        protected virtual void CheckValue(object value, string valueSrc, bool isFromCmd)
        {
            CheckByAllowedValues(value, valueSrc, isFromCmd);
            CheckByPredicates(value, valueSrc, isFromCmd);
            CheckByRegex(valueSrc, isFromCmd);
        }


        protected virtual void CheckByRegex(string valueSrc, bool isFromCmd)
        {
            if (!string.IsNullOrWhiteSpace(RegularExpression) &&
                !Regex.IsMatch(valueSrc, RegularExpression))
            {
                string e =
                    $"Argument [{Name}] value [{valueSrc}] is not allowed by regular expression";
                throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
            }
        }


        protected void CheckByPredicates(object value, string valueSrc, bool isFromCmd)
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


        protected virtual void CheckByAllowedValues(object value, string valueSrc, bool isFromCmd)
        {
            if (AllowedValues?.Length > 0 &&
                AllowedValues.Where(x => x != null).All(x => !object.Equals(x, value)))
            {
                string e = $"Argument [{Name}]: value [{valueSrc}] is not allowed";
                throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
            }
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
        #endregion
    }
}