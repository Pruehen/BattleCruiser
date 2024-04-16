using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    /// <summary>
    /// XXX_001 과 같은 형식의 문자열에서 끝부분의 숫자 - 1을 반환.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int Index(this string key)
    {
        int index = int.Parse(key.Split('_')[1]) - 1;
        return index;
    }
    public static string ShipKey(this int index)
    {
        string shipKey = $"Ship_{(index+1).ToString("D3")}";
        return shipKey;
    }
}
