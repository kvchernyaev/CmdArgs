#region usings
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class SimpleTypesTests
    {
        ////////////////////////////////////////////////////////////////
        class ConfGuid
        {
            [ValuedArgument('g')] public Guid G;
        }



        [Test]
        public void TestGuid()
        {
            var p = new CmdArgsParser<ConfGuid>();
            Guid g = Guid.NewGuid();
            Res<ConfGuid> r = p.ParseCommandLine(new[] {"-g", g.ToString()});

            Assert.AreEqual(g, r.Args.G);
        }



        ////////////////////////////////////////////////////////////////
        class ConfDateTime
        {
            [ValuedArgument('g')] public DateTime G;
        }



        [Test]
        public void TestDateTime()
        {
            var p = new CmdArgsParser<ConfDateTime>();
            var s = "2018.02.26T13:31:04.0003";
            DateTime g = DateTime.Parse(s, CultureInfo.InvariantCulture);
            Res<ConfDateTime> r = p.ParseCommandLine(new[] {"-g", s});

            Assert.AreEqual(g, r.Args.G);
        }



        ////////////////////////////////////////////////////////////////
        class ConfTimeSpan
        {
            [ValuedArgument('g')] public TimeSpan G;
        }



        [Test]
        public void TestTimeSpan()
        {
            var p = new CmdArgsParser<ConfTimeSpan>();
            var s = "05:02:06";
            TimeSpan g = TimeSpan.Parse(s, CultureInfo.InvariantCulture);
            Res<ConfTimeSpan> r = p.ParseCommandLine(new[] {"-g", s});

            Assert.AreEqual(g, r.Args.G);
        }


        ////////////////////////////////////////////////////////////////
    }
}