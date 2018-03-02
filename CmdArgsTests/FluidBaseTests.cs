#region usings
using System;
using System.Collections.Generic;
using System.IO;
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
    public class FluidBaseTests
    {
        class FluidBaseConf
        {
            /*
            --defaultsFile="core/liquibase_CIS-DB.CIS_custom.properties" ^
            --labels="bus_data OR bus_longtime_data" ^
            --contexts="Light,prod, sync_prod,sync" ^
                update ^
             */
            [ValuedArgument("labels")] public string Labels;
            [ValuedArgument("contexts")] public string[] Contexts;


            [FileArgument("defaultsFile", MustExists = false)]
            public FileInfo DefaultsFile;
        }



        [Test]
        public void Test()
        {
            var p = new CmdArgsParser();
            Res<FluidBaseConf> r = p.ParseCommandLine<FluidBaseConf>(new[]
                {
                    "--labels=bus_data OR bus_longtime_data",
                    "--defaultsFile=core/liquibase_CIS-DB.CIS_custom.properties",
                    "--contexts=Light,prod, sync_prod,sync",
                    "update"
                });
        }
    }
}