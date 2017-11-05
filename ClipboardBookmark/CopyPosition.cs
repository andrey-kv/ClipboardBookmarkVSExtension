using ClipboardBookmark.Model;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace ClipboardBookmark
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CopyPosition
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CopyCommandId = 0x0100;
        public const int PasteCommandId = 0x0200;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("ba59338a-1034-4028-a911-e93e59516768");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;
        private NavigateModel navigateModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyPosition"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CopyPosition(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCopyCommandID = new CommandID(CommandSet, CopyCommandId);
                var menuItemCopy = new MenuCommand(this.MenuItemCopyCallback, menuCopyCommandID);
                commandService.AddCommand(menuItemCopy);

                var menuPasteCommandID = new CommandID(CommandSet, PasteCommandId);
                var menuItemPaste = new MenuCommand(this.MenuItemPasteCallback, menuPasteCommandID);
                commandService.AddCommand(menuItemPaste);
            }

            this.navigateModel = new NavigateModel();
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CopyPosition Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
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
        public static void Initialize(Package package)
        {
            Instance = new CopyPosition(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCopyCallback(object sender, EventArgs e)
        {
            Document activeDoc = ((CopyPositionPackage)package).dte.ActiveDocument;

            if (activeDoc != null)
            {
                TextDocument textDoc = (TextDocument)activeDoc.Object("TextDocument");
                if (textDoc != null)
                {
                    var bufferModel = new BufferModel();
                    bufferModel.bufferBody = activeDoc.FullName;
                    bufferModel.topPosition = textDoc.Selection.TopLine;
                    bufferModel.bottomPosition = textDoc.Selection.BottomLine;
                    bufferModel.BufferToClipboard();
                }
            }
        }

        private void MenuItemPasteCallback(object sender, EventArgs e)
        {
            var bufferModel = new BufferModel();
            if (bufferModel.ClipboardToBuffer())
            {
                navigateModel.OpenFileAndGotoLine(bufferModel);
            }
            else
            {
                string title = "Paste Position";
                string message = "Position could not be extracted from clipboard.";

                VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}
