using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFileShared.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80comment")]
    [Name("z80comment")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorCommentFormat : ClassificationFormatDefinition
    {
        public Z80EditorCommentFormat()
        {
            this.DisplayName = "Z80 Assembly Comment";
            this.ForegroundColor = Color.FromRgb(145, 187, 112);
        }
    }
}
