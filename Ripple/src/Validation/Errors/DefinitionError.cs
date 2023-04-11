using Ripple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST;
using Ripple.AST.Utils;

namespace Ripple.Validation.Errors
{
    abstract class DefinitionError : ValidationError
    {
        public bool WasDefined;

        protected DefinitionError(SourceLocation location, bool wasDefined) : base(location)
        {
            WasDefined = wasDefined;
        }

        protected abstract string Name();
        protected abstract string ObjectText();

        public override sealed string GetMessage() => 
            $"{Name()} {ObjectText()} has " + (WasDefined ? 
            "already been defined." : 
            "not been defined.");

        public class Function : DefinitionError
        {
            public string FunctionName;

            public Function(SourceLocation location, bool wasDefined, string functionName) : base(location, wasDefined)
            {
                FunctionName = functionName;
            }

            protected override string Name()
            {
                return "Function";
            }

            protected override string ObjectText()
            {
                return FunctionName;
            }
        }

        public class Variable : DefinitionError
        {
            public string VariableName;

            public Variable(SourceLocation location, bool wasDefined, string variableName) : base(location, wasDefined)
            {
                VariableName = variableName;
            }

            protected override string Name()
            {
                return "Variable";
            }

            protected override string ObjectText()
            {
                return VariableName;
            }
        }

        public class Lifetime : DefinitionError
        {
            public readonly string LifetimeText;

            public Lifetime(SourceLocation location, bool wasDefined, string lifetimeText) : base(location, wasDefined)
            {
                LifetimeText = lifetimeText;
            }

            protected override string Name()
            {
                return "Lifetime";
            }

            protected override string ObjectText()
            {
                return LifetimeText;
            }
        }

        public class Type : DefinitionError
        {
            public readonly TypeName TypeName;
            public Type(SourceLocation location, bool wasDefined, TypeName typeName) : base(location, wasDefined)
            {
                TypeName = typeName;
            }

            protected override string Name()
            {
                return "Type";
            }

            protected override string ObjectText()
            {
                return TypeNamePrinter.PrintType(TypeName);
            }
        }
    }
}
