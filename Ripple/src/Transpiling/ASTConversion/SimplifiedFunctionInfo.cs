using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Transpiling.ASTConversion.SimplifiedTypes;
using Ripple.AST;
using Ripple.Utils;

namespace Ripple.Transpiling.ASTConversion
{
    class SimplifiedFunctionInfo
    {
        public readonly string Name;
        public readonly SimplifiedType Returned;
        public readonly List<Pair<string, SimplifiedType>> Parameters;
        public readonly SimplifiedType Type;

        public SimplifiedFunctionInfo(FuncDecl funcDecl)
        {
            Name = funcDecl.Name.Text;
            Returned = SimplifiedTypeGenerator.Generate(funcDecl.ReturnType);
            Parameters = funcDecl.Param.ParamList
                .Select(p => new Pair<string, SimplifiedType>(
                    p.Item2.Text, 
                    SimplifiedTypeGenerator.Generate(p.Item1)))
                .ToList();

            Type = new SFuncPtr(false, Parameters.Select(p => p.Second).ToList(), Returned);
        }

        public SimplifiedFunctionInfo(ExternalFuncDecl funcDecl)
        {
            Name = funcDecl.Name.Text;
            Returned = SimplifiedTypeGenerator.Generate(funcDecl.ReturnType);
            Parameters = funcDecl.Parameters.ParamList
                .Select(p => new Pair<string, SimplifiedType>(
                    p.Item2.Text,
                    SimplifiedTypeGenerator.Generate(p.Item1)))
                .ToList();

            Type = new SFuncPtr(false, Parameters.Select(p => p.Second).ToList(), Returned);
        }
    }
}
