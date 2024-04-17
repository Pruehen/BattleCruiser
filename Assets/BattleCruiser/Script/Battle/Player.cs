using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class Player : SceneSingleton<Player>
{
    public Vector2 screenAimPoint = Vector2.zero;//에임 위치(스크린 기준)
    Vector2 worldAimPoint = Vector2.zero;//에임 위치(월드 좌표)
    Vector2 inputMovement = Vector2.zero;
    public Vehicle controlledShip { get; private set; }
    
    public bool fireTrigger = false;
    bool isInit = false;

    public void Init(PlayerShipData playerShipData)
    {
        controlledShip = GetComponent<Vehicle>();//현재 함선 클래스

        controlledShip.Init(false, playerShipData.shipData);//함선 데이터 초기화
        controlledShip.WeaponInit(playerShipData.weaponSpriteIndexs, playerShipData.weaponDatas);//함선의 무기 초기화

        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit)
        {
            worldAimPoint = Camera.main.ScreenToWorldPoint(screenAimPoint);
            controlledShip.SetAimPosition(worldAimPoint);
            controlledShip.SetControllVector(inputMovement);

            PlayerUI.Instance.SetAimDistanceText((worldAimPoint - (Vector2)this.transform.position).magnitude);
            PlayerUI.Instance.SetAltText(this.transform.position.y);
            PlayerUI.Instance.SetSpeedText(controlledShip.Rigidbody2D().velocity.magnitude);
            PlayerUI.Instance.SetMoveOrderMarker(inputMovement, this.transform.position);
            PlayerUI.Instance.SetVelocityMarker(controlledShip.Rigidbody2D().velocity, this.transform.position);
            if (controlledShip.GetTarget() != null)
            {
                Vector2 targetPos = controlledShip.GetTarget().transform.position;
                PlayerUI.Instance.SetLockOnPointPosition(targetPos);
                PlayerUI.Instance.SetLockOnDistanceText((targetPos - (Vector2)this.transform.position).magnitude);
            }
            else
            {
                PlayerUI.Instance.DisableLockOnPoint();
            }
        }
    }

    void TargetLockOn()
    {
        List<Vehicle> vehicles = BattleSceneManager.Instance.activeEnemyList;
        float mindistance = float.MaxValue;
        Vehicle target = null;

        foreach (Vehicle item in vehicles)
        {
            float sqrdistance = ((Vector2)item.gameObject.transform.position - worldAimPoint).sqrMagnitude;

            if(sqrdistance < mindistance && item.isDead == false && sqrdistance < 1600)
            {
                target = item;
                mindistance = sqrdistance;
            }
        }

        controlledShip.SetTarget(target);
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
            if(controlledShip.GetTarget() == null)
            {
                TargetLockOn();
            }
        }
        else//뗄 때
        {
            fireTrigger = false;
        }
        controlledShip.SetTrigger(fireTrigger);
    }
    void OnRightClick(InputValue inputValue)//마우스 우클릭
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//눌렀을 때
        {
            TargetLockOn();
        }
    }
}
