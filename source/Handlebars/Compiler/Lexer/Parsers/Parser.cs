using System.IO;
using Magxe.Handlebars.Compiler.Lexer.Tokens;

namespace Magxe.Handlebars.Compiler.Lexer.Parsers
{
    internal abstract class Parser
    {
        public abstract Token Parse(TextReader reader);
    }
}

