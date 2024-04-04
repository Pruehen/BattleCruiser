using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamControll : MonoBehaviour
{
    float minCamSize = 30;//�ִ� Ȯ�� ������
    float maxCamSize = 150;//�ִ� ��� ������
    float defaultCamSize = 70;//�⺻ ������
    float sensitivity = 5f;//���콺 �� �ΰ���    
    float camSpeed = 10;//ī�޶� �󸶳� ���� ��ǥ ��ġ�� �̵�����. 1~10 ����
    float canRange = 0.1f;//ī�޶� �󸶳� ���� ��ġ�� �ָ� �̵�����. 0.5���� ����
    float camSize;

    Vector3 camTargetPosition = Vector3.zero;//ī�޶��� ��ǥ ��ġ

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
        if (z > 0)//�� ��
            camSize -= sensitivity;//Ȯ��
        else if (z < 0)//�� �ٿ�
            camSize += sensitivity;//���

        camSize = Mathf.Clamp(camSize, minCamSize, maxCamSize);
    }
}
