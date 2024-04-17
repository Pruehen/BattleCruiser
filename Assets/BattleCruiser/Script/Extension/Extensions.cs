using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static Vector2 GenerateDistanceKeepVector(this Vector2 myPos, Vector2 targetPos, float distance)
    {
        if (distance < 50)
            distance = 50;

        Vector2 toTargetDir = (targetPos - myPos).normalized;        
        Vector2 movePosition = targetPos - (toTargetDir * distance);

        while (movePosition.y <= 50)
        {
            // 회전할 각도 설정 (양수일 경우 반시계 방향, 음수일 경우 시계 방향)
            float angle = (toTargetDir.x > 0) ? -15f : 15f;

            // 쿼터니언으로 회전
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            toTargetDir = rotation * toTargetDir;

            // 새로운 목표 위치 다시 계산
            movePosition = (Vector2)targetPos - (toTargetDir * distance);
        }

        //Debug.Log("거리 유지 명령");
        return movePosition;
    }
}
