using Magxe.Handlebars.Compiler.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Magxe.Handlebars.Compiler.Translation.Expression
{
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        public static System.Linq.Expressions.Expression Bind(System.Linq.Expressions.Expression expr,
            CompilationContext context)
        {
            return new IteratorBinder(context).Visit(expr);
        }

        private IteratorBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override System.Linq.Expressions.Expression VisitBlock(BlockExpression node)
        {
            return System.Linq.Expressions.Expression.Block(
                node.Type,
                node.Variables,
                node.Expressions.Select(Visit));
        }

        protected override System.Linq.Expressions.Expression VisitConditional(ConditionalExpression node)
        {
            return System.Linq.Expressions.Expression.Condition(
                Visit(node.Test),
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }

        protected override System.Linq.Expressions.Expression VisitUnary(UnaryExpression node)
        {
            return System.Linq.Expressions.Expression.MakeUnary(
                node.NodeType,
                Visit(node.Operand),
                node.Type);
        }

        protected override System.Linq.Expressions.Expression VisitIteratorExpression(IteratorExpression iex)
        {
            var iteratorBindingContext = System.Linq.Expressions.Expression.Variable(typeof(BindingContext), "context");
            return System.Linq.Expressions.Expression.Block(
                new[]
                {
                    iteratorBindingContext
                },
                System.Linq.Expressions.Expression.IfThenElse(
                    System.Linq.Expressions.Expression.TypeIs(iex.Sequence, typeof(IEnumerable)),
                    System.Linq.Expressions.Expression.IfThenElse(
#if netstandard
                        System.Linq.Expressions.Expression.Call(new Func<object, bool>(IsNonListDynamic).GetMethodInfo(), new[] { iex.Sequence }),
#else
                        System.Linq.Expressions.Expression.Call(new Func<object, bool>(IsNonListDynamic).Method,
                            new[] {iex.Sequence}),
#endif
                        GetDynamicIterator(iteratorBindingContext, iex),
                        System.Linq.Expressions.Expression.IfThenElse(
#if netstandard
                            System.Linq.Expressions.Expression.Call(new Func<object, bool>(IsGenericDictionary).GetMethodInfo(), new[] { iex.Sequence }),
#else
                            System.Linq.Expressions.Expression.Call(new Func<object, bool>(IsGenericDictionary).Method,
                                new[] {iex.Sequence}),
#endif
                            GetDictionaryIterator(iteratorBindingContext, iex),
                            GetEnumerableIterator(iteratorBindingContext, iex))),
                    GetObjectIterator(iteratorBindingContext, iex))
            );
        }

        private System.Linq.Expressions.Expression GetEnumerableIterator(
            System.Linq.Expressions.Expression contextParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return System.Linq.Expressions.Expression.Block(
                System.Linq.Expressions.Expression.Assign(contextParameter,
                    System.Linq.Expressions.Expression.New(
                        typeof(IteratorBindingContext).GetConstructor(new[] {typeof(BindingContext)}),
                        new System.Linq.Expressions.Expression[] {CompilationContext.BindingContext})),
                System.Linq.Expressions.Expression.Call(
#if netstandard
                    new Action<IteratorBindingContext, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).GetMethodInfo(),
#else
                    new Action<IteratorBindingContext, IEnumerable, Action<TextWriter, object>,
                        Action<TextWriter, object>>(Iterate).Method,
#endif
                    new[]
                    {
                        System.Linq.Expressions.Expression.Convert(contextParameter, typeof(IteratorBindingContext)),
                        System.Linq.Expressions.Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                        fb.Compile(new[] {iex.Template}, contextParameter),
                        fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext)
                    }));
        }

        private System.Linq.Expressions.Expression GetObjectIterator(
            System.Linq.Expressions.Expression contextParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return System.Linq.Expressions.Expression.Block(
                System.Linq.Expressions.Expression.Assign(contextParameter,
                    System.Linq.Expressions.Expression.New(
                        typeof(ObjectEnumeratorBindingContext).GetConstructor(new[] {typeof(BindingContext)}),
                        CompilationContext.BindingContext)),
                System.Linq.Expressions.Expression.Call(
#if netstandard
                    new Action<ObjectEnumeratorBindingContext, object, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).GetMethodInfo(),
#else
                    new Action<ObjectEnumeratorBindingContext, object, Action<TextWriter, object>,
                        Action<TextWriter, object>>(Iterate).Method,
#endif
                    new[]
                    {
                        System.Linq.Expressions.Expression.Convert(contextParameter,
                            typeof(ObjectEnumeratorBindingContext)),
                        iex.Sequence,
                        fb.Compile(new[] {iex.Template}, contextParameter),
                        fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext)
                    }));
        }

        private System.Linq.Expressions.Expression GetDictionaryIterator(
            System.Linq.Expressions.Expression contextParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return System.Linq.Expressions.Expression.Block(
                System.Linq.Expressions.Expression.Assign(contextParameter,
                    System.Linq.Expressions.Expression.New(
                        typeof(ObjectEnumeratorBindingContext).GetConstructor(new[] {typeof(BindingContext)}),
                        CompilationContext.BindingContext)),
                System.Linq.Expressions.Expression.Call(
#if netstandard
                    new Action<ObjectEnumeratorBindingContext, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).GetMethodInfo(),
#else
                    new Action<ObjectEnumeratorBindingContext, IEnumerable, Action<TextWriter, object>,
                        Action<TextWriter, object>>(Iterate).Method,
#endif
                    new[]
                    {
                        System.Linq.Expressions.Expression.Convert(contextParameter,
                            typeof(ObjectEnumeratorBindingContext)),
                        System.Linq.Expressions.Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                        fb.Compile(new[] {iex.Template}, contextParameter),
                        fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext)
                    }));
        }

        private System.Linq.Expressions.Expression GetDynamicIterator(
            System.Linq.Expressions.Expression contextParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return System.Linq.Expressions.Expression.Block(
                System.Linq.Expressions.Expression.Assign(contextParameter,
                    System.Linq.Expressions.Expression.New(
                        typeof(ObjectEnumeratorBindingContext).GetConstructor(new[] {typeof(BindingContext)}),
                        CompilationContext.BindingContext)),
                System.Linq.Expressions.Expression.Call(
#if netstandard
                    new Action<ObjectEnumeratorBindingContext, IDynamicMetaObjectProvider, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).GetMethodInfo(),
#else
                    new Action<ObjectEnumeratorBindingContext, IDynamicMetaObjectProvider, Action<TextWriter, object>,
                        Action<TextWriter, object>>(Iterate).Method,
#endif
                    new[]
                    {
                        System.Linq.Expressions.Expression.Convert(contextParameter,
                            typeof(ObjectEnumeratorBindingContext)),
                        System.Linq.Expressions.Expression.Convert(iex.Sequence, typeof(IDynamicMetaObjectProvider)),
                        fb.Compile(new[] {iex.Template}, contextParameter),
                        fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext)
                    }));
        }

        private static bool IsNonListDynamic(object target)
        {
            var interfaces = target.GetType().GetInterfaces();
            return interfaces.Contains(typeof(IDynamicMetaObjectProvider))
                   && ((IDynamicMetaObjectProvider) target)
                   .GetMetaObject(System.Linq.Expressions.Expression.Constant(target)).GetDynamicMemberNames().Any();
        }

        private static bool IsGenericDictionary(object target)
        {
            return
                target.GetType()
#if netstandard
                    .GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)

