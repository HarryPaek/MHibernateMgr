using System.Collections.Generic;
using TestConsoleApp.Abstract;
using TestConsoleApp.Item;

namespace TestConsoleApp.TestManager
{
    public class ItemTestManager : IEPCManagerTester
    {
        public void RunTest()
        {
            var TesterList = new List<IEPCManagerTester>();

            //TesterList.Add(new ItemCRUDTester());
            //TesterList.Add(new ItemCheckOutCheckInTester());
            //TesterList.Add(new ItemLifeCycelTester());
            //TesterList.Add(new ItemCopyTester());
            //TesterList.Add(new SPItemListTester());
            TesterList.Add(new ItemRelationshipTester());

            foreach (IEPCManagerTester tester in TesterList)
            {
                tester.RunTest();
            }
        }
    }
}
