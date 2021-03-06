using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace GitFileShared.Editor
{
    /// <summary>
    /// Defines an editor format for the GitEditorClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "GitEditorClassifier")]
    [Name("GitEditorClassifier")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class GitEditorClassifierFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitEditorClassifierFormat"/> class.
        /// </summary>
        public GitEditorClassifierFormat()
        {
            this.DisplayName = "GitEditorClassifier";
            this.ForegroundColor = Color.FromRgb(150, 158, 174);
        }
    }
}
