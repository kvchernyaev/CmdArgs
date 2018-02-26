#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
#endregion



namespace CmdArgsTests
{
    [TestFixture]
    public class CustomTypesTests
    {
        ////////////////////////////////////////////////////////////////



        class P
        {
            public int A;
            public int B;


            #region generated
            protected bool Equals(P other) => A == other.A && B == other.B;


            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == this.GetType() && Equals((P) obj);
            }


            public override int GetHashCode()
            {
                unchecked
                {
                    return (A * 397) ^ B;
                }
            }
            #endregion


            public static P Deserialize(string value)
            {
                string[] ss = value.Split(':');

                if (ss.Length != 2) throw new FormatException();
                var rv = new P();
                rv.A = int.Parse(ss[0]);
                rv.B = int.Parse(ss[1]);
                return rv;
            }
        }



        class ConfOk
        {
            [CustomArgument('p')] public P P;
        }



        [Test]
        public void TestOk()
        {
            var p = new CmdArgsParser();
            Res<ConfOk> res = p.ParseCommandLine<ConfOk>(new[] {"-p", "23:64"});
            Assert.AreEqual(23, res.Args.P.A);
            Assert.AreEqual(64, res.Args.P.B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfArray
        {
            [CustomArgument('a')] public P[] A;
            [CustomArgument('l')] public List<P> L;
        }



        [Test]
        public void TestArray()
        {
            var p = new CmdArgsParser();
            Res<ConfArray> res = p.ParseCommandLine<ConfArray>(new[]
                    {"-a", "23:64", "4:56", "-l", "59:84", "58:81"});

            Assert.AreEqual(2, res.Args.A.Length);
            Assert.AreEqual(2, res.Args.L.Count);

            Assert.AreEqual(23, res.Args.A[0].A);
            Assert.AreEqual(64, res.Args.A[0].B);

            Assert.AreEqual(4, res.Args.A[1].A);
            Assert.AreEqual(56, res.Args.A[1].B);

            Assert.AreEqual(59, res.Args.L[0].A);
            Assert.AreEqual(84, res.Args.L[0].B);

            Assert.AreEqual(58, res.Args.L[1].A);
            Assert.AreEqual(81, res.Args.L[1].B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefault
        {
            [CustomArgument('p', DefaultValue = "898:01")]
            public P P;
        }



        [Test]
        public void TestDefault()
        {
            var p = new CmdArgsParser();
            Res<ConfDefault> res = p.ParseCommandLine<ConfDefault>(new[] {"-p"});
            Assert.AreEqual(898, res.Args.P.A);
            Assert.AreEqual(1, res.Args.P.B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefNotAllowed
        {
            [CustomArgument('p', AllowedValues = new object[] {"1:4", "1:3", "34:83"},
                DefaultValue = "898:01")]
            public P P;
        }



        [Test]
        public void TestDefNotAllowed()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfDefNotAllowed>(new[] {"-p", "1:3"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfAllowed
        {
            [CustomArgument('p', AllowedValues = new object[] {"1:4", "1:3", "34:83"})]
            public P P;
        }



        [Test]
        public void TestAllowedOk()
        {
            var p = new CmdArgsParser();
            Res<ConfAllowed> res = p.ParseCommandLine<ConfAllowed>(new[] {"-p", "1:3"});
            Assert.AreEqual(1, res.Args.P.A);
            Assert.AreEqual(3, res.Args.P.B);
        }


        [Test]
        public void TestAllowedNo()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<ConfAllowed>(new[] {"-p", "1:5"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefarrayOne
        {
            [CustomArgument('p', AllowedValues = new object[] {"1:4", "1:3", "34:83"},
                DefaultValue = "1:3")]
            public P[] P;
        }



        [Test]
        public void TestDefarrayOne()
        {
            var p = new CmdArgsParser();
            Res<ConfDefarrayOne> res = p.ParseCommandLine<ConfDefarrayOne>(new[] {"-p"});
            Assert.AreEqual(1, res.Args.P.Length);
            Assert.AreEqual(1, res.Args.P[0].A);
            Assert.AreEqual(3, res.Args.P[0].B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefarray
        {
            [CustomArgument('p', AllowedValues = new object[] {"1:4", "1:3", "34:83"},
                DefaultValue = new[] {"1:3", "1:4"})]
            public P[] P;
        }



        [Test]
        public void TestDefarray()
        {
            var p = new CmdArgsParser();
            Res<ConfDefarray> res = p.ParseCommandLine<ConfDefarray>(new[] {"-p"});
            Assert.AreEqual(2, res.Args.P.Length);
            Assert.AreEqual(1, res.Args.P[0].A);
            Assert.AreEqual(3, res.Args.P[0].B);
            Assert.AreEqual(1, res.Args.P[1].A);
            Assert.AreEqual(4, res.Args.P[1].B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefarrayPredicateCol
        {
            [CustomArgument('p', DefaultValue = new[] {"1:3", "1:4"})]
            public P[] P;


            public static Predicate<P[]> P_Predicate = x => x.Length > 5;
        }



        [Test]
        public void TestDefarrayPredicateCol()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfDefarrayPredicateCol>(new[] {"-p"}));
        }


        ////////////////////////////////////////////////////////////////

        // DefaultValue array, predicatecol



        class ConfDefarrayPredicate
        {
            [CustomArgument('p', DefaultValue = new[] {"1:3", "1:4"})]
            public P[] P;


            public static Predicate<P> P_Predicate = x => x.B > 3;
        }



        [Test]
        public void TestDefarrayPredicate()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfDefarrayPredicate>(new[] {"-p"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfDefaultByPredicate
        {
            [CustomArgument('p', DefaultValue = "9:83")]
            public P P;


            public static Predicate<P> P_Predicate_0 = p => p.A > 0;
            public static Predicate<P> P_Predicate = p => p.A > 10;
            public static Predicate<P> P_Predicate_1 = p => p.A > -1;
        }



        [Test]
        public void TestDefaultByPredicate()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfDefaultByPredicate>(new[] {"-p", "1:3"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfUseDef
        {
            [CustomArgument('p', DefaultValue = "9:83", UseDefWhenNoArg = true)]
            public P P;
        }



        [Test]
        public void TestUseDef()
        {
            var p = new CmdArgsParser();
            Res<ConfUseDef> r = p.ParseCommandLine<ConfUseDef>(new string[] { });
            Assert.AreEqual(9, r.Args.P.A);
            Assert.AreEqual(83, r.Args.P.B);
        }


        [Test]
        public void TestUseDefNoVal()
        {
            var p = new CmdArgsParser();
            Res<ConfUseDef> r = p.ParseCommandLine<ConfUseDef>(new[] {"-p"});
            Assert.AreEqual(9, r.Args.P.A);
            Assert.AreEqual(83, r.Args.P.B);
        }


        [Test]
        public void TestUseDefVal()
        {
            var p = new CmdArgsParser();
            Res<ConfUseDef> r = p.ParseCommandLine<ConfUseDef>(new[] {"-p", "1:2"});
            Assert.AreEqual(1, r.Args.P.A);
            Assert.AreEqual(2, r.Args.P.B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfAllowedByPredicate
        {
            [CustomArgument('p', AllowedValues = new object[] {"1:4", "1:3", "34:83"})]
            public P P;


            public static Predicate<P> P_Predicate_0 = p => p.A > 0;
            public static Predicate<P> P_Predicate = p => p.A > 10;
            public static Predicate<P> P_Predicate_1 = p => p.A > -1;
        }



        [Test]
        public void TestAllowedByPredicate()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfAllowedByPredicate>(new[] {"-p", "1:3"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfCantDeser
        {
            [CustomArgument('p')] public P P;
        }



        [Test]
        public void TestCantDeser()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfCantDeser>(new[] {"-p", "13"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfNoDesr
        {
            [CustomArgument('p')] public NoDeser P;



            public class NoDeser
            {
                public int A;
                public int B;
            }
        }



        [Test]
        public void TestNoDesr()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(
                () => p.ParseCommandLine<ConfNoDesr>(new[] {"-p", "23:64"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfBadSign
        {
            [CustomArgument('p')] public BadSign P;



            public class BadSign
            {
                public int A;
                public int B;


                public static BadSign Deserialize(string[] value)
                {
                    string[] ss = value[0].Split(':');

                    var rv = new BadSign();
                    rv.A = int.Parse(ss[0]);
                    rv.B = int.Parse(ss[1]);
                    return rv;
                }
            }
        }



        [Test]
        public void TestBadSign()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfBadSign>(new[] {"-p", "23:64"}));
        }


        ////////////////////////////////////////////////////////////////
    }
}