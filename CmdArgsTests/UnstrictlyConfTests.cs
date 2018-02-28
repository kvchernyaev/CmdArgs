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



        class Conf
        {
            [UnstrictlyConfArgument('D', "define")] public UnstrictlyConf Uns;
        }



        [Test]
        public void Test()
        {
            var p = new CmdArgsParser();

            Res<Conf> r = p.ParseCommandLine<Conf>(new[] {"-Dname=val"});

            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);
        }


        [Test]
        public void TestLongName()
        {
            var p = new CmdArgsParser();

            Res<Conf> r = p.ParseCommandLine<Conf>(new[] {"--DEFINEname=val"});

            Assert.AreEqual(1, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);
        }


        ////////////////////////////////////////////////////////////////



        class ConfTwo
        {
            [UnstrictlyConfArgument('D', "define")] public UnstrictlyConf Uns;
            [UnstrictlyConfArgument('M', "man")] public UnstrictlyConf Man;
        }



        [Test]
        public void TestTwo()
        {
            var p = new CmdArgsParser();

            Res<ConfTwo> r = p.ParseCommandLine<ConfTwo>(new[]
                    {"-Dname=val", "-Mwe=34", "-Dzzxcv=gf", "--MANcv=123"});

            Assert.AreEqual(2, r.Args.Uns.Count);
            Assert.AreEqual("name", r.Args.Uns[0].Name);
            Assert.AreEqual("val", r.Args.Uns[0].Value);
            Assert.AreEqual("zzxcv", r.Args.Uns[1].Name);
            Assert.AreEqual("gf", r.Args.Uns[1].Value);

            Assert.AreEqual(2, r.Args.Man.Count);
            Assert.AreEqual("we", r.Args.Man[0].Name);
            Assert.AreEqual("34", r.Args.Man[0].Value);
            Assert.AreEqual("cv", r.Args.Man[1].Name);
            Assert.AreEqual("123", r.Args.Man[1].Value);
        }


        ////////////////////////////////////////////////////////////////
    }
}