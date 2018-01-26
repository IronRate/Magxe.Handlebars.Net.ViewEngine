using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Magxe.Handlebars.Compiler.Lexer.Tokens;

namespace Magxe.Handlebars.Compiler.Lexer.Converter
{
    internal class LiteralConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new LiteralConverter().ConvertTokens(sequence).ToList();
        }

        private LiteralConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                if (item is LiteralExpressionToken)
                {
                    yield return Expression.Constant(((LiteralExpressionToken)item).Value);
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

