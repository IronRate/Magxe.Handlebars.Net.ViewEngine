using Microsoft.Extensions.Options;

namespace Magxe.Handlebars.ViewEngine
{
    public class HandlebarsViewEngineOptionsSetup : ConfigureOptions<HandlebarsViewEngineOptions>
    {
        public HandlebarsViewEngineOptionsSetup(): base(Configure) { }

        private new static void Configure(HandlebarsViewEngineOptions options)
        {
            options.ViewLocationFormats.Add(()=> "Views/{1}/{0}" + Constants.VIEW_EXTENSION);
            options.ViewLocationFormats.Add(()=> "Views/Shared/{0}" + Constants.VIEW_EXTENSION);
        }
}
}