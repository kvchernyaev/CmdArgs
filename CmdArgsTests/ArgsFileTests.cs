#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
using NUnit.Framework.Internal;
#endregion



namespace CmdArgsTests
{
    [TestFixture]
    public class ArgsFileTests
    {
        ////////////////////////////////////////////////////////////////
        class Conf
        {
            [ArgsFileArgument('d', "argsFile")] public FileInfo ArgsFile;


            [ValuedArgument('s')] public string S;
            [ValuedArgument('t')] public string T;
            [ValuedArgument('u')] public string U;
        }



        static string ArgsConfFilepath
        {
            get
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string filepath = Path.Combine(new FileInfo(a.Location).DirectoryName,
                    "ArgsFileTests.conf");
                return filepath;
            }
        }


        [Test]
        public void TestSimple()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[] {"-d", ArgsConfFilepath});
            Assert.AreEqual("asdffs", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        [Test]
        public void TestSimpleBefore()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[] {"-s=nm", "-d", ArgsConfFilepath});
            Assert.AreEqual("asdffs", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        [Test]
        public void TestSimpleAfter()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[] {"-d", ArgsConfFilepath, "-s=nm"});
            Assert.AreEqual("nm", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        [Test]
        public void TestDouble()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"-d", ArgsConfFilepath, "-s=nm", "-d", ArgsConfFilepath,});
            Assert.AreEqual("asdffs", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        ////////////////////////////////////////////////////////////////


        [Test]
        public void TestAdditionalOnlyBefore()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"addit", "2", "-d", ArgsConfFilepath});
            Assert.IsTrue(new[] {"addit", "2"}.SequenceEqual(r.AdditionalArguments));
        }


        [Test]
        public void TestAdditionalOnlyAfter()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"-d", ArgsConfFilepath, "addit", "2"});
            Assert.IsTrue(new[] {"addit", "2"}.SequenceEqual(r.AdditionalArguments));
        }


        static string ArgsConfAdditionalFilepath
        {
            get
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string filepath = Path.Combine(new FileInfo(a.Location).DirectoryName,
                    "ArgsFileTests_Additional.conf");
                return filepath;
            }
        }


        [Test]
        public void TestAdditionalOnlyIn()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[] {"-d", ArgsConfAdditionalFilepath});
            Assert.IsTrue(
                new[] {"addit1", "addit2", "addit3"}.SequenceEqual(r.AdditionalArguments));
        }


        [Test]
        public void TestAdditionalUnion()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"bef", "-d", ArgsConfAdditionalFilepath, "after"});
            Assert.IsTrue(
                new[] {"bef", "addit1", "addit2", "addit3", "after"}
                    .SequenceEqual(r.AdditionalArguments));
        }


        ////////////////////////////////////////////////////////////////
        // todo Unknown


        [Test]
        public void TestUnknownOnlyBefore()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"--unk", "-d", ArgsConfFilepath});
            Assert.AreEqual(1, r.UnknownArguments.Count);
            Assert.AreEqual("unk", r.UnknownArguments[0].Item1);
            Assert.IsEmpty(r.UnknownArguments[0].Item2);
        }


        [Test]
        public void TestUnknownOnlyAfter()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"-d", ArgsConfFilepath, "--unk"});
            Assert.AreEqual(1, r.UnknownArguments.Count);
            Assert.AreEqual("unk", r.UnknownArguments[0].Item1);
            Assert.IsEmpty(r.UnknownArguments[0].Item2);
        }


        static string ArgsConfUnknownFilepath
        {
            get
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string filepath = Path.Combine(new FileInfo(a.Location).DirectoryName,
                    "ArgsFileTests_Unknown.conf");
                return filepath;
            }
        }


        [Test]
        public void TestUnknownOnlyIn()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[] {"-d", ArgsConfUnknownFilepath});
            Assert.AreEqual(3, r.UnknownArguments.Count);
            Assert.AreEqual("unk1", r.UnknownArguments[0].Item1);
            Assert.AreEqual("unk2", r.UnknownArguments[1].Item1);
            Assert.AreEqual("unk3", r.UnknownArguments[2].Item1);

            Assert.IsEmpty(r.UnknownArguments[0].Item2);

            Assert.AreEqual(1, r.UnknownArguments[1].Item2.Length);
            Assert.AreEqual("var", r.UnknownArguments[1].Item2[0]);

            Assert.AreEqual(2, r.UnknownArguments[2].Item2.Length);
            Assert.AreEqual("var1", r.UnknownArguments[2].Item2[0]);
            Assert.AreEqual("var2", r.UnknownArguments[2].Item2[1]);
        }


        [Test]
        public void TestUnknownUnion()
        {
            var p = new CmdArgsParser<Conf>();

            Res<Conf> r = p.ParseCommandLine(new[]
                    {"--bef", "-d", ArgsConfUnknownFilepath, "--after"});
            Assert.AreEqual(5, r.UnknownArguments.Count);
            Assert.AreEqual("bef", r.UnknownArguments[0].Item1);
            Assert.AreEqual("unk1", r.UnknownArguments[1].Item1);
            Assert.AreEqual("unk2", r.UnknownArguments[2].Item1);
            Assert.AreEqual("unk3", r.UnknownArguments[3].Item1);
            Assert.AreEqual("after", r.UnknownArguments[4].Item1);

            Assert.IsEmpty(r.UnknownArguments[0].Item2);
            Assert.IsEmpty(r.UnknownArguments[1].Item2);
            Assert.IsEmpty(r.UnknownArguments[4].Item2);

            Assert.AreEqual(1, r.UnknownArguments[2].Item2.Length);
            Assert.AreEqual("var", r.UnknownArguments[2].Item2[0]);

            Assert.AreEqual(2, r.UnknownArguments[3].Item2.Length);
            Assert.AreEqual("var1", r.UnknownArguments[3].Item2[0]);
            Assert.AreEqual("var2", r.UnknownArguments[3].Item2[1]);
        }


        ////////////////////////////////////////////////////////////////


        // todo array args
        // todo Unstrictly
    }
}