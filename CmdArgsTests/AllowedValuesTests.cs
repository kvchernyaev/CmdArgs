#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
using NUnit.Framework.Internal;
#endregion



namespace CmdArgsTests
{
    [TestFixture]
    public class AllowedValuesTests
    {
        class Conf
        {
            [ValuedArgument('a')] public Constraint A;
            [ValuedArgument('b')] public Constraint B;


            [ValuedArgument('c', DefaultValue = Constraint.Three)]
            public Constraint C;


            [ValuedArgument('d', DefaultValue = Constraint.Three)]
            public Constraint? D;
        }



        enum Constraint : byte
        {
            Nol,
            One,
            Two,
            Three
        }



        [Test]
        public void TestEnum()
        {
            var p = new CmdArgsParser();
            Res<Conf> res = p.ParseCommandLine<Conf>(new[] {"-a", "2", "-b", "two", "-c"});

            Assert.AreEqual(Constraint.Two, res.Args.A);
            Assert.AreEqual(Constraint.Two, res.Args.B);
            Assert.AreEqual(Constraint.Three, res.Args.C);
            Assert.AreEqual(null, res.Args.D);
        }
    }
}