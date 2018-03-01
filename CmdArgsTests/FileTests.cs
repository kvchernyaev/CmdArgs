﻿#region usings
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
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfWrongFieldType>(new[] {"-f", "./sadf"}));
        }



        //////////////////////////////////////////////////////////////
        class ConfWrongArgType
        {
            [ValuedArgument('f')] public FileInfo File;
        }



        [Test]
        public void TestWrongArgType()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfWrongArgType>(new string[] { }));
        }



        //////////////////////////////////////////////////////////////
        class ConfFile
        {
            [FileArgument('f')] public FileInfo File;
        }



        [Test]
        public void TestFileOk()
        {
            var p = new CmdArgsParser();
            Res<ConfFile> res = p.ParseCommandLine<ConfFile>(new[] {"-f", "./asdf"});
            Assert.AreEqual("asdf", res.Args.File.Name);
        }


        [Test]
        public void TestFileWrongUrl()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfFile>(new[] {"-f", "./asdf:asdf"}));
        }


        [Test]
        public void TestFileWrongUrl1()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfFile>(new[] {"-f", "./asdf\""}));
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

            var p = new CmdArgsParser();
            Res<ConfFileEx> res = p.ParseCommandLine<ConfFileEx>(new[] {"-f", a.Location});
            Assert.AreEqual(true, res.Args.File.Exists);
            Assert.AreEqual(a.Location, res.Args.File.FullName);
        }


        [Test]
        public void TestFileNotExists()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(
                () => p.ParseCommandLine<ConfFileEx>(new[] {"-f", "./asdf"}));
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
            var p = new CmdArgsParser();
            Res<ConfFileDefaultValue> r = p.ParseCommandLine<ConfFileDefaultValue>(new[] {"-f"});
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
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfFileAllowedValuesBadType>(new[] {"-f", "./asdf"}));
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
            var p = new CmdArgsParser();
            // test not exactly equality
            Res<ConfFileAllowedValues> res =
                p.ParseCommandLine<ConfFileAllowedValues>(new[] {"-f", "./a.conf"});
            var fi = new FileInfo("./a.conf");
            Assert.AreEqual(fi.FullName, res.Args.File.FullName);
        }


        [Test]
        public void TestFileAllowedValuesBad()
        {
            var p = new CmdArgsParser();
            // test not exactly equality
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfFileAllowedValues>(new[] {"-f", "./a.confNO"}));
        }



        //////////////////////////////////////////////////////////////
        class ConfArray
        {
            [FileArgument('f')] public FileInfo[] Files;
        }



        [Test]
        public void TestArray()
        {
            var p = new CmdArgsParser();
            Res<ConfArray> res =
                p.ParseCommandLine<ConfArray>(new[] {"-f", "./a.conf", "./b.conf"});
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
            var p = new CmdArgsParser();
            Res<ConfList> res = p.ParseCommandLine<ConfList>(new[] {"-f", "./a.conf", "./b.conf"});
            var fi = new FileInfo("./a");
            string path = fi.DirectoryName;
            Assert.IsTrue(res.Args.Files.Select(x => x.FullName)
                .SequenceEqual(new[] {Path.Combine(path, "a.conf"), Path.Combine(path, "b.conf")}));
        }


        //////////////////////////////////////////////////////////////
    }
}