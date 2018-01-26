using HandlebarsDotNet.ViewEngine.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.ViewEngine.Extensions
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddHandlebarsViewEngine(this IMvcBuilder builder,
            Action<HandlebarsViewEngineOptions> optionsAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddOptions()
                .AddTransient<IConfigureOptions<HandlebarsViewEngineOptions>, HandlebarsViewEngineOptionsSetup>();

            if (optionsAction != null)
            {
                builder.Services.Configure(optionsAction);
            }

            var options = builder.Services.BuildServiceProvider().GetService<IOptions<HandlebarsViewEngineOptions>>()
                .Value;
            if (options.RegisterHelpers != null)
            {
                var helpers = new HelperList();
                options.RegisterHelpers.Invoke(helpers);
                var typeBaseHelper = typeof(HandlebarsBaseHelper);
                foreach (var helper in helpers)
                {
                    if (helper.IsSubclassOf(typeBaseHelper))
                    {
                        builder.Services.AddScoped(helper);
                    }
                    else
                    {
                        throw new TypeMismatchException(helper, typeBaseHelper);
                    }
                }
            }

            builder.Services
                .AddTransient<IConfigureOptions<MvcViewOptions>, HandlebarsMvcViewOptionsSetup>()
                .AddSingleton<IExpressionCache, ExpressionCache>()
                .AddSingleton<IHandlebars>(provider => Handlebars.Create(new HandlebarsConfiguration()
                    {FileSystem = new DiskFileSystem()}))
                .AddSingleton<IHbsRenderer, HbsRenderer>()
                .AddSingleton<IHandlebarsViewEngine, HandlebarsViewEngine>();

            return builder;
        }
    }
}