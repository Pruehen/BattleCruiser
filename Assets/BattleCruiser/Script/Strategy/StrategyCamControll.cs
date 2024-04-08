using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;

public class StrategyCamControll : MonoBehaviour
{
    float minCamSize = 10;//�ִ� Ȯ�� ������
    float maxCamSize = 100;//�ִ� ��� ������
    float defaultCamSize = 20;//�⺻ ������
    float sensitivity = 0.05f;//���콺 �� �ΰ���    
    float camSpeed = 0.1f;//ī�޶��� ������ ��� �ʴ� �̵� �ӵ�. 0.2 ���� ����
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
        if (z > 0)//�� ��
        {
            camSize = camSize - (camSize * sensitivity) - 1;
        }
        else if (z < 0)//�� �ٿ�
        {
            camSize = camSize + (camSize * sensitivity) + 1;
        }

        camSize = Mathf.Clamp(camSize, minCamSize, maxCamSize);
    }
    void OnMove(InputValue inputValue)//WASD ����
    {
        inputMovement = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�        
    }
}
