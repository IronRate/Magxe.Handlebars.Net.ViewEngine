using System;

namespace Magxe.Handlebars.ViewEngine.Exceptions
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(Type subType, Type parentType) : base(
            $"Type {subType} doesn't implement interface {parentType}")
        {
        }
    }
}