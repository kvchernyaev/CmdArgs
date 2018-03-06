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
    public class DirTests
    {
        //////////////////////////////////////////////////////////////
        class ConfDir
        {
            [DirArgument('f')] public DirectoryInfo Dir;
        }



        [Test]
        public void TestDirOk()
        {
            var p = new CmdArgsParser<ConfDir>();
            Res<ConfDir> res = p.ParseCommandLine(new[] {"-f", "./asdf"});
            Assert.AreEqual("asdf", res.Args.Dir.Name);
        }


        [Test]
        public void TestDirWrongUrl()
        {
            var p = new CmdArgsParser<ConfDir>();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-f", "./asdf:asdf"}));
        }


        [Test]
        public void TestDirWrongUrl1()
        {
            var p = new CmdArgsParser<ConfDir>();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-f", "./asdf\""}));
        }



        //////////////////////////////////////////////////////////////
        class ConfDirEx
        {
            [DirArgument('f', MustExists = true)] public DirectoryInfo Dir;
        }



        [Test]
        public void TestDirExists()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            var fi = new FileInfo(a.Location);

            var p = new CmdArgsParser<ConfDirEx>();
            Res<ConfDirEx> res = p.ParseCommandLine(new[] {"-f", fi.DirectoryName});
            Assert.AreEqual(true, res.Args.Dir.Exists);
            Assert.AreEqual(fi.DirectoryName, res.Args.Dir.FullName);
        }


        [Test]
        public void TestDirNotExists()
        {
            var p = new CmdArgsParser<ConfDirEx>();
            Assert.Throws<CmdException>(
                () => p.ParseCommandLine(new[] {"-f", "./asdf"}));
        }



        //////////////////////////////////////////////////////////////
        class ConfDirDefaultValue
        {
            [DirArgument('f', DefaultValue = "./somename")]
            public DirectoryInfo Dir;
        }



        [Test]
        public void TestDirDefaultValueOk()
        {
            var p = new CmdArgsParser<ConfDirDefaultValue>();
            Res<ConfDirDefaultValue> r = p.ParseCommandLine(new[] {"-f"});
            Assert.AreEqual("somename", r.Args.Dir.Name);
        }



        ////////////////////////////////////////////////////////////////
        class ConfDirAllowedValuesBadType
        {
            [DirArgument('f', AllowedValues = new object[] {"./asdf", 2})]
            public DirectoryInfo Dir;
        }



        [Test]
        public void TestDirAllowedValuesBadType()
        {
            var p = new CmdArgsParser<ConfDirAllowedValuesBadType>();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine(new[] {"-f", "./asdf"}));
        }



        ////////////////////////////////////////////////////////////////
        class ConfDirAllowedValues
        {
            [DirArgument('f', AllowedValues = new object[] {"./asdf", "somename"})]
            public DirectoryInfo Dir;
        }



        [Test]
        public void TestDirAllowedValuesOk()
        {
            var p = new CmdArgsParser<ConfDirAllowedValues>();
            // test not exactly equality
            Res<ConfDirAllowedValues> res =
                p.ParseCommandLine(new[] {"-f", "./somename"});
            var fi = new DirectoryInfo("./somename");
            Assert.AreEqual(fi.FullName, res.Args.Dir.FullName);
        }


        [Test]
        public void TestDirAllowedValuesBad()
        {
            var p = new CmdArgsParser<ConfDirAllowedValues>();
            // test not exactly equality
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-f", "./notexists"}));
        }


        //////////////////////////////////////////////////////////////
    }
}