using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFile.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80method")]
    [Name("z80method")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorMethodFormat : ClassificationFormatDefinition
    {
        public Z80EditorMethodFormat()
        {
            this.DisplayName = "Z80 Assembly Method";
            this.ForegroundColor = Color.FromRgb(216, 214, 142);
        }
    }
}
