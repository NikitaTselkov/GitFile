using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFileShared.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80variable")]
    [Name("z80variable")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80EditorVariableFormat : ClassificationFormatDefinition
    {
        public Z80EditorVariableFormat()
        {
            this.DisplayName = "Z80 Assembly Variable";
            this.ForegroundColor = Colors.LightBlue;
        }
    }
}
