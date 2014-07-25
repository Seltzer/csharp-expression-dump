using ExpressionDump.CodeWriterFormatters;
using ExpressionDump.CSharpStyles;

namespace ExpressionDump.CodeWriterConfig
{
    
    public class CodeWriterConfig
    {
        public readonly ICodeFormatter CodeFormatter;
        public readonly CSharpStyle Style;


        public CodeWriterConfig(ICodeFormatter codeFormatter = null, CSharpStyle style = null)
        {
            CodeFormatter = codeFormatter ?? new DefaultCodeFormatter();
            Style = style ?? new CSharpStyle();
        }
    }

}
