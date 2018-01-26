using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Magxe.Handlebars.ViewEngine.Abstractions;
using Magxe.Handlebars.ViewEngine.Extensions;
using Magxe.Handlebars.ViewEngine.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;

namespace Magxe.Handlebars.ViewEngine
{
    public class HandlebarsViewEngine : IHandlebarsViewEngine
    {
        private readonly HandlebarsViewEngineOptions _options;
        private readonly IHbsRenderer _renderer;

        public HandlebarsViewEngine(IOptions<HandlebarsViewEngineOptions> optionsAccessor, IHbsRenderer renderer)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _renderer = renderer;
        }

        public async Task<string> RenderViewWithDataAsync(string view, object viewData)
        {
            return await _renderer.Render(view, viewData);
        }

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            var controllerName = context.GetNormalizedRouteValue(Constants.CONTROLLER_KEY);

            var searchedLocations = new List<string>();
            foreach (var location in _options.ViewLocationFormats)
            {
                var viewPath = string.Format(location.Invoke(), viewName, controllerName);
                if (File.Exists(viewPath))
                {
                    return ViewEngineResult.Found($"FindView viewName:{viewName}", new HandlebarsView(viewPath, _renderer));
                }
                searchedLocations.Add(viewPath);
            }

            return ViewEngineResult.NotFound(viewName, searchedLocations);
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            var applicationRelativePath = PathHelper.GetAbsolutePath(executingFilePath, viewPath);

            if (!PathHelper.IsAbsolutePath(viewPath))
            {
                // Not a path this method can handle.
                return ViewEngineResult.NotFound(applicationRelativePath, Enumerable.Empty<string>());
            }

            // ReSharper disable once Mvc.ViewNotResolved
            return ViewEngineResult.Found("GetView", new HandlebarsView(applicationRelativePath, _renderer));
        }
    }
}
