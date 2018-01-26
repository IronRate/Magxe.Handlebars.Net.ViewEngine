using System;
using System.Collections.Generic;
using Magxe.Handlebars.ViewEngine.Abstractions;

namespace Magxe.Handlebars.ViewEngine
{
    public class HandlebarsViewEngineOptions
    {
        public IList<Func<string>> ViewLocationFormats { get; } = new List<Func<string>>();
        public Action<IHelperList> RegisterHelpers = null;
        public string BaseDir { get; set; }
    }
}