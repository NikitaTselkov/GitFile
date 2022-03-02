using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            catch (Exception ex) { }

            return result;
        }

        private void ProcessLine(ITextSnapshotLine line, ref List<ClassificationSpan> result)
        {
            var text = line.GetText();

            SetClassificationType("GitEditorClassifier",
                    new SnapshotSpan(line.Snapshot, new Span(line.Start, text.Length)), ref result);

            // Title
            if (text.Trim().FirstOrDefault() == '.')
            {
                SetClassificationType("z80title",
                    new SnapshotSpan(line.Snapshot, new Span(line.Start, text.Length)), ref result);
            }
            // Comment
            if (GitCompilerChecks.IsContainsComment(text))
            {
                var match = Regex.Match(text, @"<!(.*?)>$");
                SetClassificationType("z80comment",
                       new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(match.Value), match.Value.Length)), ref result);
            }
            // Variable
            if (GitCompilerChecks.IsContainsVariable(text))
            {
                foreach (Match match in Regex.Matches(text, @":(.*?)\s*="))
                {
                    var word = match.Value;

                    if (word.Contains(":else"))
                        word = word.Replace(":else", "");

                    if (!word.Contains(":if"))
                    {
                        SetClassificationType("z80variable",
                           new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(word), word.Length - 1)), ref result);

                        GitCompilerChecks.SetVariable(new string(word.Skip(1).TakeWhile(t => t != ' ').ToArray()));
                    }
                }
            }
            // Variable in command
            if (GitCompilerChecks.IsContainsVariablesInCommand(text))
            {
                int index = 0;
                foreach (Match match in Regex.Matches(text, @"\{([^}]*)\}"))
                {
                    SetClassificationType("z80variable",
                          new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(match.Value, index), match.Value.Length)), ref result);

                    index = text.IndexOf(match.Value, index) + match.Value.Length;
                }
            }
            // Variable in "if"
            if (GitCompilerChecks.IsExistsVariables(text, out (string first, string second) variables))
            {
                int index = 0;

                if (variables.first != string.Empty)
                {
                    SetClassificationType("z80variable",
                               new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(variables.first), variables.first.Length)), ref result);

                    index += text.IndexOf(variables.first) + variables.first.Length;
                }

                if (variables.second != string.Empty)
                    SetClassificationType("z80variable",
                               new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(variables.second, index), variables.second.Length)), ref result);
            }
            // Ignore
            if (GitCompilerChecks.IsNeedIgnoreOutput(text))
            {
                var match = Regex.Match(text, @"-->\s*Ignore");
                SetClassificationType("z80ignore",
                              new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(match.Value), match.Value.Length)), ref result);
            }
            // Method
            if (GitCompilerChecks.IsContainsMethod(text, out string methodName))
            {
                SetClassificationType("z80method",
                              new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(methodName), methodName.Length)), ref result);
            }
            // Range
            if (GitCompilerChecks.IsContainsRange(text))
            {
                var match = Regex.Match(text, @"=>\s*\((.*?)\)");
                SetClassificationType("z80range",
                       new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(match.Value), match.Value.Length)), ref result);
            }
            // if
            if (text.Contains(":if"))
            {
                var word = ":if";
                SetClassificationType("z80conditionalOperator",
                       new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(word), word.Length)), ref result);
            }
            // else
            else if (text.Contains(":else"))
            {
                var word = ":else";
                SetClassificationType("z80conditionalOperator",
                       new SnapshotSpan(line.Snapshot, new Span(line.Start + text.IndexOf(word), word.Length)), ref result);
            }
        }

        private void SetClassificationType(string classificationTypeName, SnapshotSpan snapshotSpan, ref List<ClassificationSpan> result)
        {
            IClassificationType titleClassifType = registry.GetClassificationType(classificationTypeName);
            result.Add(new ClassificationSpan(snapshotSpan, titleClassifType));
        }

        #endregion
    }
}
