using System;


namespace Scanning
{
    public class UnsupportedScanTechnologyException : Exception
    {

        public UnsupportedScanTechnologyException()
        {
        }

        public UnsupportedScanTechnologyException(string message)
            : base(message)
        {
        }

    }
}