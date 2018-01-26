using Magxe.Handlebars.Compiler.Structure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class StaticReplacer : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Replace(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new StaticReplacer(context).Visit(expr);
        }

        private StaticReplacer(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitBlock(BlockExpression node)
        {
            return System.Linq.Expressions.Expression.Block(
                node.Variables,
                node.Expressions.Select(Visit));
        }

        protected override System.Linq.Expressions.Expression VisitStaticExpression(StaticExpression stex)
        {
	        var encodedTextWriter = System.Linq.Expressions.Expression.Property(CompilationContext.BindingContext, "TextWriter");
#if netstandard
            var writeMethod = typeof(EncodedTextWriter).GetRuntimeMethod("Write", new [] { typeof(string), typeof(bool) });
#else
            var writeMethod = typeof(EncodedTextWriter).GetMethod("Write", new [] { typeof(string), typeof(bool) });
#endif

            return System.Linq.Expressions.Expression.Call(encodedTextWriter, writeMethod, System.Linq.Expressions.Expression.Constant(stex.Value), System.Linq.Expressions.Expression.Constant(false));
        }

        protected override System.Linq.Expressions.Expression VisitConditional(ConditionalExpression node)
        {
            return System.Linq.Expressions.Expression.Condition(
                node.Test,
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }
    }
}

