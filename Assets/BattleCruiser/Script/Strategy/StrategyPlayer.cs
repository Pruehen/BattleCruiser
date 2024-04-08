using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class StrategyPlayer : MonoBehaviour
{
    Vector2 aimPoint = Vector2.zero;    
    Vector3 moveTargetPos = Vector3.zero;    
    float moveSpeed = 10;

    // �þ� ������ �������� �þ� ����
    float viewRadius = 200;    


    // Start is called before the first frame updat
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(moveTargetPos != Vector3.zero)
        {
            this.transform.position += (moveTargetPos - this.transform.position).normalized * moveSpeed * Time.deltaTime;
            if((moveTargetPos - this.transform.position).sqrMagnitude < 0.1f)
            {
                moveTargetPos = Vector3.zero;
            }
        }
    }

    void OnAim(InputValue inputValue)//���콺 ��ġ
    {
        Vector2 screenAimPoint = inputValue.Get<Vector2>();//���콺 ��ġ �޾ƿ�
        aimPoint = Camera.main.ScreenToWorldPoint(screenAimPoint);
    }
    void OnLeftClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//������ ��
        {
            Debug.Log("��Ŭ�� ����");
        }
        else//�� ��
        {
            Debug.Log("��Ŭ�� ��");
        }        
    }
    void OnRightClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//������ ��
        {
            Debug.Log("��Ŭ�� ����");
        }
        else//�� ��
        {
            Debug.Log("��Ŭ�� ��");
            MovePosSet(aimPoint);
        }
    }
    void MovePosSet(Vector2 position)
    {        
        moveTargetPos = position;
    }
}
