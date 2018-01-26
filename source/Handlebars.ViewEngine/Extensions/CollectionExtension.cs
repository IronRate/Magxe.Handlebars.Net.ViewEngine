using System.Collections.Generic;
using HandlebarsDotNet.ViewEngine.Abstractions;

namespace HandlebarsDotNet.ViewEngine.Extensions
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