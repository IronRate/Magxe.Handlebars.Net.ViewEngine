namespace Magxe.Handlebars.Compiler.Lexer.Tokens
{
    internal class HashParameterToken : ExpressionToken
    {
        private readonly string _parameter;

        public HashParameterToken(string parameter)
        {
            _parameter = parameter;
        }

        public override TokenType Type
        {
            get { return TokenType.HashParameter; }
        }

        public override string Value
        {
            get { return _parameter; }
        }
    }
}

