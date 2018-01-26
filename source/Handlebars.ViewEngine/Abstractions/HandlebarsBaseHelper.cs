using System;
using System.IO;

namespace Magxe.Handlebars.ViewEngine.Abstractions
{
    public abstract class HandlebarsBaseHelper
    {
        public string HelperName { get; }

        public HelperType HelperType { get; }

        protected HandlebarsBaseHelper(string helperName, HelperType helperType)
        {
            HelperName = helperName;
            HelperType = helperType;
        }

        public virtual void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments)
        {
            throw new NotImplementedException(HelperName);
        }

        public virtual void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments)
        {
            throw new NotImplementedException(HelperName);
        }
    }

    public enum HelperType
    {
        HandlebarsHelper,
        HandlebarsBlockHelper
    }
}