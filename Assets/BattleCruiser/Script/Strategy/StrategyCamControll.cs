using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

public class StrategyCamControll : MonoBehaviour
{
    float minCamSize = 10;//최대 확대 사이즈
    float maxCamSize = 100;//최대 축소 사이즈
    float defaultCamSize = 20;//기본 사이즈
    float sensitivity = 0.05f;//마우스 휠 민감도    
    float camSpeed = 0.1f;//카메라의 사이즈 비례 초당 이동 속도. 0.2 이하 권장
    float camSize;

    Camera cam;
    Vector2 inputMovement;

    private void Awake()
    {
        camSize = defaultCamSize;
        cam = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        this.transform.position += new Vector3(inputMovement.x * camSpeed * camSize, inputMovement.y * camSpeed * camSize, 0);
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
    void OnMove(InputValue inputValue)//WASD 조작
    {
        inputMovement = inputValue.Get<Vector2>();//인풋 벡터 받아옴        
    }
}
