using System.Linq.Expressions;
using System.Reflection;
using Magxe.Handlebars.Compiler.Structure;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Bind(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new BlockHelperFunctionBinder(context).Visit(expr);
        }

        private BlockHelperFunctionBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is BlockHelperExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override System.Linq.Expressions.Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var isInlinePartial = bhex.HelperName == "#*inline";

            var fb = new FunctionBuilder(CompilationContext.Configuration);


            var bindingContext = isInlinePartial ? (System.Linq.Expressions.Expression)CompilationContext.BindingContext :
                            System.Linq.Expressions.Expression.Property(
                                CompilationContext.BindingContext,
                                typeof(BindingContext).GetProperty("Value"));

            var body = fb.Compile(((BlockExpression)bhex.Body).Expressions, CompilationContext.BindingContext);
            var inversion = fb.Compile(((BlockExpression)bhex.Inversion).Expressions, CompilationContext.BindingContext);
            var helper = CompilationContext.Configuration.BlockHelpers[bhex.HelperName.Replace("#", "")];
            var arguments = new System.Linq.Expressions.Expression[]
            {
                System.Linq.Expressions.Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetProperty("TextWriter")),
                System.Linq.Expressions.Expression.New(
                        typeof(HelperOptions).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0],
                        body,
                        inversion),
                //this next arg is usually data, like { first: "Marc" } 
                //but for inline partials this is the complete BindingContext.
                bindingContext,
                System.Linq.Expressions.Expression.NewArrayInit(typeof(object), bhex.Arguments)
            };


            if (helper.Target != null)
            {
                return System.Linq.Expressions.Expression.Call(
                    System.Linq.Expressions.Expression.Constant(helper.Target),
#if netstandard
                    helper.GetMethodInfo(),
#else
                    helper.Method,
#endif
                    arguments);
            }
            else
            {
                return System.Linq.Expressions.Expression.Call(
#if netstandard
                    helper.GetMethodInfo(),
#else
                    helper.Method,
#endif
                    arguments);
            }
        }
    }
}

