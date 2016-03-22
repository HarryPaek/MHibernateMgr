using System.Collections.Generic;
using TestConsoleApp.Abstract;
using TestConsoleApp.Document;

namespace TestConsoleApp.TestManager
{
    public class DocumentTestManager : IEPCManagerTester
    {
        public void RunTest()
        {
            var TesterList = new List<IEPCManagerTester>();

            //TesterList.Add(new DocumentCRUDTester());
            //TesterList.Add(new DocumentCheckOutCheckInTester());
            //TesterList.Add(new DocumentLifeCycelTester());
            //TesterList.Add(new DocumentCopyTester());
            //TesterList.Add(new SPDocumentListTester());
            TesterList.Add(new DocumentRelationshipTester());

            foreach (IEPCManagerTester tester in TesterList)
            {
                tester.RunTest();
            }
        }
    }
}
