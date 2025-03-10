using System;
using System.Runtime.Serialization;

namespace Omnae.BusinessLayer
{
    [Serializable]
    public class BLException : Exception
    {
        public BLException()
        {
        }

        public BLException(string message) : base(message)
        {
        }

        public BLException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected BLException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
