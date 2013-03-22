using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PasteR
{
    internal class PasteCommandHandler : IOleCommandTarget
    {
        private readonly Guid _guid = VSConstants.GUID_VSStandardCommandSet97; // The command category
        private readonly uint _commandId = 26; // The Paste command in the command category

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
            if (pguidCmdGroup == _guid && nCmdID == _commandId)
            {
                if (Clipboard.ContainsText())
                {   
                    TextDocument doc = (TextDocument)_dte.ActiveDocument.Object("TextDocument");
                    EditPoint start = doc.Selection.ActivePoint.CreateEditPoint();
                    
                    // Run the PasteR code after the default paste, so undo restores default
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        ReplaceText(doc, start, _textView.Caret.Position.BufferPosition);
                    }), DispatcherPriority.Normal, null);
                }
            }

            return _nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private void ReplaceText(TextDocument doc, EditPoint start, int length)
        {
            string text = Clipboard.GetText(TextDataFormat.Text);
            PasteCleaner cleaner = new PasteCleaner(text);

            if (cleaner.IsDirty())
            {
                string clean = cleaner.Clean();

                _dte.UndoContext.Open("Paste FixR");

                // Insert
                var edit = doc.CreateEditPoint(start);
                edit.ReplaceText(length, clean, 0);
                
                // Format
                doc.Selection.MoveToPoint(edit, true);
                FormatSelection();
                _textView.Selection.Clear();

                _dte.UndoContext.Close();
            }
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