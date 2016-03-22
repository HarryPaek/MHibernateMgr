using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using System.Collections.Generic;
using TestConsoleApp.Abstract;
using TestConsoleApp.Common;

namespace TestConsoleApp.TestManager
{
    public class CommonTestManager : IEPCManagerTester
    {
        public void RunTest()
        {
            var TesterList = new List<IEPCManagerTester>();

            //TesterList.Add(new ClassTester());
            //TesterList.Add(new StatusTester());
            //TesterList.Add(new PhaseTester());
            //TesterList.Add(new DomainTester());
            //TesterList.Add(new IdentifierTester());
            //TesterList.Add(new PeopleTester());
            //TesterList.Add(new RelationshipTypeTester());
            TesterList.Add(new RelationshipTester());
            // TesterList.Add(new TryRelationshipTester());

            foreach (IEPCManagerTester tester in TesterList)
            {
                tester.RunTest();
            }
        }
    }
}
