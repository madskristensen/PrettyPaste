using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Windows.Forms;

namespace PrettyPaste
{
    internal class PasteCommandHandler : IOleCommandTarget
    {
        private readonly Guid _guid = VSConstants.GUID_VSStandardCommandSet97; // The VSConstants.VSStd97CmdID enumeration
        private readonly uint _commandId = (uint)VSConstants.VSStd97CmdID.Paste; // The paste command in the above enumeration

        private ITextView _textView;
        private IOleCommandTarget _nextCommandTarget;
        private DTE2 _dte;

        public PasteCommandHandler(IVsTextView adapter, ITextView textView, DTE2 dte)
        {
            _textView = textView;
            _dte = dte;
            adapter.AddCommandFilter(this, out _nextCommandTarget);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == _guid && nCmdID == _commandId && Clipboard.ContainsText())
            {
                string text = Clipboard.GetText(TextDataFormat.Text);
                PasteCleaner cleaner = new PasteCleaner(text);

                if (cleaner.IsDirty())
                {
                    TextDocument doc = (TextDocument)_dte.ActiveDocument.Object("TextDocument");
                    EditPoint start = doc.Selection.TopPoint.CreateEditPoint();

                    // First insert plain text
                    _dte.UndoContext.Open("Paste");
                    doc.Selection.Insert(text);
                    _dte.UndoContext.Close();

                    // Then replace with clean text, so undo restores the default behavior
                    ReplaceText(cleaner, doc, start, text);

                    return VSConstants.S_OK;
                }
            }

            return _nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private void ReplaceText(PasteCleaner cleaner, TextDocument doc, EditPoint start, string text)
        {
            string clean = cleaner.Clean();
            _dte.UndoContext.Open("Paste FixR");

            // Insert
            start.ReplaceText(text.Replace("\n", string.Empty).Length, clean, 0);
            //start.Insert(clean);

            // Format
            doc.Selection.MoveToPoint(start, true);
            FormatSelection();
            _textView.Selection.Clear();

            _dte.UndoContext.Close();
        }

        private void FormatSelection()
        {
            Command command = _dte.Commands.Item("Edit.FormatSelection");

            if (command.IsAvailable)
            {
                _dte.ExecuteCommand("Edit.FormatSelection");
            }
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == _guid)
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