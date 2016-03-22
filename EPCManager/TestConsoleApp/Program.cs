using System;
using System.Collections.Generic;
using TestConsoleApp.Abstract;
using TestConsoleApp.TestManager;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var TestManagers = new List<IEPCManagerTester>();

            TestManagers.Add(new CommonTestManager());
            //TestManagers.Add(new ServiceTestManager());
            TestManagers.Add(new DocumentTestManager());
            TestManagers.Add(new ItemTestManager());

            foreach (IEPCManagerTester testManager in TestManagers)
            {
                testManager.RunTest();
            }

            Console.ReadLine();
        }
    }
}
