using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFile.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80conditionalOperator")]
    [Name("z80conditionalOperator")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorConditionalOperatorFormat : ClassificationFormatDefinition
    {
        public Z80EditorConditionalOperatorFormat()
        {
            this.DisplayName = "Z80 Assembly Conditional Operator";
            this.ForegroundColor = Color.FromRgb(192, 108, 202);
        }
    }
}
