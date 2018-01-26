﻿using System;

namespace Magxe.Handlebars.Compiler
{
    public class HandlebarsCompilerException : HandlebarsException
    {
        public HandlebarsCompilerException(string message)
            : base(message)
        {
        }

        public HandlebarsCompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

