using System.Diagnostics;

namespace Magxe.Handlebars.ViewEngine.Extensions
{
    internal static class ObjectExtension
    {
        [DebuggerHidden]
        public static T Cast<T>(this object o)
        {
            return (T)o;
        }
    }
}