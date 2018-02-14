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
        class Conf
        {
            [ValuedArgument('s')]
            public int? S { get; set; }


            [ValuedArgument('d')]
            public int? D { get; set; }


            [ValuedArgument('a', DefaultValue = -67)]
            public int? A { get; set; }
        }



        [Test]
        public void TestNullableOk()
        {
            var p = new CmdArgsParser();
            Res<Conf> rv = p.ParseCommandLine<Conf>(new[] {"-s", "34", "-a"});

            Assert.AreEqual(34, rv.Args.S);
            Assert.AreEqual(null, rv.Args.D);
            Assert.AreEqual(-67, rv.Args.A);
        }
    }
}