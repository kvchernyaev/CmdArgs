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
            var p = new CmdArgsParser();
            Res<ConfDir> res = p.ParseCommandLine<ConfDir>(new[] {"-f", "./asdf"});
            Assert.AreEqual("asdf", res.Args.Dir.Name);
        }


        [Test]
        public void TestDirWrongUrl()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfDir>(new[] {"-f", "./asdf:asdf"}));
        }


        [Test]
        public void TestDirWrongUrl1()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfDir>(new[] {"-f", "./asdf\""}));
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
            FileInfo fi = new FileInfo(a.Location);

            var p = new CmdArgsParser();
            Res<ConfDirEx> res = p.ParseCommandLine<ConfDirEx>(new[] {"-f", fi.DirectoryName});
            Assert.AreEqual(true, res.Args.Dir.Exists);
            Assert.AreEqual(fi.DirectoryName, res.Args.Dir.FullName);
        }


        [Test]
        public void TestDirNotExists()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(
                () => p.ParseCommandLine<ConfDirEx>(new[] {"-f", "./asdf"}));
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
            var p = new CmdArgsParser();
            Res<ConfDirDefaultValue> r = p.ParseCommandLine<ConfDirDefaultValue>(new[] {"-f"});
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
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfDirAllowedValuesBadType>(new[] {"-f", "./asdf"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfDirAllowedValues
        {
            [DirArgument('f', AllowedValues = new object[] {"./asdf", "somename" })]
            public DirectoryInfo Dir;
        }



        [Test]
        public void TestDirAllowedValuesOk()
        {
            var p = new CmdArgsParser();
            // test not exactly equality
            Res<ConfDirAllowedValues> res =
                p.ParseCommandLine<ConfDirAllowedValues>(new[] {"-f", "./somename" });
            var fi = new DirectoryInfo("./somename");
            Assert.AreEqual(fi.FullName, res.Args.Dir.FullName);
        }


        [Test]
        public void TestDirAllowedValuesBad()
        {
            var p = new CmdArgsParser();
            // test not exactly equality
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfDirAllowedValues>(new[] {"-f", "./notexists"}));
        }


        //////////////////////////////////////////////////////////////
    }
}