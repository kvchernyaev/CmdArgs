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
    public class ValueNullableTests
    {
        [Test]
        public void TestString()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s", "34", "-a"});

            Assert.AreEqual(rv.Args.S, 34);
            Assert.AreEqual(rv.Args.D, null);
            Assert.AreEqual(rv.Args.A, -67);
        }



        class Conf
        {
            [ValuedArgument(typeof(int), 's')]
            public int? S { get; set; }


            [ValuedArgument(typeof(int), 'd')]
            public int? D { get; set; }


            [ValuedArgument(typeof(int), 'a', DefaultValue = -67)]
            public int? A { get; set; }
        }
    }
}