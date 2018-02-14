#region usings
using System;
using System.Collections;
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
        class ConfCollections
        {
            [ValuedArgument('a')]
            public int[] Array { get; set; }


            [ValuedArgument('l')]
            public List<int> List { get; set; }
        }



        [Test]
        public void TestCollections()
        {
            var p = new CmdArgsParser();
            Res<ConfCollections> res = p.ParseCommandLine<ConfCollections>(new string[] { });
        }


        [Test]
        public void TestIntOk()
        {
            var p = new CmdArgsParser();
            Res<ConfCollections> res =
                p.ParseCommandLine<ConfCollections>(new[] {"-a", "1", "2", "-l", "-3", "45"});

            Assert.IsTrue(new[] {1, 2}.SequenceEqual(res.Args.Array));
            Assert.IsTrue(new[] {-3, 45}.SequenceEqual(res.Args.List));
        }


        [Test]
        public void TestNoVal()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<ConfCollections>(new[] {"-a"}));
        }



        class ConfOne
        {
            [ValuedArgument('i')]
            public int I { get; set; }
        }



        [Test]
        public void TestOne()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<ConfOne>(new[] {"-i", "2", "3"}));
        }
    }
}