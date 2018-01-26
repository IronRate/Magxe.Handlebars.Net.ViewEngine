﻿using Magxe.Handlebars.Compiler.Structure;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class CommentVisitor : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression expr, CompilationContext compilationContext)
        {
            return new CommentVisitor(compilationContext).Visit(expr);
        }

        private CommentVisitor(CompilationContext compilationContext) 
            : base(compilationContext)
        {
        }

        protected override System.Linq.Expressions.Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is CommentExpression)
            {
                return System.Linq.Expressions.Expression.Empty();
            }

            return sex;
        }
    }
}