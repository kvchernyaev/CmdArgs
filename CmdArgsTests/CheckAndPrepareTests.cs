using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;

namespace CmdArgsTests
{
    [TestFixture]
    public class CheckAndPrepareTests
    {
        class Conf : ICheckAndPrepare<Conf>
        {
            public string Dummy { get; set; }

            public void CheckAndPrepare(Res<Conf> parsed)
            {
                if (parsed.AdditionalArguments.Count == 0)
                    throw new CmdException("Provide command name!");
                if (parsed.AdditionalArguments.Count > 1)
                    throw new CmdException("Unsupported arguments: " + parsed.AdditionalArguments.Skip(1).Select(x => $"[{x}]"));

                Dummy = parsed.AdditionalArguments[0];
            }

        }



        [Test]
        public void TestCheckingFailed()
        {
            var p = new CmdArgsParser<Conf> { AllowAdditionalArguments = true };
            // Provide command name!
            Assert.Throws<CmdException>(() => p.ParseCommandLine(new string[] { }));
        }


        [Test]
        public void TestCheckingSuccess()
        {
            var p = new CmdArgsParser<Conf> { AllowAdditionalArguments = true };
            // Provide command name!
            var r = p.ParseCommandLine(new string[] { "mycommand" });

            Assert.AreEqual(r.Args.Dummy, "mycommand");
        }
    }
}
