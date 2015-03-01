using System;


namespace ScanEngine
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