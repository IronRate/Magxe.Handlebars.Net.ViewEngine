using System;
using Magxe.Handlebars.ViewEngine.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Magxe.Handlebars.ViewEngine
{
    public class HandlebarsMvcViewOptionsSetup : IConfigureOptions<MvcViewOptions>
    {
        private readonly IHandlebarsViewEngine _viewEngine;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="HandlebarsMvcViewOptionsSetup"/>.
        /// </summary>
        /// <param name="viewEngine">The <see cref="IHandlebarsViewEngine"/>.</param>
        public HandlebarsMvcViewOptionsSetup(IHandlebarsViewEngine viewEngine)
        {
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
        }

        /// <inheritdoc />
        /// <summary>
        /// Configures <paramref name="options" /> to use <see cref="T:Magxe.Handlebars.ViewEngine.HandlebarsViewEngine" />.
        /// </summary>
        /// <param name="options">The <see cref="T:Microsoft.AspNetCore.Mvc.MvcViewOptions" /> to configure.</param>
        public void Configure(MvcViewOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ViewEngines.Add(_viewEngine);
        }
    }
}