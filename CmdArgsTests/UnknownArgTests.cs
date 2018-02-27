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
    public class UnknownArgTests
    {
        [Test]
        public void TestForbidShortname()
        {
            var p = new CmdArgsParser {AllowUnknownArguments = false};
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-u"}));
        }


        [Test]
        public void TestForbidLongname()
        {
            var p = new CmdArgsParser {AllowUnknownArguments = false};
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"--unk"}));
        }


        [Test]
        public void TestShortname()
        {
            var p = new CmdArgsParser {AllowUnknownArguments = true};

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-u"});
            Check(rv, s: false, d: false, unk: "u");
        }


        [Test]
        public void TestLongname()
        {
            var p = new CmdArgsParser {AllowUnknownArguments = true};

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"--unk"});
            Check(rv, s: false, d: false, unk: "unk");
        }


        [Test]
        public void TestValues()
        {
            var p = new CmdArgsParser {AllowUnknownArguments = true};

            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"--unk", "val1", "val2"});
            Check(rv, s: false, d: false, unk: "unk", vals: new[] {"val1", "val2"});
        }


        static void Check(Res<Conf> rv, bool s, bool d, string unk, string[] vals = null)
        {
            Assert.AreEqual(rv.Args.Dummy, d);
            Assert.AreEqual(rv.Args.Some, s);
            Assert.IsEmpty(rv.AdditionalArguments);
            Assert.IsNotEmpty(rv.UnknownArguments);
            Assert.AreEqual(rv.UnknownArguments.Count, 1);
            Assert.AreEqual(rv.UnknownArguments[0].Item1, unk);
            if (vals == null || vals.Length == 0)
                Assert.IsEmpty(rv.UnknownArguments[0].Item2);
            else
            {
                string[] v = rv.UnknownArguments[0].Item2;
                Assert.IsTrue(v != null && v.Length == vals.Length && v.SequenceEqual(vals));
            }
        }



        class Conf
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some { get; set; }


            [SwitchArgument('d', "dummy", "dummy description")]
            public bool Dummy { get; set; }
        }
    }
}