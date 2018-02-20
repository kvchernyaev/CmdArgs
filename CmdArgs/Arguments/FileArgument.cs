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
    public class FileArgument : ValuedArgument
    {
        #region ctors
        public FileArgument(char shortName) : base(shortName) { }
        public FileArgument(string longName) : base(longName) { }
        public FileArgument(char shortName, string longName) : base(shortName, longName) { }


        public FileArgument(char shortName, string longName, string description) : base(shortName,
            longName, description) { }
        #endregion


        public bool MustExists { get; set; }


        public override Type ValueType
        {
            get => typeof(FileInfo);
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
                allowedValue.GetType() != typeof(FileInfo))
                throw new ConfException(
                    $"Argument [{Name}]: {hint} [{allowedValue}] must be of type string or FileInfo, but it is of type {allowedValue.GetType().Name}");
        }


        public override object DeserializeOne(string valueSrc)
        {
            var fi = new FileInfo(valueSrc);
            CheckValue(fi, valueSrc, true);
            return fi;
        }


        protected override void CheckValue(object value, string valueSrc, bool isFromCmd)
        {
            base.CheckValue(value, valueSrc, isFromCmd);
            if (MustExists)
            {
                FileInfo fi = value is FileInfo info ? info : new FileInfo((string) value);
                if (!fi.Exists)
                    throw new CmdException($"Argument [{Name}]: file [{valueSrc}] must be exists");
            }
        }


        protected override void CheckByAllowedValues(object value, string valueSrc, bool isFromCmd)
        {
            FileInfo fiSrc = value is FileInfo info ? info : new FileInfo((string) value);
            if (AllowedValues?.Length > 0 &&
                AllowedValues.Select(x => new FileInfo((string) x))
                    .All(fi => fi.FullName != fiSrc.FullName))
            {
                string e = $"Argument [{Name}]: value [{valueSrc}] is not allowed";
                throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
            }
        }
    }
}