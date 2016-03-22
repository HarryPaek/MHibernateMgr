using System;

namespace EPCManager.Domain.Exceptions
{
    public class EPCManagerLifeCycleException : EPCManagerException
    {
        public EPCManagerLifeCycleException(string message) : base(message)
        {
        }

        public EPCManagerLifeCycleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
