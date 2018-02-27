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
    public class EqualValueTests
    {
        ////////////////////////////////////////////////////////////////



        class ConfEmpty
        {
            [ValuedArgument('a', DefaultValue = "def")]
            public string A;
        }



        [Test]
        public void TestEmpty()
        {
            var p = new CmdArgsParser();
            p.UseEqualitySyntax = true;
            p.AllowAdditionalArguments = true;
            Res<ConfEmpty> r = p.ParseCommandLine<ConfEmpty>(new[] {"-a=", "noval"});
            Assert.AreEqual("def", r.Args.A);
        }


        ////////////////////////////////////////////////////////////////



        class ConfOne
        {
            [ValuedArgument('a')] public string A;
            [ValuedArgument('b', "Sec")] public string B;
        }



        [Test]
        public void TestOne()
        {
            var p = new CmdArgsParser();
            p.UseEqualitySyntax = true;
            Res<ConfOne> r = p.ParseCommandLine<ConfOne>(new[] {"--Sec=qwer", "-a=uiop"});
            Assert.AreEqual("uiop", r.Args.A);
            Assert.AreEqual("qwer", r.Args.B);
        }


        ////////////////////////////////////////////////////////////////



        class ConfArray
        {
            [ValuedArgument('a')] public string[] A;
            [ValuedArgument('b', "Sec")] public string[] B;
            [ValuedArgument('c', "White")] public string[] C;
        }



        [Test]
        public void TestArray()
        {
            var p = new CmdArgsParser();
            p.UseEqualitySyntax = true;
            Res<ConfArray> r = p.ParseCommandLine<ConfArray>(new[]
                    {"-a=uiop,asdf", "--Sec=qwer;zxcv", "-c=\"gh df\""});
            Assert.AreEqual(2, r.Args.A.Length);
            Assert.AreEqual("uiop", r.Args.A[0]);
            Assert.AreEqual("asdf", r.Args.A[1]);

            Assert.AreEqual(2, r.Args.B.Length);
            Assert.AreEqual("qwer", r.Args.B[0]);
            Assert.AreEqual("zxcv", r.Args.B[1]);

            Assert.AreEqual(2, r.Args.C.Length);
            Assert.AreEqual("\"gh", r.Args.C[0]);
            Assert.AreEqual("df\"", r.Args.C[1]);
        }


        ////////////////////////////////////////////////////////////////



        class ConfAdd
        {
            [ValuedArgument('a')] public string A;
            [ValuedArgument('b')] public string B;
        }



        [Test]
        public void TestAdd()
        {
            var p = new CmdArgsParser();
            p.UseEqualitySyntax = true;
            p.AllowAdditionalArguments = true;
            Res<ConfAdd> r = p.ParseCommandLine<ConfAdd>(new[] 
                    {"-a=asdf", "noval", "-b=qewr", "jkli", "-34"});
            Assert.AreEqual("asdf", r.Args.A);
            Assert.AreEqual("qewr", r.Args.B);

            Assert.IsTrue(new[] {"noval", "jkli", "-34"}.SequenceEqual(r.AdditionalArguments));
        }


        ////////////////////////////////////////////////////////////////
    }
}