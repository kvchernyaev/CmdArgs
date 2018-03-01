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
    public class UnstrictlyConfTests
    {
        ////////////////////////////////////////////////////////////////
        class ConfWrongFieldType
        {
            [UnstrictlyConfArgument('D', "define")]
            public int Uns;
        }



        [Test]
        public void TestWrongFieldType()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfWrongFieldType>(new[] {"-Dname=val"}));
        }



        ////////////////////////////////////////////////////////////////
        class ConfOkOne
        {
            [UnstrictlyConfArgument('D', "define")]
            public UnstrictlyConf Uns;
        }



        [Test]
        public void TestOkOne()
        {
            var p = new CmdArgsParser();

            Res<ConfOkOne> r = p.ParseCommandLine<ConfOkOne>(new[] {"-Dname=val"});

            Assert.IsNotNull(r.Args.Uns);
            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);
        }


        [Test]
        public void TestLongName()
        {
            var p = new CmdArgsParser();

            Res<ConfOkOne> r = p.ParseCommandLine<ConfOkOne>(new[] {"--DEFINEname=val"});

            Assert.IsNotNull(r.Args.Uns);
            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);
        }


        [Test]
        public void TestAdditional()
        {
            var p = new CmdArgsParser();
            p.AllowAdditionalArguments = true;
            Res<ConfOkOne> r =
                p.ParseCommandLine<ConfOkOne>(new[] {"-Dname=val", "additional", "--unknown"});

            Assert.IsNotNull(r.Args.Uns);
            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);

            Assert.IsTrue(new[] {"additional"}.SequenceEqual(r.AdditionalArguments));
        }


        [Test]
        public void TestNullValue()
        {
            var p = new CmdArgsParser();

            Res<ConfOkOne> r = p.ParseCommandLine<ConfOkOne>(new[] {"--DEFINEname"});

            Assert.IsNotNull(r.Args.Uns);
            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.IsNull(r.Args.Uns[0].Value);
        }


        [Test]
        public void TestEmptyValue()
        {
            var p = new CmdArgsParser();

            Res<ConfOkOne> r = p.ParseCommandLine<ConfOkOne>(new[] {"--DEFINEname="});

            Assert.IsNotNull(r.Args.Uns);
            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("", r.Args.Uns[0].Value);
        }


        [Test]
        public void TestEmpty()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfOkOne>(new[] {"-D"}));
        }


        [Test]
        public void TestOnlyEq()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfOkOne>(new[] {"-D="}));
        }


        [Test]
        public void TestEmptyName()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfOkOne>(new[] {"-D=val"}));
        }


        [Test]
        public void TestLongEmpty()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfOkOne>(new[] {"--DEFINE"}));
        }


        [Test]
        public void TestLongOnlyEq()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfOkOne>(new[] { "--DEFINE=" }));
        }


        [Test]
        public void TestLongEmptyName()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfOkOne>(new[] { "--DEFINE=val" }));
        }



        ////////////////////////////////////////////////////////////////
        class ConfTwo
        {
            [UnstrictlyConfArgument('D', "define")]
            public UnstrictlyConf Uns;


            [UnstrictlyConfArgument('M', "man")] public UnstrictlyConf Man;
        }



        [Test]
        public void TestTwo()
        {
            var p = new CmdArgsParser();

            Res<ConfTwo> r = p.ParseCommandLine<ConfTwo>(new[]
                    {"-Dname=val", "-Mwe=34", "-Dzzxcv=gf", "--MANcv=123"});

            Assert.IsNotNull(r.Args.Uns);
            Assert.AreEqual(2, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);
            Assert.AreEqual("zzxcv", r.Args.Uns[1].Name);
            Assert.AreEqual("gf", r.Args.Uns[1].Value);

            Assert.IsNotNull(r.Args.Man);
            Assert.AreEqual(2, r.Args.Man.Count);
            Assert.AreEqual("we", r.Args.Man[0].Name);
            Assert.AreEqual("34", r.Args.Man[0].Value);
            Assert.AreEqual("cv", r.Args.Man[1].Name);
            Assert.AreEqual("123", r.Args.Man[1].Value);
        }


        ////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////
    }
}