using Magxe.Handlebars.ViewEngine.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Magxe.Handlebars.ViewEngine
{
    internal class ExpressionCache : IExpressionCache
    {
        private readonly IHandlebars _handlebars;
        private readonly Dictionary<string, Func<object, string>> _dict = new Dictionary<string, Func<object, string>>();

        public ExpressionCache(IHandlebars handlebars)
        {
            _handlebars = handlebars;
        }

        public Func<object, string> GetRenderView(string path)
        {
            if (File.Exists(path))
            {
                return _handlebars.CompileView(path);
            }
            else
            {
                return _handlebars.Compile(path);
            }
            var partials = Path.GetFileNameWithoutExtension(path);
            _dict.TryGetValue(partials, out var renderView);
#if DEBUG
            if (renderView == null)
            {
                Debug.WriteLine($"[ExpressionCache] Require file: {path}, cache status: NOT HIT");
            }
            else
            {
                Debug.WriteLine($"[ExpressionCache] Require file: {path}, cache status: HIT");
            }
#endif

            if (renderView == null)
            {
                renderView = _handlebars.CompileView(path);
                _dict.Add(partials, renderView);
            }

            return renderView;
        }
    }
}