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
            [ValuedArgument("labels")] public string Labels;
            [ValuedArgument("contexts")] public string[] Contexts;


            [FileArgument("defaultsFile", MustExists = false)]
            public FileInfo DefaultsFile;


            [FileArgument("changeLogFile", MustExists = false)]
            public FileInfo ChangeLogFile;


            [SwitchArgument("verbose")] public bool Verbose;

            [ValuedArgument("logLevel")] public LogLevel LogLevel;


            [UnstrictlyConfArgument('D', "DEFINE")]
            public UnstrictlyConf Define;


            [ValuedArgument('c', "ConnectionString")]
            public string ConnectionString;


            [ValuedArgument('u', "username")] public string SqlUser;
            [ValuedArgument('p', "password")] public string SqlUserPassword;
        }



        public enum LogLevel
        {
            Error,
            Warn,
            Info,
            Verbose
        }



        [Test]
        public void TestFull()
        {
            var p = new CmdArgsParser();
            Res<FluidBaseConf> r = p.ParseCommandLine<FluidBaseConf>(new[]
                {
                    "--labels=bus_data OR bus_longtime_data",
                    "--defaultsFile=core/liquibase_CIS-DB.CIS_custom.properties",
                    "--contexts=Light,prod, sync_prod,sync",
                    "--logLevel=info",
                    "--ConnectionString=jdbc:sqlserver://test02;databaseName=CityExpressDB_ceapp_developers;integratedSecurity=true;applicationName=liquibase;",
                    "--username=liquibaseuser",
                    "--password=gfhjkm",
                    "--changeLogFile=CityExpressDB/#db.changelog-master_OnlyScheme.xml",

                    "-DSOURCE_SERVER.DB=TEST02.CityExpressDB_ceapp_developers",
                    "-DSOURCE_SERVER.APP=192.168.54.26",

                    "update"
                });

            CheckFull(r);
        }


        static void CheckFull(Res<FluidBaseConf> r)
        {
            Check(r);
            Assert.AreEqual(false, r.Args.Verbose);
            Assert.AreEqual(
                "jdbc:sqlserver://test02;databaseName=CityExpressDB_ceapp_developers;integratedSecurity=true;applicationName=liquibase;",
                r.Args.ConnectionString);
            Assert.AreEqual("liquibaseuser", r.Args.SqlUser);
            Assert.AreEqual("gfhjkm", r.Args.SqlUserPassword);
            Assert.AreEqual(LogLevel.Info, r.Args.LogLevel);
            Assert.AreEqual(
                Path.Combine(LocalDir, "CityExpressDB\\#db.changelog-master_OnlyScheme.xml"),
                r.Args.ChangeLogFile.FullName);

            Assert.AreEqual(2, r.Args.Define.Count);
            Assert.AreEqual("SOURCE_SERVER.DB", r.Args.Define[0].Name);
            Assert.AreEqual("TEST02.CityExpressDB_ceapp_developers", r.Args.Define[0].Value);
            Assert.AreEqual("SOURCE_SERVER.APP", r.Args.Define[1].Name);
            Assert.AreEqual("192.168.54.26", r.Args.Define[1].Value);
        }


        static readonly string LocalDir = new FileInfo("./a").DirectoryName;


        static void Check(Res<FluidBaseConf> r)
        {
            Assert.AreEqual("bus_data OR bus_longtime_data", r.Args.Labels);
            Assert.AreEqual(Path.Combine(LocalDir, "core\\liquibase_CIS-DB.CIS_custom.properties"),
                r.Args.DefaultsFile.FullName);
            Assert.IsTrue(
                new[] {"Light", "prod", "sync_prod", "sync"}.SequenceEqual(r.Args.Contexts));

            Assert.IsTrue(new[] {"update"}.SequenceEqual(r.AdditionalArguments));
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

            Check(r);
        }


        [Test]
        public void Test1()
        {
            var p = new CmdArgsParser();
            Res<FluidBaseConf> r = p.ParseCommandLine<FluidBaseConf>(new[]
                {
                    "--labels", "bus_data OR bus_longtime_data",
                    "--defaultsFile=core/liquibase_CIS-DB.CIS_custom.properties",
                    "--contexts=Light,prod, sync_prod,sync",
                    "update"
                });
            Check(r);
        }


        [Test]
        public void Test2()
        {
            var p = new CmdArgsParser();
            Res<FluidBaseConf> r = p.ParseCommandLine<FluidBaseConf>(new[]
                {
                    "--labels", "bus_data OR bus_longtime_data",
                    "--contexts", "Light", "prod", "sync_prod", "sync",
                    "--defaultsFile=core/liquibase_CIS-DB.CIS_custom.properties",
                    "update"
                });
            Check(r);
        }


        [Test]
        public void Test3()
        {
            var p = new CmdArgsParser();
            Res<FluidBaseConf> r = p.ParseCommandLine<FluidBaseConf>(new[]
                {
                    "update",
                    "--labels", "bus_data OR bus_longtime_data",
                    "--contexts", "Light", "prod", "sync_prod", "sync",
                    "--defaultsFile", "core/liquibase_CIS-DB.CIS_custom.properties",
                });
            Check(r);
        }


        [Test]
        public void Test4()
        {
            var p = new CmdArgsParser();
            Res<FluidBaseConf> r = p.ParseCommandLine<FluidBaseConf>(new[]
                {
                    "--labels", "bus_data OR bus_longtime_data",
                    "--contexts", "Light", "prod", "sync_prod", "sync",
                    "--defaultsFile", "core/liquibase_CIS-DB.CIS_custom.properties",
                    "update"
                });
            Check(r);
        }
    }
}