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
    public class FileTests
    {
        //////////////////////////////////////////////////////////////
        class ConfWrongFieldType
        {
            [FileArgument('f')] public string File;
        }



        [Test]
        public void TestWrongFieldType()
        {
            var p = new CmdArgsParser<ConfWrongFieldType>();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine(new[] {"-f", "./sadf"}));
        }



        //////////////////////////////////////////////////////////////
        class ConfWrongArgType
        {
            [ValuedArgument('f')] public FileInfo File;
        }



        [Test]
        public void TestWrongArgType()
        {
            var p = new CmdArgsParser<ConfWrongArgType>();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine(new string[] { }));
        }



        //////////////////////////////////////////////////////////////
        class ConfFile
        {
            [FileArgument('f')] public FileInfo File;
        }



        [Test]
        public void TestFileOk()
        {
            var p = new CmdArgsParser<ConfFile>();
            Res<ConfFile> res = p.ParseCommandLine(new[] {"-f", "./asdf"});
            Assert.AreEqual("asdf", res.Args.File.Name);
        }


        [Test]
        public void TestFileWrongUrl()
        {
            var p = new CmdArgsParser<ConfFile>();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-f", "./asdf:asdf"}));
        }


        [Test]
        public void TestFileWrongUrl1()
        {
            var p = new CmdArgsParser<ConfFile>();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-f", "./asdf\""}));
        }



        //////////////////////////////////////////////////////////////
        class ConfFileEx
        {
            [FileArgument('f', MustExists = true)] public FileInfo File;
        }



        [Test]
        public void TestFileExists()
        {
            Assembly a = Assembly.GetExecutingAssembly();

            var p = new CmdArgsParser<ConfFileEx>();
            Res<ConfFileEx> res = p.ParseCommandLine(new[] {"-f", a.Location});
            Assert.AreEqual(true, res.Args.File.Exists);
            Assert.AreEqual(a.Location, res.Args.File.FullName);
        }


        [Test]
        public void TestFileNotExists()
        {
            var p = new CmdArgsParser<ConfFileEx>();
            Assert.Throws<CmdException>(
                () => p.ParseCommandLine(new[] {"-f", "./asdf"}));
        }



        //////////////////////////////////////////////////////////////
        class ConfFileDefaultValue
        {
            [FileArgument('f', DefaultValue = "./prog.conf")]
            public FileInfo File;
        }



        [Test]
        public void TestFileDefaultValueOk()
        {
            var p = new CmdArgsParser<ConfFileDefaultValue>();
            Res<ConfFileDefaultValue> r = p.ParseCommandLine(new[] {"-f"});
            Assert.AreEqual("prog.conf", r.Args.File.Name);
        }



        ////////////////////////////////////////////////////////////////
        class ConfFileAllowedValuesBadType
        {
            [FileArgument('f', AllowedValues = new object[] {"./asdf", 2})]
            public FileInfo File;
        }



        [Test]
        public void TestFileAllowedValuesBadType()
        {
            var p = new CmdArgsParser<ConfFileAllowedValuesBadType>();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine(new[] {"-f", "./asdf"}));
        }



        ////////////////////////////////////////////////////////////////
        class ConfFileAllowedValues
        {
            [FileArgument('f', AllowedValues = new object[] {"./asdf", "a.conf"})]
            public FileInfo File;
        }



        [Test]
        public void TestFileAllowedValuesOk()
        {
            var p = new CmdArgsParser<ConfFileAllowedValues>();
            // test not exactly equality
            Res<ConfFileAllowedValues> res =
                p.ParseCommandLine(new[] {"-f", "./a.conf"});
            var fi = new FileInfo("./a.conf");
            Assert.AreEqual(fi.FullName, res.Args.File.FullName);
        }


        [Test]
        public void TestFileAllowedValuesBad()
        {
            var p = new CmdArgsParser<ConfFileAllowedValues>();
            // test not exactly equality
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine(new[] {"-f", "./a.confNO"}));
        }



        //////////////////////////////////////////////////////////////
        class ConfArray
        {
            [FileArgument('f')] public FileInfo[] Files;
        }



        [Test]
        public void TestArray()
        {
            var p = new CmdArgsParser<ConfArray>();
            Res<ConfArray> res =
                p.ParseCommandLine(new[] {"-f", "./a.conf", "./b.conf"});
            var fi = new FileInfo("./a");
            string path = fi.DirectoryName;
            Assert.IsTrue(res.Args.Files.Select(x => x.FullName)
                .SequenceEqual(new[] {Path.Combine(path, "a.conf"), Path.Combine(path, "b.conf")}));
        }



        //////////////////////////////////////////////////////////////
        class ConfList
        {
            [FileArgument('f')] public List<FileInfo> Files;
        }



        [Test]
        public void TestList()
        {
            var p = new CmdArgsParser<ConfList>();
            Res<ConfList> res = p.ParseCommandLine(new[] {"-f", "./a.conf", "./b.conf"});
            var fi = new FileInfo("./a");
            string path = fi.DirectoryName;
            Assert.IsTrue(res.Args.Files.Select(x => x.FullName)
                .SequenceEqual(new[] {Path.Combine(path, "a.conf"), Path.Combine(path, "b.conf")}));
        }


        //////////////////////////////////////////////////////////////
    }
}