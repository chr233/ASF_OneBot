using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ASF_OneBot.Exceptions
{
    internal class BotException : ApplicationException
    {
        public BotException() { }
        public BotException(string message) : base(message) { }
        public BotException(string message, Exception innerException) : base(message, innerException) { }
        protected BotException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
    internal class BotNotFound : BotException
    {
        public BotNotFound() { }

        public BotNotFound(string message) : base(message) { }

        public BotNotFound(string message, Exception innerException) : base(message, innerException) { }

        protected BotNotFound(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
