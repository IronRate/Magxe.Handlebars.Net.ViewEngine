using System;
using System.IO;
using System.Linq;
using Magxe.Handlebars.Compiler.Structure;

#if netstandard
using System.Reflection;
#endif

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        private static string SpecialPartialBlockName = "@partial-block";

        public static System.Linq.Expressions.Expression Bind(System.Linq.Expressions.Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PartialExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override System.Linq.Expressions.Expression VisitPartialExpression(PartialExpression pex)
        {
            System.Linq.Expressions.Expression bindingContext = CompilationContext.BindingContext;
            if (pex.Argument != null)
            {
                bindingContext = System.Linq.Expressions.Expression.Call(
                    bindingContext,
                    typeof(BindingContext).GetMethod("CreateChildContext"),
                    pex.Argument);
            }

            var fb = new FunctionBuilder(CompilationContext.Configuration);
            var partialBlockTemplate =
                fb.Compile(pex.Fallback != null ? new[] {pex.Fallback} : Enumerable.Empty<System.Linq.Expressions.Expression>(), bindingContext);

            var partialInvocation = System.Linq.Expressions.Expression.Call(
#if netstandard
                new Func<string, BindingContext, HandlebarsConfiguration, Action<TextWriter, object>, bool>(InvokePartial).GetMethodInfo(),
#else
                new Func<string, BindingContext, HandlebarsConfiguration, Action<TextWriter, object>, bool>(InvokePartial).Method,
#endif
                System.Linq.Expressions.Expression.Convert(pex.PartialName, typeof(string)),
                bindingContext,
                System.Linq.Expressions.Expression.Constant(CompilationContext.Configuration),
                partialBlockTemplate);

            var fallback = pex.Fallback;
            if (fallback == null)
            {
                fallback = System.Linq.Expressions.Expression.Call(
#if netstandard
                new Action<string>(HandleFailedInvocation).GetMethodInfo(),
#else
                new Action<string>(HandleFailedInvocation).Method,
#endif
                System.Linq.Expressions.Expression.Convert(pex.PartialName, typeof(string)));
            }

            return System.Linq.Expressions.Expression.IfThen(
                    System.Linq.Expressions.Expression.Not(partialInvocation),
                    fallback);
        }

        private static void HandleFailedInvocation(
            string partialName)
        {
            throw new HandlebarsRuntimeException(
                string.Format("Referenced partial name {0} could not be resolved", partialName));
        }

        private static bool InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration,
            Action<TextWriter, object> partialBlockTemplate)
        {
            var partialBindingContext = context as PartialBindingContext;
            if (partialName.Equals(SpecialPartialBlockName) && partialBindingContext != null)
            {
                if (partialBindingContext.PartialBlockTemplate == null)
                {
                    return false;
                }

                partialBindingContext.PartialBlockTemplate(context.TextWriter, context);
                return true;
            }

            context = new PartialBindingContext(context)
            {
                PartialBlockTemplate = partialBlockTemplate
            };

            //if we have an inline partial, skip the file system and RegisteredTemplates collection
            if (context.InlinePartialTemplates.ContainsKey(partialName))
            {
                context.InlinePartialTemplates[partialName](context.TextWriter, context);
                return true;
            }

            
            if (configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                if (configuration.FileSystem != null && context.TemplatePath != null)
                {
                    var partialPath = configuration.FileSystem.Closest(context.TemplatePath,
                        "partials/" + partialName + ".hbs");
                    if (partialPath != null)
                    {
                        var compiled = Handlebars.Create(configuration)
                            .CompileView(partialPath);
                        configuration.RegisteredTemplates.Add(partialName, (writer, o) =>
                        {
                            writer.Write(compiled(o));
                        });
                    }
                    else
                    {
                        // Failed to find partial in filesystem
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            try
            {
                configuration.RegisteredTemplates[partialName](context.TextWriter, context);
                return true;
            }
            catch (Exception exception)
            {
                throw new HandlebarsRuntimeException(
                    $"Runtime error while rendering partial '{partialName}', see inner exception for more information",
                    exception);
            }

        }

        private class PartialBindingContext : BindingContext
        {
            public PartialBindingContext(BindingContext context)
                : base(context.Value, context.TextWriter, context.ParentContext, context.TemplatePath, context)
            {
            }

            public Action<TextWriter, object> PartialBlockTemplate { get; set; }

            public override BindingContext CreateChildContext(object value)
            {
                return new PartialBindingContext(base.CreateChildContext(value))
                {
                    PartialBlockTemplate = PartialBlockTemplate
                };
            }
        }
    }
}

