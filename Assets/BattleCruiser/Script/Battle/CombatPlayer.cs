using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class CombatPlayer : SceneSingleton<CombatPlayer>
{
    public Vector2 screenAimPoint = Vector2.zero;//에임 위치(스크린 기준)
    Vector2 inputMovement = Vector2.zero;
    Vehicle controlledShip;
    EquipWeaponData weaponData;
    public bool fireTrigger = false;    

    private void Awake()
    {
        controlledShip = GetComponent<Vehicle>();//현재 함선 클래스
        weaponData = GetComponent<EquipWeaponData>();//함선에 장비된 무기 데이터 클래스 (무기데이터 초기화에 사용)
    }

    private void Start()
    {
        controlledShip.Init(false, 1000000, 10000, 50, 10, 20, 3);

        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 300, 3, 10, 203, 1, 1, 45, 2f, Vector2.zero));
        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 300, 3, 10, 203, 1, 1, 45, 2f, Vector2.zero));
        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 300, 3, 10, 203, 1, 1, 45, 2f, Vector2.zero));
        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 300, 3, 10, 203, 1, 1, 45, 2f, Vector2.zero));

        controlledShip.WeaponInit(weaponData.weaponDatas);
    }

    // Update is called once per frame
    void Update()
    {
        controlledShip.SetAimPosition(Camera.main.ScreenToWorldPoint(screenAimPoint));
        controlledShip.SetControllVector(inputMovement);        

        PlayerUI.Instance.SetAltText(this.transform.position.y);
        PlayerUI.Instance.SetSpeedText(controlledShip.Rigidbody2D().velocity.magnitude);
        PlayerUI.Instance.SetMoveOrderMarker(inputMovement, this.transform.position);
        PlayerUI.Instance.SetVelocityMarker(controlledShip.Rigidbody2D().velocity, this.transform.position);
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
