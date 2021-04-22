using System;

namespace DeclarativePM.Lib.Exceptions
{
    public class LogValueNotSetException : Exception
    {
        public LogValueNotSetException() { }

        public LogValueNotSetException(string message)
            : base(message) { }

        public LogValueNotSetException(string message, Exception inner)
            : base(message, inner) { }
    }
}