using System;


namespace Ripple.AST.Info.Types
{
	abstract class TypeInfo
	{
		public abstract void Accept(ITypeInfoVisitor iTypeInfoVisitor);
		public abstract T Accept<T>(ITypeInfoVisitor<T> iTypeInfoVisitor);
		public abstract TReturn Accept<TReturn, TArg>(ITypeInfoVisitor<TReturn, TArg> iTypeInfoVisitor, TArg arg);

        public override string ToString()
        {
            TypeInfoPrinterVisitor visitor = new TypeInfoPrinterVisitor();
            return this.Accept(visitor);
        }
    }
}
