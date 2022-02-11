using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFile.Editor
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "z80Variable")]
    [Name("z80Variable")]
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
