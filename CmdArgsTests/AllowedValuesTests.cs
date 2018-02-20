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
            [ValuedArgument('b', AllowedValues = new object[] {"6", "2"}, DefaultValue = 2)]
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



        class ConfBadDefaultByAllowed
        {
            [ValuedArgument('b', AllowedValues = new object[] {7, 8, 9}, DefaultValue = 5)]
            public int B;
        }



        [Test]
        public void TestBadDefaultByAllowed()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfBadDefaultByAllowed>(new[] {"-b"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfAllowedBadDefaultByRegex
        {
            [ValuedArgument('b', DefaultValue = "qwer", RegularExpression = @"\d")]
            public string B;
        }



        [Test]
        public void TestAllowedBadDefaultByRegex()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfAllowedBadDefaultByRegex>(new[] {"-b"}));
        }
        ////////////////////////////////////////////////////////////////



        class ConfAllowedBadAllowedByRegex
        {
            [ValuedArgument('b', AllowedValues = new object[] {"a2", "asdf", "b4"},
                RegularExpression = @"\d")]
            public string B;
        }



        [Test]
        public void TestAllowedBadAllowedByRegex()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfAllowedBadAllowedByRegex>(new[] {"-b"}));
        }



        ////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////
        class ConfPredicate
        {
            /// <summary>
            /// must be 1..20 and divides by 3
            /// </summary>
            [ValuedArgument('b', "IntPredicate")] public int B;


            public static Predicate<int> B_Predicate_closure;
            public static Predicate<int> B_Predicate_set;
            public static Predicate<int> B_Predicate_const = i => i > 0; // const predicate
        }



        [Test]
        public void TestPredicateBad()
        {
            var p = new CmdArgsParser();
            var testi = 20;
            Predicate<int> pred = i => i <= testi; // using closure
            ConfPredicate.B_Predicate_closure = pred; // using delegate variable
            ConfPredicate.B_Predicate_set = i => i % 3 == 0;

            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfPredicate>(new[] {"--IntPredicate", "25"}));
        }


        [Test]
        public void TestPredicateOk()
        {
            var p = new CmdArgsParser();
            var testi = 20;
            Predicate<int> pred = i => i <= testi; // using closure
            ConfPredicate.B_Predicate_closure = pred; // using delegate variable
            ConfPredicate.B_Predicate_set = i => i % 3 == 0;

            // must be 1..20 and divides by 3
            Res<ConfPredicate> res = p.ParseCommandLine<ConfPredicate>(new[] {"-b", "9"});
            Assert.AreEqual(9, res.Args.B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfPredicateArOne
        {
            [ValuedArgument('b')] public int[] B;


            public static Predicate<int> B_Predicate_one = i => i > 2; // const predicate
        }



        [Test]
        public void TestPredicateArOneOk()
        {
            var p = new CmdArgsParser();
            Res<ConfPredicateArOne> res =
                p.ParseCommandLine<ConfPredicateArOne>(new[] {"-b", "25", "123"});
            Assert.IsTrue(new[] {25, 123}.SequenceEqual(res.Args.B));
        }


        [Test]
        public void TestPredicateArOneBad()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfPredicateArOne>(new[] {"-b", "25", "1", "123"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfPredicateArAr
        {
            [ValuedArgument('b')] public int[] B;


            public static Predicate<int> B_Predicate_one = i => i > 2; // const predicate
            public static Predicate<int[]> B_Predicate_ar = i => i.Length > 2; // const predicate
        }



        [Test]
        public void TestPredicateArArOk()
        {
            var p = new CmdArgsParser();
            Res<ConfPredicateArAr> res =
                p.ParseCommandLine<ConfPredicateArAr>(new[] {"-b", "25", "123", "12"});
            Assert.IsTrue(new[] {25, 123, 12}.SequenceEqual(res.Args.B));
        }


        [Test]
        public void TestPredicateArArBadOne()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfPredicateArAr>(new[] {"-b", "25", "1", "12"}));
        }


        [Test]
        public void TestPredicateArArBadAr()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfPredicateArAr>(new[] {"-b", "25", "12"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfPredicateBadType
        {
            [ValuedArgument('b')] public int B;


            public static Predicate<string>
                B_Predicate_third = i => i.Length > 10; // const predicate
        }



        [Test]
        public void TestPredicateBadType()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfPredicateBadType>(new[] {"-b", "25"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfPredicateBadTypeAr
        {
            [ValuedArgument('b')] public int[] B;


            public static Predicate<string>
                B_Predicate_third = i => i.Length > 10; // const predicate
        }



        [Test]
        public void TestPredicateBadTypeAr()
        {
            var p = new CmdArgsParser();
            Assert.Throws<ConfException>(() =>
                p.ParseCommandLine<ConfPredicateBadTypeAr>(new[] {"-b", "25"}));
        }


        ////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////



        class ConfRegex
        {
            [ValuedArgument('b', RegularExpression = @"^[a-d]B\d$")]
            public string B;
        }



        [Test]
        public void TestRegexOk()
        {
            var p = new CmdArgsParser();
            Res<ConfRegex> res = p.ParseCommandLine<ConfRegex>(new[] {"-b", "bB1"});
            Assert.AreEqual("bB1", res.Args.B);
        }


        [Test]
        public void TestRegexBad()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() => p.ParseCommandLine<ConfRegex>(new[] {"-b", "eB1"}));
        }


        ////////////////////////////////////////////////////////////////



        class ConfRegexInt
        {
            [ValuedArgument('b', RegularExpression = @"^[123]0[89]$")]
            public int B;
        }



        [Test]
        public void TestRegexIntOk()
        {
            var p = new CmdArgsParser();
            Res<ConfRegexInt> res = p.ParseCommandLine<ConfRegexInt>(new[] {"-b", "208"});
            Assert.AreEqual(208, res.Args.B);
        }


        [Test]
        public void TestRegexIntBad()
        {
            var p = new CmdArgsParser();
            Assert.Throws<CmdException>(() =>
                p.ParseCommandLine<ConfRegexInt>(new[] {"-b", "218"}));
        }
    }
}