using System;

namespace HandlebarsDotNet.ViewEngine.Abstractions
{
    internal interface IExpressionCache
    {
        Func<object, string> GetRenderView(string path);
    }
}