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
    public abstract class Argument : IArgument
    {
        public string Description { get; set; }
        public char? ShortName { get; set; }
        public string LongName { get; set; }
        public string FullDescription { get; set; }
        public string Example { get; set; }

        public string Name => LongName ?? ShortName.ToString();

        public abstract Type ValueType { get; set; }


        #region ctors
        /// <summary>
        /// Creates new command line argument with a <see cref="ShortName">short name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        protected Argument(char shortName)
        {
            if (!CheckShortName(shortName))
                throw new ConfException(
                    $"Short name of arguments must be a letter, but [{shortName}] provided");
            ShortName = shortName;
        }


        internal static bool CheckLongName(char longNameStart) => char.IsLetter(longNameStart);
        internal static bool CheckShortName(char shortName) => char.IsLetter(shortName);


        /// <summary>
        /// Creates new command line argument with a <see cref="LongName">long name</see>.
        /// </summary>
        /// <param name="longName">Long name of the argument</param>
        protected Argument(string longName)
        {
            if (string.IsNullOrWhiteSpace(longName))
                throw new ConfException("Long name is empty");
            if (longName.Length == 0 || !CheckLongName(longName[0]))
                throw new ConfException(
                    $"First symbol of long name of arguments must be a letter, but [{longName}] provided");
            LongName = longName;
        }


        /// <summary>
        /// Creates new command line argument with a <see cref="ShortName">short name</see>and <see cref="LongName">long name</see>.
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        protected Argument(char shortName, string longName)
        {
            if (!CheckShortName(shortName) && string.IsNullOrWhiteSpace(longName))
                throw new ConfException(
                    $"{nameof(shortName)} is not letter ({shortName} provided) and {nameof(longName)} is empty");
            LongName = longName;
            ShortName = shortName;
        }


        /// <summary>
        /// Creates new command line argument with a <see cref="P:CmdArgs.Argument.ShortName">short name</see>,
        /// <see cref="P:CmdArgs.Argument.LongName">long name</see> and <see cref="P:CmdArgs.Argument.Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">Description of the argument</param>
        protected Argument(char shortName, string longName, string description)
            : this(shortName, longName)
        {
            Description = description;
        }
        #endregion


        #region conf
        public bool Mandatory { get; set; } = false;
        public virtual bool AllowMultiple { get; set; } = false;
        #endregion


        public virtual void InitAndCheck<T>(MemberInfo mi, CmdArgsParser<T> p, T target)
            where T : new()
        {
            CheckFieldType(mi.GetFieldType());
        }


        public abstract void CheckFieldType(Type fieldType);


        public abstract bool Parse(object prevValue, string[] values,
            out object argVal);
    }
}