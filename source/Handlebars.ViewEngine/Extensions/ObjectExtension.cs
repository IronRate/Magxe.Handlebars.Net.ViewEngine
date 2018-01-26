using System.Diagnostics;

namespace HandlebarsDotNet.ViewEngine.Extensions
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