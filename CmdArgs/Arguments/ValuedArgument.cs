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

            CheckFieldType(null);

            // type of DefaultValue
            if (DefaultValue != null && !ValueType.IsInstanceOfType(DefaultValue))
                throw new ConfException(
                    $"Argument [{Name}]: {nameof(DefaultValue)} must be of type {ValueType.Name}, but it is of type {DefaultValue.GetType().Name}");

            // type of AllowedValues
            if (AllowedValues?.Length > 0)
            {
                foreach (object allowedValue in AllowedValues)
                    if (!ValueType.IsInstanceOfType(allowedValue))
                        throw new ConfException(
                            $"Argument [{Name}]: allowed value [{allowedValue}] must be of type {ValueType.Name}, but it is of type {allowedValue.GetType().Name}");

                if (DefaultValue != null && !AllowedValues.Contains(DefaultValue))
                    throw new ConfException(
                        $"Argument [{Name}]: default value [{DefaultValue}[ is not allowed");
            }

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
            if (!ValueType.IsEnum && !AllowedTypes.Contains(ValueType))
                throw new ConfException(
                    $"Argument [{Name}]: field type {ValueType.Name} is not allowed");
        }


        /// <returns>Instance of ValueType</returns>
        public virtual object DeserializeOne(string val)
        {
            object rv;
            if (ValueType.IsEnum)
                rv = Enum.Parse(ValueType, val, true);
            else
                rv = Convert.ChangeType(val, ValueType, Culture ?? CultureInfo.InvariantCulture);

            CheckAllowedOne(rv, val);
            return rv;
        }


        void CheckAllowedOne(object value, string valueSrc)
        {
            if (AllowedValues?.Length > 0 && !AllowedValues.Contains(value))
                throw new CmdException($"Argument [{Name}]: value [{valueSrc}] is not allowed");
            if (ValuePredicatesForOne?.Count > 0)
                foreach (Delegate predicate in ValuePredicatesForOne)
                {
                    var ok = (bool) predicate.DynamicInvoke(value);
                    if (!ok)
                        throw new CmdException(
                            $"Argument [{Name}] value [{valueSrc}] is not allowed by predicate");
                }
            if (!string.IsNullOrWhiteSpace(RegularExpression))
                if (!Regex.IsMatch(valueSrc, RegularExpression))
                    throw new CmdException(
                        $"Argument [{Name}] value [{valueSrc}] is not allowed by regular expression");
        }


        public void CheckAllowedCollection(object collectionValue)
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
    }
}