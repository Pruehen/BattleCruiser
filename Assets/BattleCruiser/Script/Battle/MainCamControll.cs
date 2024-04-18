using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamControll : MonoBehaviour
{
    float minCamSize = 30;//최대 확대 사이즈
    float maxCamSize = 1000;//최대 축소 사이즈
    float defaultCamSize = 70;//기본 사이즈
    float sensitivity = 0.05f;//마우스 휠 민감도    
    float camSpeed = 10;//카메라가 얼마나 빨리 목표 위치로 이동할지. 1~20 권장
    float canRange = 10;//카메라가 얼마나 에임 위치로 멀리 이동할지. 1~20이하 권장
    float camSize;

    Vector3 camTargetPosition = Vector3.zero;//카메라의 목표 위치

    Camera cam;

    private void Awake()
    {
        camSize = defaultCamSize;
        cam = GetComponent<Camera>();

        sensitivity = GameManager.Instance.Setting.wheelSens;
        camSpeed = GameManager.Instance.Setting.camSpeed;
        canRange = GameManager.Instance.Setting.camRange;
    }
    void FixedUpdate()
    {
        Vector3 offset = ((Vector3)Player.Instance.screenAimPoint - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)) * 0.0001f * camSize;
        camTargetPosition = Player.Instance.transform.position + new Vector3(0, 0, -10) + offset * canRange;
        this.transform.position = Vector3.Lerp(this.transform.position, camTargetPosition, Time.deltaTime * camSpeed);
        cam.orthographicSize = camSize;
    }

    void OnZoom(InputValue inputValue)
    {
        float z = inputValue.Get<float>();
        if (z > 0)//휠 업
        {
            camSize = camSize - (camSize * sensitivity) - 1;
        }
        else if (z < 0)//휠 다운
        {
            camSize = camSize + (camSize * sensitivity) + 1;
        }

        camSize = Mathf.Clamp(camSize, minCamSize, maxCamSize);
    }
}
