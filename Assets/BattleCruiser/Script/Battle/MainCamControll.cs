using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamControll : MonoBehaviour
{
    float minCamSize = 30;//�ִ� Ȯ�� ������
    float maxCamSize = 1000;//�ִ� ��� ������
    float defaultCamSize = 70;//�⺻ ������
    float sensitivity = 0.05f;//���콺 �� �ΰ���    
    float camSpeed = 10;//ī�޶� �󸶳� ���� ��ǥ ��ġ�� �̵�����. 1~20 ����
    float canRange = 10;//ī�޶� �󸶳� ���� ��ġ�� �ָ� �̵�����. 1~20���� ����
    float camSize;

    Vector3 camTargetPosition = Vector3.zero;//ī�޶��� ��ǥ ��ġ

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
}
