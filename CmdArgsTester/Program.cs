#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
#endregion



namespace CmdArgsTester
{
    public class Program
    {
        static void Main(string[] a)
        {
            var p = new CmdArgsParser<ConfFullExample>();
            p.AllowUnknownArguments = false;
            p.AllowAdditionalArguments = false;

            Res<ConfFullExample> r;

            r = p.ParseCommandLine(new[] {"--myenum", "1"});
            Assert.AreEqual(MyEnum.One, r.Args.MyEnumVal);

            r = p.ParseCommandLine(new[] {"--myenum", "Three"});
            Assert.AreEqual(MyEnum.Three, r.Args.MyEnumVal);

            r = p.ParseCommandLine(new[] {"--SwitchOne"});
            Assert.AreEqual(true, r.Args.SwitchOne);

            r = p.ParseCommandLine(new[] {"-ab"});
            Assert.AreEqual(true, r.Args.SwitchOne);
            Assert.AreEqual(true, r.Args.SwitchTwo);

            r = p.ParseCommandLine(new[] {"--IntField", "-34", "--IntDefault"});
            Assert.AreEqual(-34, r.Args.IntField);
            Assert.AreEqual(5, r.Args.IntDefault);

            r = p.ParseCommandLine(new[] {"--IntArray", "-34", "4", "234"});
            Assert.IsTrue(new[] {-34, 4, 234}.SequenceEqual(r.Args.IntArray));

            r = p.ParseCommandLine(new[] {"--IntList", "-34", "4", "234"});
            Assert.IsTrue(new[] {-34, 4, 234}.SequenceEqual(r.Args.IntList));

            r = p.ParseCommandLine(new[] {"--StringRegex", "bqwerty"});
            Assert.AreEqual("bqwerty", r.Args.StringRegex);

            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"--StringRegex", "qwe"}));

            r = p.ParseCommandLine(new[] {"--IntRegex", "208"});
            Assert.AreEqual(208, r.Args.IntRegex);

            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"--IntRegex", "218"}));

            var testi = 20;
            Predicate<int> pred = i => i <= testi; // using closure
            ConfFullExample.IntPredicated_Predicate_closure = pred; // using delegate variable

            r = p.ParseCommandLine(new[] {"--IntPredicated", "19"}); // <=20
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"--IntPredicated", "21"})); // not <=20

            r = p.ParseCommandLine(new[]
                    {"--IntArrayPredicated", "3", "12", "21"});
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[]
                        {"--IntArrayPredicated", "3", "12", "1"})); // not item > 2
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[]
                        {"--IntArrayPredicated", "3", "12"})); // not length > 2

            r = p.ParseCommandLine(new[] {"--StringAllowed", "asd"});
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"--StringAllowed", "kjhpj"}));

            // features also: MustExists, DefaultValue, AllowedValues work too (string)
            r = p.ParseCommandLine(new[]
                    {"--FileConfig", "./CmdArgsTester.exe.config"});
            r = p.ParseCommandLine(new[] {"--DirConfig", "./DirectoryName"});
        }



        public class ConfFullExample
        {
            [SwitchArgument('a', "SwitchOne")] public bool SwitchOne;
            [SwitchArgument('b', "SwitchTwo")] public bool SwitchTwo;


            [ValuedArgument('c', "IntField")] public int IntField;


            [ValuedArgument('d', "IntDefault", DefaultValue = 5)]
            public int IntDefault;


            [ValuedArgument('e', "IntProp")]
            public int IntProp { get; set; }


            [ValuedArgument('f')] public string S;
            [ValuedArgument('g')] public char C;
            [ValuedArgument('h')] public decimal Dec;


            [ValuedArgument('i', "IntNullable")] public int? IntNullable;


            // AllowedValues & predicates works also
            [ValuedArgument('k', "IntArray")] public int[] IntArray;
            [ValuedArgument('l', "IntList")] public List<int> IntList;


            [ValuedArgument('m', "StringRegex", RegularExpression = @"^[a-d]\w{5,7}$")]
            public string StringRegex;


            [ValuedArgument('n', "IntRegex", RegularExpression = @"^[123]0[89]$")]
            public int IntRegex;


            [ValuedArgument('o', "IntPredicated")] public int IntPredicated;
            public static Predicate<int> IntPredicated_Predicate_closure;


            [ValuedArgument('p', "IntArrayPredicated")]
            public int[] IntArrayPredicated;


            public static Predicate<int> IntArrayPredicated_Predicate_const = i => i > 2;
            public static Predicate<int[]> IntArrayPredicated_Predicate_array = i => i.Length > 2;


            [ValuedArgument('q', "StringAllowed", AllowedValues = new object[] {"asd", "rt"},
                DefaultValue = "rt")]
            public string StringAllowed;


            [ValuedArgument('r', "myenum", AllowedValues = new object[] {MyEnum.One, MyEnum.Three})]
            public MyEnum MyEnumVal;


            [FileArgument('s', "FileConfig", DefaultValue = "./CmdArgsTester.exe.config")]
            public FileInfo FileConfig;


            [FileArgument('t', "DirConfig")] public FileInfo DirConfig;
        }



        public enum MyEnum : byte
        {
            Nol,
            One,
            Two,
            Three
        }
    }
}