using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Magxe.Handlebars.Compiler.Structure;

#if netstandard
using System.Reflection;
#endif

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        private ContextBinder()
            : base(null)
        {
        }

        public static System.Linq.Expressions.Expression Bind(System.Linq.Expressions.Expression body, CompilationContext context, System.Linq.Expressions.Expression parentContext, string templatePath)
        {
            var writerParameter = System.Linq.Expressions.Expression.Parameter(typeof(TextWriter), "buffer");
            var objectParameter = System.Linq.Expressions.Expression.Parameter(typeof(object), "data");
            if (parentContext == null)
            {
                parentContext = System.Linq.Expressions.Expression.Constant(null, typeof(BindingContext));
            }

            var encodedWriterExpression = ResolveEncodedWriter(writerParameter, context.Configuration.TextEncoder);
            var templatePathExpression = System.Linq.Expressions.Expression.Constant(templatePath, typeof(string));
            var newBindingContext = System.Linq.Expressions.Expression.New(
                            typeof(BindingContext).GetConstructor(
                                new[] { typeof(object), typeof(EncodedTextWriter), typeof(BindingContext), typeof(string) }),
                            new[] { objectParameter, encodedWriterExpression, parentContext, templatePathExpression });
            return System.Linq.Expressions.Expression.Lambda<Action<TextWriter, object>>(
                System.Linq.Expressions.Expression.Block(
                    new[] { context.BindingContext },
                    new System.Linq.Expressions.Expression[]
                    {
                        System.Linq.Expressions.Expression.IfThenElse(
                            System.Linq.Expressions.Expression.TypeIs(objectParameter, typeof(BindingContext)),
                            System.Linq.Expressions.Expression.Assign(context.BindingContext, System.Linq.Expressions.Expression.TypeAs(objectParameter, typeof(BindingContext))),
                            System.Linq.Expressions.Expression.Assign(context.BindingContext, newBindingContext))
                    }.Concat(
                        ((BlockExpression)body).Expressions
                    )),
                new[] { writerParameter, objectParameter });
        }

        private static System.Linq.Expressions.Expression ResolveEncodedWriter(ParameterExpression writerParameter, ITextEncoder textEncoder)
        {
            var outputEncoderExpression = System.Linq.Expressions.Expression.Constant(textEncoder, typeof(ITextEncoder));

#if netstandard
            var encodedWriterFromMethod = typeof(EncodedTextWriter).GetRuntimeMethod("From", new[] { typeof(TextWriter), typeof(ITextEncoder) });
#else
            var encodedWriterFromMethod = typeof(EncodedTextWriter).GetMethod("From", new[] { typeof(TextWriter), typeof(ITextEncoder) });
#endif

            return System.Linq.Expressions.Expression.Call(encodedWriterFromMethod, writerParameter, outputEncoderExpression);
        }
    }
}