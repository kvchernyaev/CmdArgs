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
        [Test]
        public void TestTypeNotMatch()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() => p.ParseCommandLine<ConfTypeNotMatch>(new[] {"-s"}));
        }



        class ConfTypeNotMatch
        {
            [ValuedArgument(typeof(int), 's', "some")]
            public bool Some { get; set; }
        }



        [Test]
        public void TestDefTypeNotMatch()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(
                () => p.ParseCommandLine<ConfDefTypeNotMatch>(new[] {"-s"}));
        }



        class ConfDefTypeNotMatch
        {
            [ValuedArgument(typeof(int), 's', "some", DefaultValue = "1")]
            public int Some { get; set; }
        }



        [Test]
        public void TestIntVal()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s", "1"});

            Assert.AreEqual(rv.Args.Some, 1);
        }


        [Test]
        public void TestIntLongVal()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-d", "11"});

            Assert.AreEqual(rv.Args.Dummy, 11);
        }


        [Test]
        public void TestDefVal()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"--def"});

            Assert.AreEqual(rv.Args.Def, 5);
        }


        [Test]
        public void TestString()
        {
            var val = "qewr23ыфа";
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-t", val});

            Assert.AreEqual(rv.Args.Str, val);
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



        class Conf
        {
            [ValuedArgument(typeof(int), 's', "some")]
            public int Some { get; set; }


            [ValuedArgument(typeof(int), 'e', "def", DefaultValue = 5)]
            public int Def { get; set; }


            [ValuedArgument(typeof(int), 'd', "dummy")]
            public long Dummy { get; set; }


            [ValuedArgument(typeof(string), 't', "str")]
            public string Str { get; set; }
        }



        [Test]
        public void TestFloating()
        {
            //var p = new CmdArgsParser(CultureInfo.GetCultureInfo("ru"));
            var p = new CmdArgsParser();
            Res<ConfFloating> rv = p.ParseCommandLine<ConfFloating>(new[]
                    {"-s", "1.1", "--dec", "1.123", "--flo", "-123.4534"});

            Assert.AreEqual(rv.Args.Some, 1.1m);
            Assert.AreEqual(rv.Args.Flo, -123.4534f);
            Assert.AreEqual(rv.Args.Dec, 1.123m);
        }



        class ConfFloating
        {
            [ValuedArgument(typeof(double), 's')]
            public decimal Some { get; set; }


            [ValuedArgument(typeof(decimal), 'd', "dec")]
            public decimal Dec { get; set; }


            [ValuedArgument(typeof(float), 'f', "flo")]
            public float Flo { get; set; }
        }
    }
}