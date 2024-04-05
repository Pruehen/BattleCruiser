using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerUI : Singleton<PlayerUI>
{
    public RectTransform aimPoint;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI altText;

    public void SetAimPointPosition(Vector2 screenPosition)
    {
        aimPoint.position = screenPosition;
    }

    public void SetSpeedText(float value)
    {
        speedText.text = ((int)value).ToString();
    }

    public void SetAltText(float value)
    {
        altText.text = ((int)value).ToString();
    }
}
