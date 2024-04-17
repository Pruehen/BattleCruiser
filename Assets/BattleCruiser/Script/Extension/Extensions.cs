using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static Vector2 GenerateDistanceKeepVector(this Vector2 myPos, Vector2 targetPos, float distance)
    {
        if (distance < 50)
            distance = 50;

        Vector2 toTargetDir = (targetPos - myPos).normalized;        
        Vector2 movePosition = targetPos - (toTargetDir * distance);

        while (movePosition.y <= 50)
        {
            // ȸ���� ���� ���� (����� ��� �ݽð� ����, ������ ��� �ð� ����)
            float angle = (toTargetDir.x > 0) ? -15f : 15f;

            // ���ʹϾ����� ȸ��
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            toTargetDir = rotation * toTargetDir;

            // ���ο� ��ǥ ��ġ �ٽ� ���
            movePosition = (Vector2)targetPos - (toTargetDir * distance);
        }

        //Debug.Log("�Ÿ� ���� ���");
        return movePosition;
    }
}
