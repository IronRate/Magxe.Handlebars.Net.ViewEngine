using System.Collections.Generic;

namespace Magxe.Handlebars.Compiler.Lexer.Converter
{
    internal abstract class TokenConverter
    {
        public abstract IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

