using Magxe.Handlebars.Compiler.Structure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class BoolishConverter : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Convert(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new BoolishConverter(context).Visit(expr);
        }

        private BoolishConverter(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return System.Linq.Expressions.Expression.Call(
#if netstandard
                new Func<object, bool>(HandlebarsUtils.IsTruthyOrNonEmpty).GetMethodInfo(),
#else
                new Func<object, bool>(HandlebarsUtils.IsTruthyOrNonEmpty).Method,
#endif
                Visit(bex.Condition));
        }

        protected override System.Linq.Expressions.Expression VisitBlock(BlockExpression node)
        {
            return System.Linq.Expressions.Expression.Block(
                node.Type,
                node.Variables,
                node.Expressions.Select(Visit));
        }

        protected override System.Linq.Expressions.Expression VisitUnary(UnaryExpression node)
        {
            return System.Linq.Expressions.Expression.MakeUnary(
                node.NodeType,
                Visit(node.Operand),
                node.Type);
        }

        protected override System.Linq.Expressions.Expression VisitMethodCall(MethodCallExpression node)
        {
            return System.Linq.Expressions.Expression.Call(
                Visit(node.Object),
                node.Method,
                node.Arguments.Select(Visit));
        }

        protected override System.Linq.Expressions.Expression VisitConditional(ConditionalExpression node)
        {
            return System.Linq.Expressions.Expression.Condition(
                Visit(node.Test),
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }
    }
}

