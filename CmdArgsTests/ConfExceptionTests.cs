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
    public class ConfExceptionTests
    {
        [Test]
        public void TestConfWrongSwitch()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() => p.ParseCommandLine<ConfWrongSwitch>(new[] {"-s"}));
        }



        class ConfWrongSwitch
        {
            [SwitchArgument('s')] public int Some; // must be bool
        }



        [Test]
        public void TestConfWrongLong()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() => p.ParseCommandLine<ConfManyLong>(new[] {"-s"}));
        }



        class ConfManyLong
        {
            [SwitchArgument('s', "some", "some description")]
            public bool Some { get; set; }


            [SwitchArgument('d', "some", "dummy description")]
            public bool Dummy { get; set; }
        }



        [Test]
        public void TestConfWrongShort()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() => p.ParseCommandLine<ConfManyShort>(new[] {"-s"}));
        }



        class ConfManyShort
        {
            [SwitchArgument('s')]
            public bool Some { get; set; }


            [SwitchArgument('s')]
            public bool Dummy { get; set; }
        }



        [Test]
        public void TestNotLetter()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() => p.ParseCommandLine<ConfNotLetter>(new[] {"-s"}));
        }



        class ConfNotLetter
        {
            [SwitchArgument('1')]
            public bool Some { get; set; }
        }
    }
}