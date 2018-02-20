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
    public class DirArgument : ValuedArgument
    {
        #region ctors
        public DirArgument(char shortName) : base(shortName) { }
        public DirArgument(string longName) : base(longName) { }
        public DirArgument(char shortName, string longName) : base(shortName, longName) { }


        public DirArgument(char shortName, string longName, string description) : base(shortName,
            longName, description) { }
        #endregion


        public bool MustExists { get; set; }


        public override Type ValueType
        {
            get => typeof(DirectoryInfo);
            set => throw new Exception();
        }


        public override void CheckFieldType(Type fieldType)
        {
            if (fieldType != ValueType)
                throw new ConfException(
                    $"Argument [{Name}]: field type must be {ValueType.Name} but {fieldType.Name} provided");
        }


        protected override void CheckAllowedValueType(object allowedValue, string hint)
        {
            if (allowedValue != null &&
                allowedValue.GetType() != typeof(string) &&
                allowedValue.GetType() != typeof(DirectoryInfo))
                throw new ConfException(
                    $"Argument [{Name}]: {hint} [{allowedValue}] must be of type string or DirectoryInfo, but it is of type {allowedValue.GetType().Name}");
        }


        public override object DeserializeOne(string valueSrc)
        {
            var fi = new DirectoryInfo(valueSrc);
            CheckValue(fi, valueSrc, true);
            return fi;
        }


        protected override void CheckValue(object value, string valueSrc, bool isFromCmd)
        {
            base.CheckValue(value, valueSrc, isFromCmd);
            if (MustExists)
            {
                DirectoryInfo fi = value is DirectoryInfo info
                    ? info
                    : new DirectoryInfo((string) value);
                if (!fi.Exists)
                    throw new CmdException($"Argument [{Name}]: Dir [{valueSrc}] must be exists");
            }
        }


        protected override void CheckByAllowedValues(object value, string valueSrc, bool isFromCmd)
        {
            DirectoryInfo fiSrc =
                value is DirectoryInfo info ? info : new DirectoryInfo((string) value);
            if (AllowedValues?.Length > 0 &&
                AllowedValues.Select(x => new DirectoryInfo((string) x))
                    .All(fi => fi.FullName != fiSrc.FullName))
            {
                string e = $"Argument [{Name}]: value [{valueSrc}] is not allowed";
                throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
            }
        }
    }
}