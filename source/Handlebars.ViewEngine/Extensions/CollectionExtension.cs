using Magxe.Handlebars.ViewEngine.Abstractions;

namespace Magxe.Handlebars.ViewEngine.Extensions
{
    public static class CollectionExtension
    {
        public static IHelperList Append<T>(this IHelperList list) where T: HandlebarsBaseHelper
        {
            list.Add(typeof(T));
            return list;
        }
    }
}