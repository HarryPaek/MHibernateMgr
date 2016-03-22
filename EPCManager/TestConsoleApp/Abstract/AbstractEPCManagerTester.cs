using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Abstract
{
    internal abstract class AbstractEPCManagerTester : IEPCManagerTester
    {
        protected virtual void Before()
        {

        }

        protected virtual void After()
        {

        }

        public void Run(Action actionToRun)
        {
            Before();
            actionToRun();
            After();
        }

        public abstract void RunTest();
    }
}
