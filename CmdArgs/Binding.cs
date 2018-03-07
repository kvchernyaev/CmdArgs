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
        internal Bindings<T> bs;

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


        public bool IsSame(Binding<T> other) => _miTarget == other._miTarget;


        public void SetVal(string[] values)
        {
            if (AlreadySet && !Argument.AllowMultiple)
                throw new CmdException(
                    $"Argument [{Argument.Name}] can not be set multiple times");

            if (Argument.Parse(_miTarget.GetValue(_targetConfObject), values, out object argVal))
                SetParsedVal(argVal);
        }


        internal void SetParsedVal(Binding<T> bArgVal)
        {
            object argVal = _miTarget.GetValue(bArgVal._targetConfObject);
            SetParsedVal(argVal);
        }


        void SetParsedVal(object argVal)
        {
            SetValueInternal(argVal);
            if (Argument is ArgsFileArgument afa)
            {
                var fi = (FileInfo) _miTarget.GetValue(_targetConfObject);
                afa.Apply(fi, _cmdArgsParser, bs);
            }
        }


        void SetValueInternal(object argVal)
        {
            _miTarget.SetValue(_targetConfObject, argVal);
            AlreadySet = true;
        }
    }
}