#else
                    .GetInterfaces()
                    .Where(i => i.IsGenericType)
#endif
                    .Any(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private static void Iterate(
            ObjectEnumeratorBindingContext context,
            object target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            if (HandlebarsUtils.IsTruthy(target))
            {
                context.Index = 0;
                foreach (var member in target.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public).OfType<MemberInfo>()
                    .Concat(
                        target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                    ))
                {
                    context.Key = member.Name;
                    var value = AccessMember(target, member);
                    context.First = (context.Index == 0);
                    template(context.TextWriter, value);
                    context.Index++;
                }

                if (context.Index == 0)
                {
                    ifEmpty(context.TextWriter, context.Value);
                }
            }
            else
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static void Iterate(
            ObjectEnumeratorBindingContext context,
            IEnumerable target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            if (HandlebarsUtils.IsTruthy(target))
            {
                context.Index = 0;
#if netstandard
                var keysProperty = target.GetType().GetRuntimeProperty("Keys");
#else
                var keysProperty = target.GetType().GetProperty("Keys");
#endif
                if (keysProperty != null)
                {
                    if (keysProperty.GetGetMethod().Invoke(target, null) is IEnumerable keys)
                    {
                        foreach (var key in keys)
                        {
                            context.Key = key.ToString();
                            var value = target.GetType().GetMethod("get_Item").Invoke(target, new[] {key});
                            context.First = (context.Index == 0);
                            template(context.TextWriter, value);
                            context.Index++;
                        }
                    }
                }

                if (context.Index == 0)
                {
                    ifEmpty(context.TextWriter, context.Value);
                }
            }
            else
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static void Iterate(
            ObjectEnumeratorBindingContext context,
            IDynamicMetaObjectProvider target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            if (HandlebarsUtils.IsTruthy(target))
            {
                context.Index = 0;
                var meta = target.GetMetaObject(System.Linq.Expressions.Expression.Constant(target));
                foreach (var name in meta.GetDynamicMemberNames())
                {
                    context.Key = name;
                    var value = GetProperty(target, name);
                    context.First = (context.Index == 0);
                    template(context.TextWriter, value);
                    context.Index++;
                }

                if (context.Index == 0)
                {
                    ifEmpty(context.TextWriter, context.Value);
                }
            }
            else
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static void Iterate(
            IteratorBindingContext context,
            IEnumerable sequence,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            context.Index = 0;

            var iter = sequence.GetEnumerator();
            using (iter as IDisposable)
            {
                if (iter.MoveNext())
                {
                    var item = iter.Current;
                    while (!context.Last)
                    {
                        context.Last = !iter.MoveNext();
                        context.First = (context.Index == 0);
                        template(context.TextWriter, item);
                        context.Index++;

                        if (!context.Last)
                        {
                            item = iter.Current;
                        }
                    }
                }
            }

            if (context.Index == 0)
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices
                .CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(
                    Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(),
                        new[] {Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null)}));
            return site.Target(site, target);
        }

        private class IteratorBindingContext : BindingContext
        {
            public IteratorBindingContext(BindingContext context)
                : base(context.Value, context.TextWriter, context.ParentContext, context.TemplatePath)
            {
            }

            public int Index { get; set; }

            public bool First { get; set; }

            public bool Last { get; set; }
        }

        private class ObjectEnumeratorBindingContext : BindingContext
        {
            public ObjectEnumeratorBindingContext(BindingContext context)
                : base(context.Value, context.TextWriter, context.ParentContext, context.TemplatePath)
            {
            }

            public string Key { get; set; }

            public int Index { get; set; }

            public bool First { get; set; }
        }

        private static object AccessMember(object instance, MemberInfo member)
        {
            switch (member)
            {
                case PropertyInfo info:
                    return info.GetValue(instance, null);
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(instance);
            }

            throw new InvalidOperationException("Requested member was not a field or property");
        }
    }
}