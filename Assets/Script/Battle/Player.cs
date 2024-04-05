using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    public Vector2 screenAimPoint = Vector2.zero;//에임 위치(스크린 기준)
    Vector2 inputMovement = Vector2.zero;
    Vehicle controlledShip;
    public bool fireTrigger = false;

    private void Awake()
    {
        controlledShip = GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        controlledShip.SetAimPosition(Camera.main.ScreenToWorldPoint(screenAimPoint));
        controlledShip.SetControllVector(inputMovement);
        controlledShip.SetTrigger(fireTrigger);

        PlayerUI.Instance.SetAltText(this.transform.position.y);
        PlayerUI.Instance.SetSpeedText(controlledShip.Rigidbody2D().velocity.magnitude);
    }


    void OnMove(InputValue inputValue)//WASD 조작
    {
        inputMovement = inputValue.Get<Vector2>();//인풋 벡터 받아옴        
    }
    void OnAim(InputValue inputValue)//마우스 위치
    {
        screenAimPoint = inputValue.Get<Vector2>();//마우스 위치 받아옴
        PlayerUI.Instance.SetAimPointPosition(screenAimPoint);//에임포인트 위치갱신
    }
    void OnLeftClick(InputValue inputValue)//마우스 좌클릭
    {
        float isClick = inputValue.Get<float>();

        if(isClick == 1)//눌렀을 때
        {
            fireTrigger = true;
        }
        else//뗄 때
        {
            fireTrigger = false;
        }
        controlledShip.SetTrigger(fireTrigger);
    }
    void OnRightClick(InputValue inputValue)//마우스 우클릭
    {
        Debug.Log(inputValue);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
