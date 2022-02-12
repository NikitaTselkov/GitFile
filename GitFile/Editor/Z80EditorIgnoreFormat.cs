using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFile.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80ignore")]
    [Name("z80ignore")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorIgnoreFormat : ClassificationFormatDefinition
    {
        public Z80EditorIgnoreFormat()
        {
            this.DisplayName = "Z80 Assembly Ignore";
            this.ForegroundColor = Colors.Crimson;
        }
    }
}
