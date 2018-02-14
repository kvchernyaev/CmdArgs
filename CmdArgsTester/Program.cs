#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdArgs;
using NUnit.Framework;
#endregion



namespace CmdArgsTester
{
    public class Program
    {
        static void Main(string[] a)
        {
            /*
             * -s
             * -ss FAIL
             * -sd
             * --some
             * -s --some FAIL
             * -s -s FAIL
             * -s -d
             * -s --dummy
             * --some --dummy
             */
            /*
             * -a val
             * -a=val
             * -s --multy val1 val2 val3 -d
             * -s --multy=val1,val2,val3 -d
             */
            /*
             * -Dname=val
             */
            /*
             * /some
             */
            CmdArgsParser parser;

            parser = new CmdArgsParser
                    {AllowAdditionalArguments = true, AllowUnknownArgument = true};
            var argsOk = new[]
                {
                    new[] {"-s"},
                    new[] {"-sd"},
                    new[] {"-ds"},
                    new[] {"-s", "--dummy"},
                    new[] {"-s", "-d"},
                    new[] {"-s", "-u", "-d"},
                    new[] {"-s", "--unknown", "-d"},
                    new[] {"-s", "--unknown", "val", "val2", "-d"},
                    new[] {"-s", "--unknown", "val", "val2"},
                    new[] {"-s", "-d", "command"},
                    new[] {"-s", "-d", "command", "andCommand"},
                };

            foreach (string[] args in argsOk)
            {
                Res<Conf> res = parser.ParseCommandLine<Conf>(args);
            }

            //parser = new CmdArgsParser
            //        {ValuesOnlyByEqual = true};
            //argsOk = new[]
            //        {new[] {"-s", "-d", "command", "andCommand"}// use AdditionalArguments
            //};
            //foreach (string[] args in argsOk)
            //{
            //    conf = new Conf();
            //    parser.ParseCommandLine(args, conf);
            //}

            var argsErr = new[]
                {
                    new[] {"-ss"},
                    new[] {"-s", "-s"},
                    new[] {"-s", "value", "-d"},
                    new[] {"-s", "--some"},
                };
            foreach (string[] args in argsErr)
            {
                Assert.Throws<CmdArgsException>(() => parser.ParseCommandLine<Conf>(args));
            }
        }
    }



    public class Conf
    {
        [SwitchArgument('s', "some", "some description")]
        public bool Some { get; set; }


        [SwitchArgument('d', "dummy", "dummy description")]
        public bool Dummy { get; set; }
    }
}