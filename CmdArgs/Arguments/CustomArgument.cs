#region usings
using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
            //if (/*нет метода public static P Deserialize(string[] values)*/)
            //    throw new ConfException($"Argument [{Name}]: class {fieldType.Name} does not contain method \"public static {fieldType.Name} Deserialize(string[] values)\"");
        }


        protected override bool CheckAllowedValueType(Type t)
        {
            throw new NotImplementedException();
        }


        protected override object DeserializeOne(string valueSrc)
        {
            throw new NotImplementedException();
        }


        protected override void CheckValueAdditional(object value, string valueSrc, bool isFromCmd)
        {
            throw new NotImplementedException();
        }


        protected override bool CompareWithAllowedValue(object value, object allowedValue)
        {
            throw new NotImplementedException();
        }
    }
}