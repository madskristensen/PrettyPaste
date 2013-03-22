using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PasteR
{
    public class PasteCleaner
    {
        private bool _hasExtraLineBreaks, _hasLineNumbers, _hasOrphanedLineNumbers;
        private const string _regex = @"^([\d]+)(\s|\.)?";
        private string _text;

        public PasteCleaner(string text)
        {
            _text = text;
        }

        public bool IsDirty()
        {
            string[] lines = _text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            // No reason to mark less than 3 lines of code dirty
            if (lines.Length < 2)
                return false;
            
            float emptyCounter = 0;
            float orphanCounter = 0;
            float numberCounter = 0;
            Regex regex = new Regex(_regex);

            bool prevIsEmpty = false;
            int parsed = 0;

            for (int i = 0; i < Math.Min(lines.Length, 25); i++)
            {
                bool isEmpty = lines[i].Length == 0;
                bool isOrphaned = int.TryParse(lines[i], out parsed);
                bool isNumber = !isEmpty && regex.IsMatch(lines[i]);

                if (isEmpty && !prevIsEmpty)
                    emptyCounter += 1;

                if (isOrphaned)
                    orphanCounter += 1;

                if (isNumber)
                    numberCounter += 1;

                prevIsEmpty = isEmpty;
            }

            _hasExtraLineBreaks = (float)lines.Length / emptyCounter < 2.4F;
            _hasOrphanedLineNumbers = (float)lines.Length / orphanCounter < 2.4F;
            _hasLineNumbers = (lines.Length - emptyCounter) / numberCounter < 2.2F;

            return _hasExtraLineBreaks || _hasOrphanedLineNumbers || _hasLineNumbers;
        }

        public string Clean()
        {
            string[] lines = _text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<string> cleanLines = new List<string>(lines);

            if (_hasOrphanedLineNumbers)
            {
                int parsed = 0;

                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    if (int.TryParse(lines[i], out parsed))
                    {
                        cleanLines.RemoveAt(i);
                    }
                }
            }

            string result = string.Join(Environment.NewLine, cleanLines);

            if (_hasLineNumbers)
                result = CleanLineNumbers(result);

            return result;
        }

        private string CleanLineNumbers(string result)
        {
            Regex regex = new Regex(_regex, RegexOptions.Multiline);

            return regex.Replace(result, string.Empty);
        }
    }
}