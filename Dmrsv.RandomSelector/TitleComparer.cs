namespace Dmrsv.RandomSelector
{
    public class TitleComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (string.Equals(x, y))
            {
                return 0;
            }
            if (y == null)
            {
                return -1;
            }
            if (x == null)
            {
                return 1;
            }
            // Djmax sorts titles with case-insensitive and ignoring the characters below
            x = CleanString(x);
            y = CleanString(y);
            int index = x.Zip(y, (a, b) => a == b).TakeWhile(equals => equals).Count();
            if (index == Math.Min(x.Length, y.Length))
            {
                return x.Length - y.Length;
            }
            // priority order: white-space -> non-alphabetic letter -> special character -> number -> alphabet(Normalize diacritics)
            char a = x[index], b = y[index];
            int priorityA = GetPriority(a, index);
            int priorityB = GetPriority(b, index);
            if (priorityA == priorityB)
            {
                return a.CompareTo(b);
            }
            return priorityA - priorityB;
        }

        private string CleanString(string text)
        {
            // Djmax sorts titles with case-insensitive and ignoring the characters below
            string s = text.Replace("'", string.Empty).Replace("-", string.Empty).ToUpper();

            // Djmax treats umlauts as their base alphabets for sorting
            s = s.Replace("Ö", "O")
                 .Replace("Ä", "A")
                 .Replace("Ü", "U")
                 .Replace("È", "E")
                 .Replace("É", "E");
            
            return s;
        }

        private int GetPriority(char ch, int idx)
        {
            if (char.IsWhiteSpace(ch))
            {
                return 0;
            }
            if (char.IsUpper(ch)) // alphabet
            {
                return 4;
            }
            if (char.IsLetter(ch)) // non-alphabetic letter
            {
                return idx == 0 ? 1 : 5;
            }
            if (char.IsDigit(ch))
            {
                return 3;
            }
            // symbol, punctuation, etc.
            return 2;
        }
    }
}
