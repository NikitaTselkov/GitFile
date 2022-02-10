using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitFile.Editor
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "GitEditorClassifier" classification type.
    /// </summary>
    internal class GitEditorClassifier : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType classificationType;

        private readonly IClassificationTypeRegistryService registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitEditorClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal GitEditorClassifier(IClassificationTypeRegistryService registry)
        {
            this.registry = registry;
            this.classificationType = registry.GetClassificationType("GitEditorClassifier");
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();

            if (span.Length == 0) return result;

            ITextSnapshot snapshot = span.Snapshot;
            ITextSnapshotLine line = snapshot.GetLineFromPosition(span.Start.Position);
            ITextSnapshotLine endLine = snapshot.GetLineFromPosition(span.End.Position);

            try
            {
                while (true)
                {
                    // Process current line
                    ProcessLine(line, ref result);

                    if (line.LineNumber == endLine.LineNumber)
                        break;
                    // Next line
                    line = snapshot.GetLineFromPosition(line.EndIncludingLineBreak + 1);
                }
            }
            catch (Exception) { }

            return result;
        }

        private void ProcessLine(ITextSnapshotLine line, ref List<ClassificationSpan> result)
        {
            var text = line.GetText().Trim();

            // Title
            if (text.FirstOrDefault() == '.')
            {
                IClassificationType titleClassifType = registry.GetClassificationType("z80title");
                result.Add(new ClassificationSpan(
                  new SnapshotSpan(line.Snapshot, new Span(line.Start, text.Length)),
                  titleClassifType));
            }
        }

        #endregion
    }
}
