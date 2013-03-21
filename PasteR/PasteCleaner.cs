using System;

namespace PasteR
{
    public class PasteCleaner
    {
        public static bool IsDirty(string content)
        {
            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            // No reason to mark less than 3 lines of code dirty
            if (lines.Length < 2)
            {
                return false;
            }
            
            float counter = 0;
            bool prevIsEmpty = false;

            for (int i = 0; i < lines.Length; i++)
            {
                bool isEmpty = lines[i].Length == 0;

                if (isEmpty && !prevIsEmpty)
                {
                    counter += 1;
                }

                prevIsEmpty = isEmpty;

                // Let's break the loop early if the content isn't dirty
                if (i > 50 && counter < 20)
                {
                    break;
                }
            }

            return (float)lines.Length / counter <= 2.5F; // Minimum every other line must be empty
        }

        public static string Clean(string content)
        {
            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            
            // Add a trailing newline to mimick the VS default paste behavior.
            string result = string.Join(Environment.NewLine, lines) + Environment.NewLine;
            
            return result;
        }
    }
}
