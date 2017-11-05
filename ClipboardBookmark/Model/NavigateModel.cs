using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardBookmark.Model
{
    class NavigateModel
    {
        public void OpenFileAndGotoLine(string fileName, string stringLine)
        {
            IVsUIShellOpenDocument openDoc = Package.GetGlobalService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            IVsWindowFrame frame;
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp;
            IVsUIHierarchy hier;
            uint itemid;
            Guid logicalView = VSConstants.LOGVIEWID_Code;
            if (ErrorHandler.Failed(openDoc.OpenDocumentViaProject(fileName, ref logicalView, out sp, out hier, out itemid, out frame))
                  || frame == null)
            {
                return;
            }
            object docData;
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData);
            VsTextBuffer buffer = docData as VsTextBuffer;
            if (buffer == null)
            {
                IVsTextBufferProvider bufferProvider = docData as IVsTextBufferProvider;
                if (bufferProvider != null)
                {
                    IVsTextLines lines;
                    ErrorHandler.ThrowOnFailure(bufferProvider.GetTextBuffer(out lines));
                    buffer = lines as VsTextBuffer;
                    Debug.Assert(buffer != null, "IVsTextLines does not implement IVsTextBuffer");
                    if (buffer == null)
                    {
                        return;
                    }
                }
            }
            IVsTextManager mgr = Package.GetGlobalService(typeof(VsTextManagerClass)) as IVsTextManager;
            int line = 0;
            Int32.TryParse(stringLine, out line);
            mgr.NavigateToLineAndColumn(buffer, ref logicalView, line, 0, line, 0);
        }
    }
}
