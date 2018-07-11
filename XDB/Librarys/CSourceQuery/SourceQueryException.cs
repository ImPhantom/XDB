using System;

namespace Phantom.CSourceQuery
{
    public class SourceQueryException : Exception
    {
        public SourceQueryException(string message) : base(message) { }
    }
}
