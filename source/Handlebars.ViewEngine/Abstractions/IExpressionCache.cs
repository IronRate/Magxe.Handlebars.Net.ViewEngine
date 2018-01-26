using System;

namespace Magxe.Handlebars.ViewEngine.Abstractions
{
    internal interface IExpressionCache
    {
        Func<object, string> GetRenderView(string path);
    }
}