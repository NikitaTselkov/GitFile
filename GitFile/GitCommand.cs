using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace GitFile
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GitCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int StartAllGitFilesCommandId = 0x0100;
        public const int StartGitFilesInProjectCommandId = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid SolutionCommandSet = new Guid("3025a1db-e889-4caa-ad24-006760fbb4f9");
        public static readonly Guid ProjectCommandSet = new Guid("3025a1db-e899-3caa-ad24-006760fbb4f8");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly DTE dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GitCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuSolutionCommandID = new CommandID(SolutionCommandSet, StartAllGitFilesCommandId);
            var menuSolutionItem = new MenuCommand(this.StartAllGitFiles, menuSolutionCommandID);
            commandService.AddCommand(menuSolutionItem);

            var menuProjectCommandID = new CommandID(ProjectCommandSet, StartGitFilesInProjectCommandId);
            var menuProjectItem = new MenuCommand(this.StartGitFilesInProject, menuProjectCommandID);
            commandService.AddCommand(menuProjectItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GitCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GitCommand(package, commandService);
        }

        private void StartAllGitFiles(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var path = Path.GetDirectoryName(dte.Solution.FullName);

            foreach (var filePath in Directory.EnumerateFiles(path, "*.git", SearchOption.AllDirectories))
            {
                GitCompiler.StartGitFile(filePath);
            }
        }

        private void StartGitFilesInProject(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = GetSelectedSolutionExplorerItem()?.Object as Project;
            var path = Path.GetDirectoryName(project.FullName);

            foreach (var filePath in Directory.EnumerateFiles(path, "*.git", SearchOption.AllDirectories))
            {
                GitCompiler.StartGitFile(filePath);
            }
        }

        private UIHierarchyItem GetSelectedSolutionExplorerItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            UIHierarchy solutionExplorer = ((DTE2)dte).ToolWindows.SolutionExplorer;
            object[] items = solutionExplorer.SelectedItems as object[];
            if (items.Length != 1)
                return null;

            return items[0] as UIHierarchyItem;
        }
    }
}
