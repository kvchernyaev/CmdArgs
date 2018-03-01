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
    public class TrivialTests
    {
        ////////////////////////////////////////////////////////////////
        class Conf { }



        [Test]
        public void Test()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new string[] { });
        }


        [Test]
        public void TestM()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new string[] {"-"});
            Assert.IsTrue(new[] {"-"}.SequenceEqual(rv.AdditionalArguments));
        }


        [Test]
        public void TestM1()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new string[] {"-1"});
            Assert.IsTrue(new[] {"-1"}.SequenceEqual(rv.AdditionalArguments));
        }


        [Test]
        public void TestMM()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new string[] {"--"});
            Assert.IsTrue(new[] {"--"}.SequenceEqual(rv.AdditionalArguments));
        }


        [Test]
        public void TestMM1()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new string[] {"--1"});
            Assert.IsTrue(new[] {"--1"}.SequenceEqual(rv.AdditionalArguments));
        }
    }
}