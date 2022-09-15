using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using Ripple.Utils.Extensions;

namespace Ripple.Transpiling
{
    abstract class CStatement
    {
        public abstract string ConvertToCCode(int offset);
        
        public class Package : CStatement
        {
            public const string Seperator = "#####";

            public readonly string Name;
            public readonly List<Func> Functions;
            public readonly List<Var> GlobalVariables;
            public readonly List<string> AdditionalIncludes;

            public Package(string name, List<Func> functions, List<Var> globalVariables, List<string> additionalIncludes)
            {
                Name = name;
                Functions = functions;
                GlobalVariables = globalVariables;
                AdditionalIncludes = additionalIncludes;
            }

            public override string ConvertToCCode(int offset)
            {
                string header = GenHeader();
                string cpp = GenCpp(offset);
                return header + "\n" + Seperator + cpp;
            }

            private string GenCpp(int offset)
            {
                string cpp = "#" + CKeywords.Include + "\"" + Name + ".h\"";

                foreach (Var variable in GlobalVariables)
                    cpp += variable.ConvertToCCode(offset);

                cpp += "\n";

                foreach (Func function in Functions)
                    cpp += function.ConvertToCCode(offset);
                return cpp;
            }

            private string GenHeader()
            {
                string header = "#pragma once";

                Func<string, string> includeMod = s => "#" + CKeywords.Include + "\"" + s + ".h\"";
                header += AdditionalIncludes.Concat(includeMod, "\n");

                foreach (Var variable in GlobalVariables)
                    header += variable.GetForwardDeclaration();

                header += "\n";

                foreach (Func func in Functions)
                    header += func.GetForwardDeclaration();
                return header;
            }
        }

        public class Block : CStatement
        {
            public readonly List<CStatement> Statements;

            public Block(List<CStatement> statements)
            {
                Statements = statements;
            }

            public override string ConvertToCCode(int offset)
            {
                string code = StringUtils.GenIndent(offset) + "{\n";
                foreach (CStatement statement in Statements)
                    code += statement.ConvertToCCode(offset + 1);
                code += StringUtils.GenIndent(offset) + "}\n";
                return code;
            }
        }

        public class Return : CStatement
        {
            public readonly CExpression Expression;

            public Return(CExpression expression)
            {
                Expression = expression;
            }

            public override string ConvertToCCode(int offset)
            {
                return StringUtils.GenIndent(offset) + 
                       CKeywords.Return + " " + 
                       Expression.ConvertToString() + ";\n";
            }
        }

        public class For : CStatement
        {
            public readonly Var Initalizer;
            public readonly CExpression Condition;
            public readonly CExpression Iterator;
            public readonly CStatement Body;

            public For(Var initalizer, CExpression condition, CExpression iterator, CStatement body)
            {
                Initalizer = initalizer;
                Condition = condition;
                Iterator = iterator;
                this.Body = body;
            }

            public override string ConvertToCCode(int offset)
            {
                string code = StringUtils.GenIndent(offset);

                code += CKeywords.For + "(";
                if(Initalizer != null)
                {
                    code += Initalizer.ConvertToCCode(0);
                    // removes the \n charactor for the variable declaration
                    code = code.Remove(code.Length - 1);
                }
                else
                {
                    code += ";";
                }

                if(Condition != null)
                    code += " " + Condition.ConvertToString();
                code += ";";

                if(Iterator != null)
                    code += " " + Iterator.ConvertToString();
                code += ")\n";


                if (Body is Block b)
                    code += b.ConvertToCCode(offset);
                else
                    code += Body.ConvertToCCode(offset + 1);

                return code;
            }
        }

        public class If : CStatement
        {
            public readonly CExpression Condition;
            public readonly CStatement Body;

            public If(CExpression condition, CStatement body)
            {
                Condition = condition;
                Body = body;
            }

            public override string ConvertToCCode(int offset)
            {
                string code = StringUtils.GenIndent(offset);
                code += CKeywords.If + "(";
                code += Condition.ConvertToString() + ")\n";

                if (Body is Block b)
                    code += b.ConvertToCCode(offset);
                else
                    code += Body.ConvertToCCode(offset + 1);

                return code;
            }
        }

        public class Var : CStatement
        {
            public readonly string TypeName;
            public readonly List<string> VarNames;
            public readonly CExpression Initializer;

            public Var(string typeName, List<string> varNames, CExpression initializer)
            {
                TypeName = typeName;
                VarNames = varNames;
                Initializer = initializer;
            }

            public string GetForwardDeclaration()
            {
                string code = CKeywords.Extern + " " + TypeName + " ";
                for(int i = 0; i < VarNames.Count; i++)
                {
                    if (i != 0)
                        code += ", ";
                    code += VarNames[i];
                }

                code += ";\n";
                return code;
            }

            public override string ConvertToCCode(int offset)
            {
                string code = StringUtils.GenIndent(offset);
                code += TypeName + " ";
                for(int i = 0; i < VarNames.Count; i++)
                {
                    if (i != 0)
                        code += ", ";

                    code += VarNames[i];
                }

                code += " = ";
                code += Initializer.ConvertToString();
                code += ";\n";
                return code;
            }
        }

        public class Func : CStatement
        {
            public readonly string ReturnType;
            public readonly string Name;
            public readonly List<(string, string)> Parameters;
            public readonly Block Body;

            public Func(string returnType, string name, List<(string, string)> parameters, Block body)
            {
                ReturnType = returnType;
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public string GetForwardDeclaration()
            {
                string code = ReturnType + " " + Name + "(";

                for (int i = 0; i < Parameters.Count; i++)
                {
                    if (i != 0)
                        code += ", ";
                    code += Parameters[i].Item1 + " " + Parameters[i].Item2;
                }
                code += ");\n";

                return code;
            }

            public override string ConvertToCCode(int offset)
            {
                string code = StringUtils.GenIndent(offset) + 
                              ReturnType + " " + Name + "(";

                for(int i = 0; i < Parameters.Count; i++)
                {
                    if (i != 0)
                        code += ", ";
                    code += Parameters[i].Item1 + " " + Parameters[i].Item2;
                }
                code += ")\n";
                code += Body.ConvertToCCode(offset);

                return code;
            }
        }

        public class ExprStmt : CStatement
        {
            public readonly CExpression Expression;

            public ExprStmt(CExpression expression)
            {
                Expression = expression;
            }

            public override string ConvertToCCode(int offset)
            {
                return StringUtils.GenIndent(offset) + 
                       Expression.ConvertToString() + ";\n";
            }
        }
    }
}
