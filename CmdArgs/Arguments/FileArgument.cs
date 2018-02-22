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
    public class FileArgument : ConcreteArgument<FileInfo>
    {
        #region ctors
        public FileArgument(char shortName) : base(shortName) { }
        public FileArgument(string longName) : base(longName) { }
        public FileArgument(char shortName, string longName) : base(shortName, longName) { }


        public FileArgument(char shortName, string longName, string description) : base(shortName,
            longName, description) { }
        #endregion


        public bool MustExists { get; set; }


        protected override bool CheckAllowedValueType(Type t) =>
            t == typeof(string) || t == typeof(FileInfo);


        #region value
        protected override object DeserializeOne(string valueSrc)
        {
            var fi = new FileInfo(valueSrc);
            return fi;
        }


        protected override void CheckValueAdditional(object value, string valueSrc, bool isFromCmd)
        {
            if (MustExists)
            {
                var fi = (FileInfo) DeserializeOneOrPass(value);
                if (!fi.Exists)
                {
                    string e = $"Argument [{Name}]: file [{valueSrc}] must be exists";
                    throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
                }
            }
        }


        protected override bool Compare(FileInfo value, FileInfo allowedValue) =>
            value.FullName == allowedValue.FullName;
        #endregion
    }
}