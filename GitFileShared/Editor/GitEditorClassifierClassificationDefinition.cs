using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GitFileShared.Editor
{
    /// <summary>
    /// Classification type definition export for GitEditorClassifier
    /// </summary>
    internal static class GitEditorClassifierClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        [Export]
        [Name("GitEditorClassifier")]
        [BaseDefinition("code")]
        [BaseDefinition("projection")]
        internal static ContentTypeDefinition gitContentTypeDefinition;

        [Export]
        [FileExtension(".git")]
        [ContentType("GitEditorClassifier")]
        internal static FileExtensionToContentTypeDefinition gitFileExtensionDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80title")]
        internal static ClassificationTypeDefinition Z80TitleDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80comment")]
        internal static ClassificationTypeDefinition Z80CommentDefinition;
        
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80conditionalOperator")]
        internal static ClassificationTypeDefinition Z80ConditionalOperatorDefinition;
        
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80variable")]
        internal static ClassificationTypeDefinition Z80VariableDefinition;
        
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80ignore")]
        internal static ClassificationTypeDefinition Z80IgnoreDefinition;
        
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80method")]
        internal static ClassificationTypeDefinition Z80MethodDefinition;
        
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("z80range")]
        internal static ClassificationTypeDefinition Z80RangeDefinition;

        /// <summary>
        /// Defines the "GitEditorClassifier" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("GitEditorClassifier")]
        private static ClassificationTypeDefinition typeDefinition;

#pragma warning restore 169
    }
}
