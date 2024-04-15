using System.Collections;
using System.Collections.Generic;

public static class StringExtensions
{
    public static int Index(this string str)
    {
        int index = int.Parse(str.Split('_')[1]) - 1;
        return index;
    }
}
