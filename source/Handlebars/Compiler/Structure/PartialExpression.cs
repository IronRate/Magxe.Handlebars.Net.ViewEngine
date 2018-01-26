using System;
using System.Linq.Expressions;

namespace Magxe.Handlebars.Compiler.Structure
{
    internal class PartialExpression : HandlebarsExpression
    {
        private readonly Expression _partialName;
        private readonly Expression _argument;
        private readonly Expression _fallback;

        public PartialExpression(Expression partialName, Expression argument, Expression fallback)
        {
            _partialName = partialName;
            _argument = argument;
            _fallback = fallback;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.PartialExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public Expression PartialName
        {
            get { return _partialName; }
        }

        public Expression Argument
        {
            get { return _argument; }
        }

        public Expression Fallback
        {
            get { return _fallback; }
        }
    }
}

