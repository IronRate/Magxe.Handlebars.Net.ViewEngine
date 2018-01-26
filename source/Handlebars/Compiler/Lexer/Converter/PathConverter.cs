using System.Collections.Generic;
using System.Linq;
using Magxe.Handlebars.Compiler.Lexer.Tokens;
using Magxe.Handlebars.Compiler.Structure;

namespace Magxe.Handlebars.Compiler.Lexer.Converter
{
    internal class PathConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new PathConverter().ConvertTokens(sequence).ToList();
        }

        private PathConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                if (item is WordExpressionToken)
                {
                    yield return HandlebarsExpression.Path(((WordExpressionToken)item).Value);
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

