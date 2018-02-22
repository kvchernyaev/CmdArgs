#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public abstract class ConcreteArgument<T> : ValuedArgument
    {
        #region ctors
        protected ConcreteArgument(char shortName) : base(shortName) { }
        protected ConcreteArgument(string longName) : base(longName) { }
        protected ConcreteArgument(char shortName, string longName) : base(shortName, longName) { }


        protected ConcreteArgument(char shortName, string longName, string description) : base(
            shortName,
            longName, description) { }
        #endregion


        //public override Type ValueType
        //{
        //    get => typeof(T);
        //    set => throw new Exception();
        //}


        public override void CheckFieldType(Type fieldType)
        {
            Type elemType;
            if (!(fieldType == ValueType ||
                  (elemType = GetElemTypeIfCollection(fieldType)) == null || elemType == ValueType))
                throw new ConfException(
                    $"Argument [{Name}]: field type must be {ValueType.Name} or collection of it but {fieldType.Name} provided");
        }


        protected override bool CompareWithAllowedValue(object value, object allowedValue)
        {
            var v = (T) DeserializeOneOrPass(value);
            var a = (T) DeserializeOneOrPass(allowedValue);

            return Compare(v, a);
        }


        protected virtual bool Compare(T value, T allowedValue) =>
            object.Equals(value, allowedValue);
    }
}