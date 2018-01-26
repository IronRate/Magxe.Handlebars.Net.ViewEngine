using System.Collections.Generic;
using System.Linq;
using Magxe.Handlebars.Compiler.Lexer.Tokens;
using Magxe.Handlebars.Compiler.Structure;

namespace Magxe.Handlebars.Compiler.Lexer.Converter
{
    internal class StaticConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new StaticConverter().ConvertTokens(sequence).ToList();
        }

        private StaticConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                if (item is StaticToken)
                {
                    if (((StaticToken)item).Value != string.Empty)
                    {
                        yield return HandlebarsExpression.Static(((StaticToken)item).Value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

