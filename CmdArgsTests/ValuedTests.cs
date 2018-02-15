#region usings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
#endregion



namespace CmdArgsTests
{
    [TestFixture]
    public class ValuedTests
    {
        class ConfBool
        {
            [ValuedArgument('a', DefaultValue = true)]
            public bool A { get; set; }


            [ValuedArgument('b')]
            public bool B { get; set; }


            [ValuedArgument('c')]
            public bool C { get; set; }


            [ValuedArgument('d')]
            public bool? D { get; set; }
        }



        [Test]
        public void TestBool()
        {
            var p = new CmdArgsParser();
            Res<ConfBool> res = p.ParseCommandLine<ConfBool>(new[] {"-a", "-b", "true"});
            Assert.AreEqual(expected: true, actual: res.Args.A);
            Assert.AreEqual(expected: true, actual: res.Args.B);
            Assert.AreEqual(expected: false, actual: res.Args.C);
            Assert.AreEqual(expected: null, actual: res.Args.D);
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefTypeNotMatch
        {
            [ValuedArgument('s', "some", DefaultValue = "1")]
            public int Some { get; set; }
        }



        [Test]
        public void TestDefTypeNotMatch()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(
                () => p.ParseCommandLine<ConfDefTypeNotMatch>(new[] {"-s"}));
        }



        ////////////////////////////////////////////////////////////////
        class Conf
        {
            [ValuedArgument('s', "some")]
            public int Some { get; set; }


            [ValuedArgument('e', "def", DefaultValue = 5)]
            public int Def { get; set; }


            [ValuedArgument('d', "dummy")]
            public long Dummy { get; set; }


            [ValuedArgument('t', "str")]
            public string Str { get; set; }
        }



        [Test]
        public void TestIntVal()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s", "1"});

            Assert.AreEqual(actual: rv.Args.Some, expected: 1);
        }


        [Test]
        public void TestNegative()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s", "-1"});

            Assert.AreEqual(actual: rv.Args.Some, expected: -1);
        }


        [Test]
        public void TestIntLongVal()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-d", "11"});

            Assert.AreEqual(11L, rv.Args.Dummy);
        }


        [Test]
        public void TestDefVal()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"--def"});

            Assert.AreEqual(actual: rv.Args.Def, expected: 5);
        }


        [Test]
        public void TestString()
        {
            var val = "qewr23ыфа";
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-t", val});

            Assert.AreEqual(actual: rv.Args.Str, expected: val);
        }


        [Test]
        public void TestTypeCanNotParsed()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-s", "1.1"}));
        }


        [Test]
        public void TestNoDefault()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-s"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfFloating
        {
            [ValuedArgument('s')]
            public decimal Dec { get; set; }


            [ValuedArgument('d', "doub")]
            public double Doub { get; set; }


            [ValuedArgument('f', "flo")]
            public float Flo { get; set; }
        }



        [Test]
        public void TestFloating()
        {
            //var p = new CmdArgsParser(CultureInfo.GetCultureInfo("ru"));
            var p = new CmdArgsParser();
            Res<ConfFloating> rv = p.ParseCommandLine<ConfFloating>(new[]
                    {"-s", "1.1", "--doub", "1.123", "--flo", "-123.4534"});

            Assert.AreEqual(actual: rv.Args.Dec, expected: 1.1m);
            Assert.AreEqual(actual: rv.Args.Doub, expected: 1.123d);
            Assert.AreEqual(actual: rv.Args.Flo, expected: -123.4534f);
        }


        [Test]
        public void TestFloatingRus()
        {
            var p = new CmdArgsParser(CultureInfo.GetCultureInfo("ru"));
            //var p = new CmdArgsParser();
            Res<ConfFloating> rv = p.ParseCommandLine<ConfFloating>(new[]
                    {"-s", "1,1", "--doub", "1,123", "--flo", "-123,4534"});

            Assert.AreEqual(actual: rv.Args.Dec, expected: 1.1m);
            Assert.AreEqual(actual: rv.Args.Doub, expected: 1.123d);
            Assert.AreEqual(actual: rv.Args.Flo, expected: -123.4534f);
        }


        ////////////////////////////////////////////////////////////////



        class ConfChar
        {
            [ValuedArgument('c')] public char C;
        }



        [Test]
        public void TestChar()
        {
            var p = new CmdArgsParser();
            Res<ConfChar> rv = p.ParseCommandLine<ConfChar>(new[] {"-c", "u"});
            Assert.AreEqual(actual: rv.Args.C, expected: 'u');
        }
    }
}