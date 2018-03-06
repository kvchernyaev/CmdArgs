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
        ////////////////////////////////////////////////////////////////
        class ConfWrongType
        {
            [SwitchArgument('s')]
            public int Some { get; set; }
        }



        [Test]
        public void TestWrongType()
        {
            var p = new CmdArgsParser<ConfWrongType>();
            Assert.Throws<ConfException>(() => p.ParseCommandLine(new[] {"-a"}));
        }



        ////////////////////////////////////////////////////////////////
        class Conf
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some { get; set; }


            [SwitchArgument('d', "dummy", "dummy description")]
            public bool Dummy { get; set; }
        }



        [Test]
        public void TestShortname()
        {
            var p = new CmdArgsParser<Conf>();
            Res<Conf> rv = p.ParseCommandLine(new[] {"-s"});
            Check(rv, s: true, d: false);
        }


        [Test]
        public void TestShortnameCase()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>
                {
                    AllowUnknownArguments = false
                };

            Assert.Throws<CmdException>(() => p.ParseCommandLine(new[] {"-S"}));
        }


        [Test]
        public void TestShortname2()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>
                {
                    AllowUnknownArguments = true
                };

            Res<Conf> rv = p.ParseCommandLine(new[] {"-d"});
            Check(rv, s: false, d: true);
        }


        [Test]
        public void TestLongname()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>();

            Res<Conf> rv = p.ParseCommandLine(new[] {"--dummy"});
            Check(rv, s: false, d: true);
        }


        [Test]
        public void TestIgnoreCase()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>();

            Res<Conf> rv = p.ParseCommandLine(new[] {"--DUMMY"});
            Check(rv, s: false, d: true);
        }


        [Test]
        public void TestTwoInOne()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>();

            Res<Conf> rv = p.ParseCommandLine(new[] {"-sd"});
            Check(rv, s: true, d: true);
        }


        [Test]
        public void TestTwoInOne1()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>();

            Res<Conf> rv = p.ParseCommandLine(new[] {"-ds"});
            Check(rv, s: true, d: true);
        }


        [Test]
        public void TestTwo()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> rv = p.ParseCommandLine(new[] {"-s", "-d"});
            Check(rv, s: true, d: true);
        }


        [Test]
        public void TestVal()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>();

            Assert.Throws<CmdException>(() => p.ParseCommandLine(new[] {"-s", "val"}));
        }


        [Test]
        public void TestVal2()
        {
            var p = new CmdArgs.CmdArgsParser<Conf>();

            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-s", "--dummy", "val"}));
        }


        static void Check(Res<Conf> rv, bool s, bool d)
        {
            Assert.AreEqual(rv.Args.Dummy, d);
            Assert.AreEqual(rv.Args.Some, s);
            Assert.IsEmpty(rv.UnknownArguments);
            Assert.IsEmpty(rv.AdditionalArguments);
        }



        ////////////////////////////////////////////////////////////////
        class ConfFields
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some;


            [SwitchArgument('d', "dummy", "dummy description")]
            public bool Dummy;
        }



        [Test]
        public void TestFields()
        {
            var p = new CmdArgsParser<ConfFields>();

            Res<ConfFields> rv = p.ParseCommandLine(new[] {"-s"});
            Check(rv, s: true, d: false);
        }


        static void Check(Res<ConfFields> rv, bool s, bool d, string[] addit = null)
        {
            Assert.AreEqual(rv.Args.Dummy, d);
            Assert.AreEqual(rv.Args.Some, s);
            Assert.IsEmpty(rv.UnknownArguments);
            if (addit == null)
                Assert.IsEmpty(rv.AdditionalArguments);
            else
                Assert.IsTrue(addit.SequenceEqual(rv.AdditionalArguments));
        }
    }
}