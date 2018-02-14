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
    public class ValCollectionTests
    {
        [Test]
        public void TestIntOk()
        {
            var p = new CmdArgsParser();
            Res<Conf> res = p.ParseCommandLine<Conf>(new[] {"-i", "1", "2"});

            Assert.IsTrue(new[] {1, 2}.SequenceEqual(res.Args.I));
        }


        [Test]
        public void TestNoVal()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] {"-i"}));
        }



        class Conf
        {
            [ValuedArgument(typeof(int), 'i')]
            public int[] I { get; set; }
        }



        [Test]
        public void TestOne()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<Conf>(new[] { "-i", "2", "3" }));
        }


        class ConfOne
        {
            [ValuedArgument(typeof(int), 'i')]
            public int I { get; set; }
        }
    }
}