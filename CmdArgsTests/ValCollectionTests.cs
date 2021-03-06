﻿#region usings
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
        ////////////////////////////////////////////////////////////////
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
            var p = new CmdArgsParser<ConfCollections>();
            Res<ConfCollections> res = p.ParseCommandLine(new string[] { });
        }


        [Test]
        public void TestIntOk()
        {
            var p = new CmdArgsParser<ConfCollections>();
            Res<ConfCollections> res =
                p.ParseCommandLine(new[] {"-a", "1", "2", "-l", "-3", "45"});

            Assert.IsTrue(new[] {1, 2}.SequenceEqual(res.Args.Array));
            Assert.IsTrue(new[] {-3, 45}.SequenceEqual(res.Args.List));
        }


        [Test]
        public void TestNoVal()
        {
            var p = new CmdArgsParser<ConfCollections>();
            Assert.Throws<CmdException>(() => p.ParseCommandLine(new[] {"-a"}));
        }



        ////////////////////////////////////////////////////////////////
        class ConfCollectionDef
        {
            [ValuedArgument('a', DefaultValue = 2)]
            public int[] Array { get; set; }


            [ValuedArgument('r', DefaultValue = new[] {2, 3}, UseDefWhenNoArg = true)]
            public int[] Array1 { get; set; }


            [ValuedArgument('l', DefaultValue = 2)]
            public List<int> List { get; set; }


            [ValuedArgument('s', DefaultValue = new[] {2, 3}, UseDefWhenNoArg = true)]
            public List<int> List1 { get; set; }
        }



        [Test]
        public void TestCollectionDefOk()
        {
            var p = new CmdArgsParser<ConfCollectionDef>();
            Res<ConfCollectionDef> res =
                p.ParseCommandLine(new[] {"-a", "-l"});

            Assert.IsTrue(new[] {2}.SequenceEqual(res.Args.Array));
            Assert.IsTrue(new[] {2, 3}.SequenceEqual(res.Args.Array1));
            Assert.IsTrue(new[] {2}.SequenceEqual(res.Args.List));
            Assert.IsTrue(new[] {2, 3}.SequenceEqual(res.Args.List1));
        }



        ////////////////////////////////////////////////////////////////
        class ConfOne
        {
            [ValuedArgument('i')]
            public int I { get; set; }
        }



        [Test]
        public void TestOne()
        {
            var p = new CmdArgsParser<ConfOne>();
            Res<ConfOne> r = p.ParseCommandLine(new[] {"-i", "2", "3"});
            Assert.AreEqual(2, r.Args.I);
            Assert.IsTrue(new[] {"3"}.SequenceEqual(r.AdditionalArguments));
        }



        ////////////////////////////////////////////////////////////////
        class ConfDefPredicateBad
        {
            [ValuedArgument('a', DefaultValue = 2)]
            public int[] Array { get; set; }


            // ReSharper disable once UnusedMember.Local
            public static Predicate<int> Array_Predicate = x => x > 10;
        }



        [Test]
        public void TestDefPredicateBad()
        {
            var p = new CmdArgsParser<ConfDefPredicateBad>();
            Assert.Throws<ConfException>(
                () => p.ParseCommandLine(new[] {"-i"}));
        }



        ////////////////////////////////////////////////////////////////
        class ConfDefPredicateCollectionBad
        {
            [ValuedArgument('a', DefaultValue = 2)]
            public int[] Array { get; set; }


            // ReSharper disable once UnusedMember.Local
            public static Predicate<int[]> Array_Predicate = x => x.Length > 1;
        }



        [Test]
        public void TestDefPredicateCollectionBad()
        {
            var p = new CmdArgsParser<ConfDefPredicateCollectionBad>();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine(new[] {"-i"}));
        }


        ////////////////////////////////////////////////////////////////
    }
}