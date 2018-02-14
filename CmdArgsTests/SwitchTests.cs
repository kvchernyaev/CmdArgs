#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
#endregion



namespace CmdArgsTests
{
    [TestFixture]
    public class SwitchTests
    {
        [Test]
        public void TestShortname()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s"});
            Check(rv, s: true, d: false);
        }


        [Test]
        public void TestShortnameCase()
        {
            var p = new CmdArgs.CmdArgsParser
                {
                    AllowUnknownArgument = false
                };

            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-S"}));
        }


        [Test]
        public void TestShortname2()
        {
            var p = new CmdArgs.CmdArgsParser
                {
                    AllowUnknownArgument = true
                };

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-d"});
            Check(rv, s: false, d: true);
        }


        [Test]
        public void TestLongname()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"--dummy"});
            Check(rv, s: false, d: true);
        }


        [Test]
        public void TestIgnoreCase()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"--DUMMY"});
            Check(rv, s: false, d: true);
        }


        [Test]
        public void TestTwoInOne()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-sd"});
            Check(rv, s: true, d: true);
        }


        [Test]
        public void TestTwoInOne1()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-ds"});
            Check(rv, s: true, d: true);
        }


        [Test]
        public void TestTwo()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s", "-d"});
            Check(rv, s: true, d: true);
        }


        [Test]
        public void TestVal()
        {
            var p = new CmdArgs.CmdArgsParser();

            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-s", "val"}));
        }


        [Test]
        public void TestVal2()
        {
            var p = new CmdArgs.CmdArgsParser();

            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<Conf>(new[] {"-s", "--dummy", "val"}));
        }


        static void Check(Res<Conf> rv, bool s, bool d)
        {
            Assert.AreEqual(rv.Args.Dummy, d);
            Assert.AreEqual(rv.Args.Some, s);
            Assert.IsEmpty(rv.UnknownArguments);
            Assert.IsEmpty(rv.AdditionalArguments);
        }


        [Test]
        public void TestFields()
        {
            var p = new CmdArgs.CmdArgsParser();

            Res<ConfFields> rv = p.ParseCommandLine<ConfFields>(new[] {"-s"});
            Check(rv, s: true, d: false);
        }


        static void Check(Res<ConfFields> rv, bool s, bool d)
        {
            Assert.AreEqual(rv.Args.Dummy, d);
            Assert.AreEqual(rv.Args.Some, s);
            Assert.IsEmpty(rv.UnknownArguments);
            Assert.IsEmpty(rv.AdditionalArguments);
        }



        class Conf
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some { get; set; }


            [SwitchArgument('d', "dummy", "dummy description")]
            public bool Dummy { get; set; }
        }



        class ConfFields
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some;


            [SwitchArgument('d', "dummy", "dummy description")]
            public bool Dummy;
        }
    }
}