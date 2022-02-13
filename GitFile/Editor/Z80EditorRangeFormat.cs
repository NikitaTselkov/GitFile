using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFile.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80range")]
    [Name("z80range")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorRangeFormat : ClassificationFormatDefinition
    {
        public Z80EditorRangeFormat()
        {
            this.DisplayName = "Z80 Assembly Range";
            this.ForegroundColor = Color.FromRgb(216, 214, 142);
        }
    }
}
