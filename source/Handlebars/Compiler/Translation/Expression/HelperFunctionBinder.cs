using Magxe.Handlebars.Compiler.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Bind(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new HelperFunctionBinder(context).Visit(expr);
        }

        private HelperFunctionBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitBlock(BlockExpression node)
        {
            return System.Linq.Expressions.Expression.Block(
                node.Variables,
                node.Expressions.Select(Visit));
        }

        protected override System.Linq.Expressions.Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex.Body is HelperExpression ? Visit(sex.Body) : sex;
        }

        protected override System.Linq.Expressions.Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return HandlebarsExpression.Boolish(Visit(bex.Condition));
        }

        protected override System.Linq.Expressions.Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return HandlebarsExpression.BlockHelper(
                bhex.HelperName,
                bhex.Arguments.Select(Visit),
                Visit(bhex.Body),
                Visit(bhex.Inversion));
        }

        protected override System.Linq.Expressions.Expression VisitSubExpression(SubExpressionExpression subex)
        {
            return HandlebarsExpression.SubExpression(
                Visit(subex.Expression));
        }

        protected override System.Linq.Expressions.Expression VisitHelperExpression(HelperExpression hex)
        {
            if (CompilationContext.Configuration.Helpers.ContainsKey(hex.HelperName))
            {
                var helper = CompilationContext.Configuration.Helpers[hex.HelperName];
                var arguments = new System.Linq.Expressions.Expression[]
                {
                    System.Linq.Expressions.Expression.Property(
                        CompilationContext.BindingContext,
#if netstandard
                        typeof(BindingContext).GetRuntimeProperty("TextWriter")),
#else
                        typeof(BindingContext).GetProperty("TextWriter")),
#endif
                    System.Linq.Expressions.Expression.Property(
                        CompilationContext.BindingContext,
#if netstandard
                        typeof(BindingContext).GetRuntimeProperty("Value")),
#else
                        typeof(BindingContext).GetProperty("Value")),
#endif
                    System.Linq.Expressions.Expression.NewArrayInit(typeof(object), hex.Arguments.Select(a => Visit(a)))
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
            else
            {
                return System.Linq.Expressions.Expression.Call(
                    System.Linq.Expressions.Expression.Constant(this),
#if netstandard
                    new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).GetMethodInfo(),
#else
                    new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).Method,
#endif
                    CompilationContext.BindingContext,
                    System.Linq.Expressions.Expression.Constant(hex.HelperName),
                    System.Linq.Expressions.Expression.NewArrayInit(typeof(object), hex.Arguments));
            }
        }

        private void LateBindHelperExpression(
            BindingContext context,
            string helperName,
            IEnumerable<object> arguments)
        {
            if (CompilationContext.Configuration.Helpers.ContainsKey(helperName))
            {
                var helper = CompilationContext.Configuration.Helpers[helperName];
                helper(context.TextWriter, context.Value, arguments.ToArray());
            }
            else
            {
                throw new HandlebarsRuntimeException(
                    $"Template references a helper that is not registered. Could not find helper '{helperName}'");
            }
        }
    }
}
