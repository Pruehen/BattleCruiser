using System.Collections;
using System.Collections.Generic;

public static class StringExtensions
{
    /// <summary>
    /// XXX_001 �� ���� ������ ���ڿ����� ���κ��� ���� - 1�� ��ȯ.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int Index(this string str)
    {
        int index = int.Parse(str.Split('_')[1]) - 1;
        return index;
    }
}
