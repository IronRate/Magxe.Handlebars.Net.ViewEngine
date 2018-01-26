using Magxe.Handlebars.ViewEngine.Abstractions;
using Magxe.Handlebars.ViewEngine.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Magxe.Handlebars.ViewEngine
{
    internal class HbsRenderer : IHbsRenderer
    {
        private readonly HandlebarsViewEngineOptions _options;
        private readonly IHandlebars _handlebars;
        private readonly IExpressionCache _expressionCache;
        private readonly IServiceProvider _services;

        public HbsRenderer(IServiceProvider services, IHandlebars handlebars, IOptions<HandlebarsViewEngineOptions> options, IExpressionCache expressionCache)
        {
            _options = options.Value;
            _services = services.CreateScope().ServiceProvider;
            _expressionCache = expressionCache;
            _handlebars = handlebars;

            var helpersAction = _options.RegisterHelpers;
            if (helpersAction != null)
            {
                var helpers = new HelperList();
                helpersAction.Invoke(helpers);

                foreach (var helper in helpers)
                {
                    var h = _services.GetService(helper).Cast<HandlebarsBaseHelper>();
                    switch (h.HelperType)
                    {
                        case HelperType.HandlebarsHelper:
                            _handlebars.RegisterHelper(h.HelperName, h.HandlebarsHelper);
                            break;
                        case HelperType.HandlebarsBlockHelper:
                            _handlebars.RegisterHelper(h.HelperName, h.HandlebarsBlockHelper);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public async Task<string> Render(string filePath, object viewData)
        {
            var output = string.Empty;
            await Task.Run(() =>
            {
                var renderView = _expressionCache.GetRenderView(filePath);
                var data = viewData;
                output = renderView(data);
            });
            return output;
        }
    }
}