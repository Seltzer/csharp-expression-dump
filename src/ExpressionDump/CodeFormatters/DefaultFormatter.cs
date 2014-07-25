using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionDump.CodeFormatters
{
    
    class DefaultFormatter : ICodeFormatter
    {
        public string TypeToString(Type type)
        {
            return type.ToString();
        }


        public string CommaSeparatorToString()
        {
            return ", ";
        }
    }
}
