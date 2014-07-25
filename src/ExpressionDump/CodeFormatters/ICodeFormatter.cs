using System;

namespace ExpressionDump.CodeFormatters
{
    
    interface ICodeFormatter
    {
        string TypeToString(Type type);

        string CommaSeparatorToString();
    }

}
