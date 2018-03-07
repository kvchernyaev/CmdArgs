#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
#endregion



namespace CmdArgs
{
    [DebuggerDisplay("{Argument} {_miTarget}")]
    internal class Binding<T> where T : new()
    {
        public Argument Argument { get; }
        readonly MemberInfo _miTarget;
        readonly T _targetConfObject;
        readonly Res<T> _target;
        readonly CmdArgsParser<T> _cmdArgsParser;

        public bool AlreadySet { get; private set; } = false;


        public Binding(bool longNameIgnoreCase, Argument argument, MemberInfo miTarget,
            Res<T> target, CmdArgsParser<T> cmdArgsParser)
        {
            _longNameIgnoreCase = longNameIgnoreCase;
            Argument = argument;
            _miTarget = miTarget;
            _target = target;
            _targetConfObject = target.Args;
            _cmdArgsParser = cmdArgsParser;
            _target = target;
        }


        readonly bool _longNameIgnoreCase;


        public bool Is(string longName)
            => string.Compare(Argument.LongName, 0, longName, 0,
                   int.MaxValue,
                   _longNameIgnoreCase
                       ? StringComparison.InvariantCultureIgnoreCase
                       : StringComparison.InvariantCulture) == 0;


        public bool Is(char shortName) =>
            Argument.ShortName.HasValue && Argument.ShortName.Value == shortName;


        public void SetVal(string[] values)
        {
            if (AlreadySet && !Argument.AllowMultiple)
                throw new CmdException(
                    $"Argument [{Argument.Name}] can not be set multiple times");

            if (Argument.Parse(GetValueInternal(), values, out object argVal))
                SetValueInternal(argVal);

            if (Argument is ArgsFileArgument afa)
            {
                var fi = (FileInfo) GetValueInternal();
                afa.Apply(fi, _cmdArgsParser, _target);
            }
        }


        /// <param name="argVal">Must be of argument's type</param>
        void SetValueInternal(object argVal)
        {
            if (_miTarget is FieldInfo fi)
                fi.SetValue(_targetConfObject, argVal);
            else if (_miTarget is PropertyInfo pi)
                pi.SetValue(_targetConfObject, argVal);
            else
                throw new ConfException(
                    $"Binding.SetValueInternal(): type [{_miTarget.GetType().Name}] is not accepted");

            AlreadySet = true;
        }


        object GetValueInternal()
        {
            object rv;
            if (_miTarget is FieldInfo fi)
                rv = fi.GetValue(_targetConfObject);
            else if (_miTarget is PropertyInfo pi)
                rv = pi.GetValue(_targetConfObject);
            else
                throw new ConfException(
                    $"Binding.SetValueInternal(): type [{_miTarget.GetType().Name}] is not accepted");

            return rv;
        }
    }
}