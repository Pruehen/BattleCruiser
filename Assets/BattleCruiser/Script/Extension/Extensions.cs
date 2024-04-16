using System.Collections;
using System.Collections.Generic;

public static class StringExtensions
{
    /// <summary>
    /// XXX_001 과 같은 형식의 문자열에서 끝부분의 숫자 - 1을 반환.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int Index(this string str)
    {
        int index = int.Parse(str.Split('_')[1]) - 1;
        return index;
    }
}
