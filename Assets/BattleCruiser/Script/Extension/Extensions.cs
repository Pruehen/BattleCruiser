using System.Collections;
using System.Collections.Generic;

public static class Extensions
{
    /// <summary>
    /// XXX_001 �� ���� ������ ���ڿ����� ���κ��� ���� - 1�� ��ȯ.
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
