using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFile.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80title")]
    [Name("z80title")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorTitleFormat : ClassificationFormatDefinition
    {
        public Z80EditorTitleFormat()
        {
            this.DisplayName = "Z80 Assembly Title";
            this.IsBold = true;
            this.ForegroundColor = Color.FromRgb(209, 153, 93);
        }
    }
}
