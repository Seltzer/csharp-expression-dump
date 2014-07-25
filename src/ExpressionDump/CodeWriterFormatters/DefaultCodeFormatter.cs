using System;
using ExpressionDump.Core;
using ExpressionDump.CSharpStyles;

namespace ExpressionDump.CodeWriterFormatters
{
    
    public class DefaultCodeFormatter : ICodeFormatter
    {
        readonly CSharpStyle Style;


        internal DefaultCodeFormatter(CSharpStyle style = null)
        {
            Style = style ?? new CSharpStyle();
        }


        public string TypeToString(Type type)
        {
            return type.Pipe(TypeExt.GetTypeAliasOrSelf);
        }


        public string CommaSeparatorToString()
        {
            return "," + SpaceOrEmpty(Style.InsertSpaceAfterComma);
        }


        public string OperatorToString(string operatorStr)
        {
            return SurroundWithSpaces(operatorStr, Style.SurroundOperatorsWithSpace);
        }


        string SpaceOrEmpty(bool insertSpace)
        {
            return insertSpace ? " " : "";
        }


        string SurroundWithSpaces(string str, bool insertSpace)
        {
            return SpaceOrEmpty(insertSpace) + str + SpaceOrEmpty(insertSpace);
        }
    }

}
