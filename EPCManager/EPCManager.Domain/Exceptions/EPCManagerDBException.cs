using System;

namespace EPCManager.Domain.Exceptions
{
    public class EPCManagerDBException : EPCManagerException
    {
        public EPCManagerDBException(string message) : base(message)
        {
        }

        public EPCManagerDBException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
