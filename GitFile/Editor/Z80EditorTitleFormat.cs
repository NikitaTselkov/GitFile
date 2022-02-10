using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.ForegroundColor = Colors.Orange;
        }
    }
}
