using System;

namespace PasteR
{
    public class PasteCleaner
    {
        public static bool IsDirty(string content)
        {
            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            
            int counter = 0;
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
                if (i > 50 && counter < 10)
                {
                    break;
                }
            }

            return (float)lines.Length / counter >= 2;
        }

        public static string Clean(string content)
        {
            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(Environment.NewLine, lines);
        }
    }
}
