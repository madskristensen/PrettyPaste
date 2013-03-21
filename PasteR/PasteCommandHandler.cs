using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Windows.Forms;

namespace PasteR
{
    class PasteCommandHandler : IOleCommandTarget
    {
        private const uint _commandId = 26; // The Paste command
        private ITextView _textView;
        private IOleCommandTarget _nextCommandTarget;        

        public PasteCommandHandler(IVsTextView adapter, ITextView textView)
        {
            this._textView = textView;
            adapter.AddCommandFilter(this, out _nextCommandTarget);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID && nCmdID == _commandId)
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText(TextDataFormat.Text);

                    if (PasteCleaner.IsDirty(text))
                    {
                        string clean = PasteCleaner.Clean(text);
                        Clipboard.SetText(clean);
                    }
                }
            }

            return _nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                for (int i = 0; i < cCmds; i++)
                {
                    if (prgCmds[i].cmdID == _commandId)
                    {
                        prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                        return VSConstants.S_OK;
                    }
                }
            }

            return _nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
