using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class StrategyPlayer : MonoBehaviour
{
    Vector2 aimPoint = Vector2.zero;    
    Vector3 moveTargetPos = Vector3.zero;    
    float moveSpeed = 10;

    // 시야 영역의 반지름과 시야 각도
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

    void OnAim(InputValue inputValue)//마우스 위치
    {
        Vector2 screenAimPoint = inputValue.Get<Vector2>();//마우스 위치 받아옴
        aimPoint = Camera.main.ScreenToWorldPoint(screenAimPoint);
    }
    void OnLeftClick(InputValue inputValue)//마우스 좌클릭
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//눌렀을 때
        {
            Debug.Log("좌클릭 누름");
        }
        else//뗄 때
        {
            Debug.Log("좌클릭 뗌");
        }        
    }
    void OnRightClick(InputValue inputValue)//마우스 우클릭
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//눌렀을 때
        {
            Debug.Log("우클릭 누름");
        }
        else//뗄 때
        {
            Debug.Log("우클릭 뗌");
            MovePosSet(aimPoint);
        }
    }
    void MovePosSet(Vector2 position)
    {        
        moveTargetPos = position;
    }
}
