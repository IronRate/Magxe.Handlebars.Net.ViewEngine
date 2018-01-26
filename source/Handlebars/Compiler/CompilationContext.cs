using System.Linq.Expressions;
using Magxe.Handlebars.Compiler.Structure;

namespace Magxe.Handlebars.Compiler
{
    internal class CompilationContext
    {
        private readonly HandlebarsConfiguration _configuration;
        private readonly ParameterExpression _bindingContext;

        public CompilationContext(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
            _bindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public virtual HandlebarsConfiguration Configuration
        {
            get { return _configuration; }
        }

        public virtual ParameterExpression BindingContext
        {
            get { return _bindingContext; }
        }
    }
}
