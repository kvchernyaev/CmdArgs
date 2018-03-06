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
    public class MandatoryAndMultipleTests
    {
        class Conf
        {
            [SwitchArgument('s', "some", "", Mandatory = true)]
            public bool Some { get; set; }


            [SwitchArgument('m', "mult", "", AllowMultiple = true)]
            public bool Mult { get; set; }


            [SwitchArgument('n', "nomult", "", AllowMultiple = false)]
            public bool NoMult { get; set; }
        }



        [Test]
        public void TestNo()
        {
            var p = new CmdArgsParser<Conf>();
            Assert.Throws<CmdException>(() => p.ParseCommandLine(new string[] { }));
        }


        [Test]
        public void TestYes()
        {
            var p = new CmdArgsParser<Conf>();
            Res<Conf> rv = p.ParseCommandLine(new[] {"-s"});
            Check(rv, true, false, false);
        }


        [Test]
        public void TestNoMult()
        {
            var p = new CmdArgsParser<Conf>();
            Res<Conf> rv = p.ParseCommandLine(new[] {"-s", "--nomult"});
            Check(rv, true, true, false);
        }


        [Test]
        public void TestNoMult2()
        {
            var p = new CmdArgsParser<Conf>();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"--nomult", "--nomult"}));
        }


        [Test]
        public void TestMult1()
        {
            var p = new CmdArgsParser<Conf>();
            Res<Conf> rv = p.ParseCommandLine(new[] {"-s", "--mult"});
            Check(rv, true, false, true);
        }


        [Test]
        public void TestMult2()
        {
            var p = new CmdArgsParser<Conf>();
            Res<Conf> rv = p.ParseCommandLine(new[] {"-s", "--mult", "--mult"});
            Check(rv, true, false, true);
        }


        [Test]
        public void TestMult3()
        {
            var p = new CmdArgsParser<Conf>();
            Res<Conf> rv = p.ParseCommandLine(new[] {"-s", "--mult", "--mult", "--mult"});
            Check(rv, true, false, true);
        }


        static void Check(Res<Conf> rv, bool s, bool n, bool m, string unk = null,
            string[] vals = null)
        {
            Assert.AreEqual(rv.Args.Mult, m);
            Assert.AreEqual(rv.Args.NoMult, n);
            Assert.AreEqual(rv.Args.Some, s);

            Assert.IsEmpty(rv.AdditionalArguments);

            if (string.IsNullOrWhiteSpace(unk))
                Assert.IsEmpty(rv.UnknownArguments);
            else
            {
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
        }
    }
}