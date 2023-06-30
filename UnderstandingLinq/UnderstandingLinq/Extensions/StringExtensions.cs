namespace UnderstandingLinq.Extensions;

// 1. Class must be static (which forces all methods to be static)
public static class StringExtensions
{
    // 2. First parameter with "this" keyword 
    public static string FirstLetterToUpper(this string str)
    {
        if (str.Length == 0)
            return str;

        var characters = str.ToCharArray();
        characters[0] = char.ToUpper(characters[0]);
        return new string(characters);
    }

    // 3. Extension methods can have additional parameters.
    public static string LetterAtIndexToUpper(this string str, int idx)
    {
        var characters = str.ToCharArray();
        if (idx >= characters.Length)
            return str;
        
        characters[idx] = char.ToUpper(characters[idx]);
        return new string(characters);
    }
}