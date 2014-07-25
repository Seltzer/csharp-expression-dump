using System;

namespace ExpressionDump.CodeWriterFormatters
{
    
    public interface ICodeFormatter
    {
        string TypeToString(Type type);

        string CommaSeparatorToString();

        string OperatorToString(string operatorStr);
    }

}
