using HandlebarsDotNet.ViewEngine.Abstractions;
using HandlebarsDotNet.ViewEngine.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;

namespace HandlebarsDotNet.ViewEngine
{
    public class HandlebarsView : IView
    {
        private readonly IHbsRenderer _renderer;

        public string Path { get; }

        public HandlebarsView(string path, IHbsRenderer renderer)
        {
            Path = path;
            _renderer = renderer;
        }

        public async Task RenderAsync(ViewContext context)
        {
            await Task.Run(async () =>
            {
                var viewData = context.ViewData;
                await context.Writer.WriteAsync(await _renderer.Render(Path, context.ViewData.Model));
            });
        }
    }
}