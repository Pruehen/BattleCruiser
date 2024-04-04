using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamControll : MonoBehaviour
{
    float minCamSize = 30;//최대 확대 사이즈
    float maxCamSize = 150;//최대 축소 사이즈
    float defaultCamSize = 70;//기본 사이즈
    float sensitivity = 5f;//마우스 휠 민감도    
    float camSpeed = 10;//카메라가 얼마나 빨리 목표 위치로 이동할지. 1~10 권장
    float canRange = 0.1f;//카메라가 얼마나 에임 위치로 멀리 이동할지. 0.5이하 권장
    float camSize;

    Vector3 camTargetPosition = Vector3.zero;//카메라의 목표 위치

    Camera cam;

    private void Awake()
    {
        camSize = defaultCamSize;
        cam = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        camTargetPosition = Player.Instance.transform.position + new Vector3(0, 0, -10) + (Vector3)Player.Instance.aimDirection * canRange;
        this.transform.position = Vector3.Lerp(this.transform.position, camTargetPosition, Time.deltaTime * camSpeed);
        cam.orthographicSize = camSize;
    }

    void OnZoom(InputValue inputValue)
    {
        float z = inputValue.Get<float>();
        if (z > 0)//휠 업
            camSize -= sensitivity;//확대
        else if (z < 0)//휠 다운
            camSize += sensitivity;//축소

        camSize = Mathf.Clamp(camSize, minCamSize, maxCamSize);
    }
}
