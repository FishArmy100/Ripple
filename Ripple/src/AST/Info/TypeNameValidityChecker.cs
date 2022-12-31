﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Lexing;
using Ripple.Utils;
using Ripple.AST.Utils;

namespace Ripple.AST.Info
{
    class TypeNameValidityChecker : TypeWalkerBase
    {
        private readonly List<PrimaryTypeInfo> m_PrimaryTypes;
        private readonly List<Token> m_ActiveLifetimes;
        public readonly List<ASTInfoError> Errors = new List<ASTInfoError>();

        public TypeNameValidityChecker(TypeName typeName, List<PrimaryTypeInfo> primaryTypes, List<Token> activeLifetimes)
        {
            this.m_PrimaryTypes = primaryTypes;
            this.m_ActiveLifetimes = activeLifetimes;
            typeName.Accept(this);
        }

        public override void VisitBasicType(BasicType basicType)
        {
            PrimaryTypeInfo comparator = new PrimaryTypeInfo(basicType.Identifier);
            if (!m_PrimaryTypes.Contains(comparator))
                Errors.Add(new ASTInfoError("Type '" + comparator.Name.Text + "' has not been defined.", comparator.Name));
        }

        public override void VisitReferenceType(ReferenceType referenceType)
        {
            if (referenceType.Lifetime is Token lifetime && !m_ActiveLifetimes.Contains(lifetime))
                Errors.Add(new ASTInfoError("Liftime '" + lifetime.Text + "' has not been defined.", lifetime));

            base.VisitReferenceType(referenceType);
        }
    }
}
