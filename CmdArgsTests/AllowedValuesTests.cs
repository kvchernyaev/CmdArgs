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


        ////////////////////////////////////////////////////////////////



        class ConfAllowed
        {
            [ValuedArgument('a', AllowedValues = new object[] {3, 5, 6})]
            public int A;


            [ValuedArgument('b', AllowedValues = new object[] {7, 8, 9}, DefaultValue = 8)]
            public int B;


            [ValuedArgument('c', AllowedValues = new object[] {10, 11, 12}, DefaultValue = 11)]
            public int? C;
        }



        [Test]
        public void TestAllowedOk()
        {
            var p = new CmdArgsParser();
            Res<ConfAllowed> res = p.ParseCommandLine<ConfAllowed>(new[] {"-a", "5", "-b"});
            Assert.AreEqual(5, res.Args.A);
            Assert.AreEqual(8, res.Args.B);
            Assert.AreEqual(null, res.Args.C);
        }


        [Test]
        public void TestAllowedOut()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<ConfAllowed>(new[] {"-a", "4"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfEnumAllowed
        {
            [ValuedArgument('a', AllowedValues = new object[] {Constraint.One, Constraint.Three})]
            public Constraint A;
        }



        [Test]
        public void TestEnumAllowedOk()
        {
            var p = new CmdArgsParser();
            Res<ConfEnumAllowed> res = p.ParseCommandLine<ConfEnumAllowed>(new[] {"-a", "3"});
            Assert.AreEqual(Constraint.Three, res.Args.A);
        }


        [Test]
        public void TestEnumAllowedOut()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(
                () => p.ParseCommandLine<ConfEnumAllowed>(new[] {"-a", "2"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfAllowedBadAllowedType
        {
            [ValuedArgument('b', AllowedValues = new object[] {"6", "2"}, DefaultValue = 5)]
            public int B;
        }



        [Test]
        public void TestAllowedBadAllowedType()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfAllowedBadAllowedType>(new[] {"-b"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfAllowedBadDefault
        {
            [ValuedArgument('b', AllowedValues = new object[] {7, 8, 9}, DefaultValue = 5)]
            public int B;
        }



        [Test]
        public void TestAllowedBadDefault()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfAllowedBadDefault>(new[] {"-b"}));
        }
    }
}