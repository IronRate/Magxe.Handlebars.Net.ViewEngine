using System;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    public class HandlebarsUndefinedBindingException : Exception
    {
        public HandlebarsUndefinedBindingException(string path, string missingKey) : base(missingKey + " is undefined")
        {
            this.Path = path;
            this.MissingKey = missingKey;
        }

        public string Path { get; set; }

        public string MissingKey { get; set; }
    }
}
