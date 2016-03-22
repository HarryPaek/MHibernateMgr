using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using System.Collections.Generic;
using TestConsoleApp.Abstract;
using TestConsoleApp.Services;

namespace TestConsoleApp.TestManager
{
    public class ServiceTestManager : IEPCManagerTester
    {
        public void RunTest()
        {
            var TesterList = new List<IEPCManagerTester>();

            TesterList.Add(new RevisionProviderTester());

            foreach (IEPCManagerTester tester in TesterList)
            {
                tester.RunTest();
            }
        }
    }
}
