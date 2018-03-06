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
            [ArgsFileArgument('d', "argsFile")]
            //[FileArgument('d', "argsFile", MustExists = true)]
            public FileInfo ArgsFile;


            [ValuedArgument('s')] public string S;
            [ValuedArgument('t')] public string T;
            [ValuedArgument('u')] public string U;
        }



        static string ArgsConfFilepath
        {
            get
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string filepath = Path.Combine(new FileInfo(a.Location).DirectoryName, "args.conf");
                return filepath;
            }
        }


        [Test]
        public void TestSimple()
        {
            var p = new CmdArgsParser();

            Res<Conf> r = p.ParseCommandLine<Conf>(new[] {"-d", ArgsConfFilepath});
            Assert.AreEqual("asdffs", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        [Test]
        public void TestSimpleBefore()
        {
            var p = new CmdArgsParser();

            Res<Conf> r = p.ParseCommandLine<Conf>(new[] {"-s=nm", "-d", ArgsConfFilepath});
            Assert.AreEqual("asdffs", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        [Test]
        public void TestSimpleAfter()
        {
            var p = new CmdArgsParser();

            Res<Conf> r = p.ParseCommandLine<Conf>(new[] {"-d", ArgsConfFilepath, "-s=nm"});
            Assert.AreEqual("nm", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        [Test]
        public void TestDouble()
        {
            var p = new CmdArgsParser();

            Res<Conf> r = p.ParseCommandLine<Conf>(new[]
                    {"-d", ArgsConfFilepath, "-s=nm", "-d", ArgsConfFilepath,});
            Assert.AreEqual("nm", r.Args.S);
            Assert.AreEqual("qwer", r.Args.T);
            Assert.AreEqual("uio 89", r.Args.U);
        }


        ////////////////////////////////////////////////////////////////
    }
}