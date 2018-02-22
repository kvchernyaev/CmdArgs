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


            public static P Deserialize(string[] values)
            {
                string[] ss = values[0].Split(':');

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

        // signature of Deserialize method
        // array, list
        // DefaultValue
        // AllowedValues
        // UseDef
    }
}