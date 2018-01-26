using Microsoft.Extensions.Options;
using static HandlebarsDotNet.ViewEngine.Constants;

namespace HandlebarsDotNet.ViewEngine
{
    public class HandlebarsViewEngineOptionsSetup : ConfigureOptions<HandlebarsViewEngineOptions>
    {
        public HandlebarsViewEngineOptionsSetup(): base(Configure) { }

        private new static void Configure(HandlebarsViewEngineOptions options)
        {
            options.ViewLocationFormats.Add(()=> "Views/{1}/{0}" + VIEW_EXTENSION);
            options.ViewLocationFormats.Add(()=> "Views/Shared/{0}" + VIEW_EXTENSION);
        }
}
}