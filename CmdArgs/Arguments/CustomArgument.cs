#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class CustomArgument : ValuedArgument
    {
        #region ctors
        public CustomArgument(char shortName) : base(shortName) { }
        public CustomArgument(string longName) : base(longName) { }
        public CustomArgument(char shortName, string longName) : base(shortName, longName) { }


        public CustomArgument(char shortName, string longName, string description) : base(shortName,
            longName, description) { }
        #endregion


        public override void CheckFieldType(Type fieldType)
        {
            MethodInfo deserMi = GetMethodDeserialize(fieldType);
            if (deserMi == null)
                throw new ConfException(
                    $"Argument [{Name}]: class {fieldType.Name} does not contain method \"public static {fieldType.Name} Deserialize(string value)\"");
        }


        protected override bool CheckAllowedValueType(Type t) =>
            t == typeof(string) || t == ValueType;


        protected override object DeserializeOne(string valueSrc)
        {
            MethodInfo deserMi = GetMethodDeserialize(ValueType);
            try
            {
                object o = deserMi.Invoke(null, new object[] {valueSrc});
                return o;
            }
            catch (TargetInvocationException e)
            {
                throw new CmdException(
                    $"Argument [{Name}]: value [{valueSrc}] can not be converted to type {ValueType.Name}",
                    e);
            }
        }


        protected override bool CompareWithAllowedValue(object value, object allowedValue)
        {
            object v = DeserializeOneOrPass(value);
            object a = DeserializeOneOrPass(allowedValue);

            bool rv = object.Equals(a, v);
            return rv;
        }


        static MethodInfo GetMethodDeserialize(Type fieldType)
        {
            List<MethodInfo> ms = fieldType.GetMethods().Where(x =>
                {
                    ParameterInfo[] pis = x.GetParameters();
                    return x.IsStatic && x.Name == "Deserialize" && x.ReturnType == fieldType
                           && pis.Length == 1 && pis[0].ParameterType == typeof(string);
                })
                .ToList();
            return ms.FirstOrDefault();
        }
    }
}