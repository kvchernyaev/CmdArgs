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
    public class DirArgument : ConcreteArgument<DirectoryInfo>
    {
        #region ctors
        public DirArgument(char shortName) : base(shortName) { }
        public DirArgument(string longName) : base(longName) { }
        public DirArgument(char shortName, string longName) : base(shortName, longName) { }


        public DirArgument(char shortName, string longName, string description) : base(shortName,
            longName, description) { }
        #endregion


        public bool MustExists { get; set; }


        protected override bool CheckAllowedValueType(Type t) =>
            t == typeof(string) || t == typeof(DirectoryInfo);


        #region value
        protected override object DeserializeOne(string valueSrc)
        {
            var fi = new DirectoryInfo(valueSrc);
            return fi;
        }


        protected override void CheckValueAdditional(object value, string valueSrc, bool isFromCmd)
        {
            if (MustExists)
            {
                var fi = (DirectoryInfo) DeserializeOneOrPass(value);
                if (!fi.Exists)
                {
                    string e = $"Argument [{Name}]: Dir [{valueSrc}] must be exists";
                    throw isFromCmd ? (Exception) new CmdException(e) : new ConfException(e);
                }
            }
        }


        protected override bool Compare(DirectoryInfo value, DirectoryInfo allowedValue) =>
            value.FullName == allowedValue.FullName;
        #endregion
    }
}