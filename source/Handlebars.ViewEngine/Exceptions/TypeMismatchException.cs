using System;

namespace HandlebarsDotNet.ViewEngine.Extensions
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(Type subType, Type parentType) : base(
            $"Type {subType} doesn't implement interface {parentType}")
        {
        }
    }
}