using Build.TestReporting.Xml2CSharp;
using Cake.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Build.TestReporting
{
    public static class TrxReader
    {
        public static TestRun GetTestRun(BuildContext context)
        {
            try
            {
                var fileName = context.File("TestResults/testResult.trx");
                var absolutePath = fileName.Path.MakeAbsolute(context.Environment);

                using (Stream reader = new FileStream(absolutePath.FullPath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(TestRun));
                    TestRun result = (TestRun)serializer.Deserialize(reader);

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new TestRun();
            }
        }
    }
}
