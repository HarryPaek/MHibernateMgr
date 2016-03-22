using System;

namespace EPCManager.Domain.Exceptions
{
    public class EPCManagerCopyException : EPCManagerException
    {
        public EPCManagerCopyException(string message) : base(message)
        {
        }

        public EPCManagerCopyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
