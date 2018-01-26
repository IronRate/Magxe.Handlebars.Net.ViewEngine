using System;

namespace Magxe.Handlebars
{
    public class HandlebarsException : Exception
    {
        public HandlebarsException(string message)
            : base(message)
        {
        }

        public HandlebarsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

