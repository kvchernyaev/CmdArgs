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
#endregion



namespace CmdArgs
{
    [DebuggerDisplay("{Argument} {_miTarget}")]
    internal class Binding<TArgs> where TArgs : new()
    {
        public Argument Argument { get; }

        readonly MemberInfo _miTarget;
        readonly TArgs _targetConfObject;

        readonly Res<TArgs> _target;
        readonly CmdArgsParser<TArgs> _cmdArgsParser;
        internal Bindings<TArgs> bs;

        public bool AlreadySet { get; private set; } = false;


        public Binding(bool longNameIgnoreCase, Argument argument, MemberInfo miTarget,
            Res<TArgs> target, CmdArgsParser<TArgs> cmdArgsParser)
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


        public bool IsSame(Binding<TArgs> other) => _miTarget == other._miTarget;


        public void SetVal(string[] values)
        {
            //if (AlreadySet && !Argument.AllowMultiple)
            //    throw new CmdException(
            //        $"Argument [{Argument.Name}] can not be set multiple times");

            if (Argument.Parse(_miTarget.GetValue(_targetConfObject), values, out object argVal))
                SetParsedVal(argVal);
        }


        internal void SetParsedVal(Binding<TArgs> bArgVal)
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
            if (AlreadySet && Argument is UnstrictlyConfArgument uca)
            {
                var uc = (UnstrictlyConf) _miTarget.GetValue(_targetConfObject);
                uca.Append(uc, (UnstrictlyConf) argVal);
            }
            else
                _miTarget.SetValue(_targetConfObject, argVal);
            AlreadySet = true;
        }
    }
}