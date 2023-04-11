using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.Core;
using Ripple.AST.Utils;
using Ripple.Validation.Info;

namespace Ripple.Validation.Errors
{
    class UnsafeThingIsInSafeContextError : ValidationError
    {
        public readonly string ThingName;
        public readonly UnsafeThingType UnsafeThing;

        public enum UnsafeThingType
        { 
            Function,
            Variable,
            Type
        }

        private UnsafeThingIsInSafeContextError(SourceLocation location, string thingName, UnsafeThingType type) : base(location)
        {
            ThingName = thingName;
            UnsafeThing = type;
        }

        public static UnsafeThingIsInSafeContextError Function(SourceLocation location, string name)
        {
            return new UnsafeThingIsInSafeContextError(location, name, UnsafeThingType.Function);
        }

        public static UnsafeThingIsInSafeContextError Variable(SourceLocation location, string name)
        {
            return new UnsafeThingIsInSafeContextError(location, name, UnsafeThingType.Variable);
        }

        public static UnsafeThingIsInSafeContextError Type(SourceLocation location, TypeName type)
        {
            return new UnsafeThingIsInSafeContextError(location, TypeNamePrinter.PrintType(type), UnsafeThingType.Function);
        }

        public override string GetMessage() => $"Unsafe {UnsafeThing} '{ThingName}' in a safe context.";
    }
}
