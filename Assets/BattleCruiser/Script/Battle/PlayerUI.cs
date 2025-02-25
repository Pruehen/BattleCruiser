using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerUI : SceneSingleton<PlayerUI>
{
    public RectTransform aimPoint;
    public TextMeshProUGUI aimDistanceText;

    public RectTransform lockOnPoint;
    public TextMeshProUGUI lockOnDistanceText;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI altText;

    public RectTransform velocityMarker;
    public RectTransform moveOrderMarker;
    float markerOrbitRadius = 250;

    public Image hpBar;

    public void SetAimPointPosition(Vector2 screenPosition)
    {
        aimPoint.position = screenPosition;
    }
    public void SetAimDistanceText(float distance)
    {
        aimDistanceText.text = ((int)(distance * 10)).ToString();
    }
    public void SetLockOnPointPosition(Vector2 worldPosition)
    {
        lockOnPoint.gameObject.SetActive(true);
        lockOnPoint.position = Camera.main.WorldToScreenPoint(worldPosition);
    }
    public void DisableLockOnPoint()
    {
        lockOnPoint.gameObject.SetActive(false);
    }
    public void SetLockOnText(float distance, string name)
    {
        lockOnDistanceText.text = string.Format($"{distance * 10:N0}\n-{name}-");
    }


    public void SetSpeedText(float value)
    {
        speedText.text = ((int)(value * 10)).ToString();
    }

    public void SetAltText(float value)
    {
        altText.text = ((int)(value * 10)).ToString();
    }

    public void SetVelocityMarker(Vector2 velocity, Vector2 playerPosition)
    {
        if(velocity.magnitude < 0.5f)
        {
            velocityMarker.gameObject.SetActive(false);
        }
        else
        {
            velocityMarker.gameObject.SetActive(true);
            Vector2 normalizedVelocity = velocity.normalized;
            velocityMarker.position = normalizedVelocity * markerOrbitRadius + (Vector2)Camera.main.WorldToScreenPoint(playerPosition);

            float angle = Mathf.Atan2(normalizedVelocity.y, normalizedVelocity.x) * Mathf.Rad2Deg;
            velocityMarker.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    public void SetMoveOrderMarker(Vector2 moveOrder, Vector2 playerPosition)
    {
        if (moveOrder == Vector2.zero)
        {
            moveOrderMarker.gameObject.SetActive(false);
        }
        else
        {
            moveOrderMarker.gameObject.SetActive(true);
            moveOrderMarker.position = moveOrder * markerOrbitRadius + (Vector2)Camera.main.WorldToScreenPoint(playerPosition);

            float angle = Mathf.Atan2(moveOrder.y, moveOrder.x) * Mathf.Rad2Deg;
            moveOrderMarker.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    public void SetHpBarRatio(float value)
    {
        hpBar.fillAmount = value;
        hpBar.color = new Color(1-value, value, 0);
    }
}
