#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
using NUnit.Framework.Internal;
#endregion



namespace CmdArgsTests
{
    [TestFixture]
    public class AdditionalArgumentsTests
    {
        ////////////////////////////////////////////////////////////////
        class Conf
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some;


            [SwitchArgument('d', "dummy", "dummy description")]
            public bool Dummy;
        }



        static void Check(Res<Conf> rv, bool s, bool d, string[] addit = null)
        {
            Assert.AreEqual(rv.Args.Dummy, d);
            Assert.AreEqual(rv.Args.Some, s);
            Assert.IsEmpty(rv.UnknownArguments);
            if (addit == null)
                Assert.IsEmpty(rv.AdditionalArguments);
            else
                Assert.IsTrue(addit.SequenceEqual(rv.AdditionalArguments));
        }


        [Test]
        public void TestAdditional()
        {
            var p = new CmdArgsParser();
            p.AllowAdditionalArguments = true;
            p.UseEqualitySyntax = true;
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-ds", "addval"});
            Check(rv, s: true, d: true, addit: new[] {"addval"});
        }


        [Test]
        public void TestAdditionalAllowfalse()
        {
            var p = new CmdArgsParser();
            p.AllowAdditionalArguments = false;
            p.UseEqualitySyntax = true;
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-ds", "addval"}));
        }


        [Test]
        public void TestAdditionalEqfalse()
        {
            var p = new CmdArgsParser();
            p.AllowAdditionalArguments = true;
            p.UseEqualitySyntax = false;
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-ds", "addval"}));
        }


        [Test]
        public void TestAdditionalFirst()
        {
            var p = new CmdArgsParser();
            p.AllowAdditionalArguments = true;
            p.UseEqualitySyntax = true;
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"addval", "-ds",});
            Check(rv, s: true, d: true, addit: new[] {"addval"});
        }


        ////////////////////////////////////////////////////////////////
    }
}