using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : Singleton<PlayerUI>
{
    public RectTransform aimPoint;

    public void SetAimPointPosition(Vector2 screenPosition)
    {
        aimPoint.position = screenPosition;
    }
}
