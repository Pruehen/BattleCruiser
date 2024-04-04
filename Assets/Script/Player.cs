using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    Rigidbody2D rigidbody2D;
    float mass = 0;
    bool isDead = false;

    Vector2 controllVector = Vector2.zero;//컨트롤 이동 벡터
    public float hoverPower = 5;//상하 이동 추력
    public float strafePower = 10;//좌우 이동 추력

    public Vector2 screenAimPoint = Vector2.zero;//에임 위치(스크린 기준)
    public Vector2 aimPosition = Vector2.zero;//에임 위치(절대 좌표)
    public Vector2 aimDirection = Vector2.zero;//에임 방향(상대 좌표)

    public bool fireTrigger = false;

    public Transform weaponsTrf;
    List<Weapon> childWeaponList;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        mass = rigidbody2D.mass;

        childWeaponList = new List<Weapon>();
        for (int i = 0; i < weaponsTrf.childCount; i++)
        {            
            childWeaponList.Add(weaponsTrf.GetChild(i).gameObject.GetComponent<Weapon>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetTurretTargetPos(aimPosition);

        aimPosition = Camera.main.ScreenToWorldPoint(screenAimPoint);//월드 벡터로 변환
        aimDirection = aimPosition - (Vector2)transform.position;//플레이어 기준 로컬 벡터로 변환
    }

    private void FixedUpdate()
    {
        AltitudeHoldPropulsion();
        ControllPropulsion();

        SetTurretParentVelocity(rigidbody2D.velocity);
    }
    void ControllPropulsion()//상하좌우 추진
    {
        rigidbody2D.AddForce(controllVector * mass, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//고도유지 추진
    {
        rigidbody2D.AddForce(Vector2.up * 9.8f * mass, ForceMode2D.Force);
    }
    void SetTurretTargetPos(Vector2 aimPoint)//터렛 목표 각도로 회전
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetTargetPoint(aimPoint);
        }
    }
    void SetTurretTrigger(bool value)//터렛 트리거 조정
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetTrigger(value);
        }
    }
    void SetTurretParentVelocity(Vector2 velocity)
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetParentVelocity(velocity);
        }
    }


    void OnMove(InputValue inputValue)//WASD 조작
    {
        Vector2 inputMovement = inputValue.Get<Vector2>();//인풋 벡터 받아옴
        controllVector = new Vector2(inputMovement.x * strafePower, inputMovement.y * hoverPower);//조종 벡터로 변환
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
        SetTurretTrigger(fireTrigger);
    }
    void OnRightClick(InputValue inputValue)//마우스 우클릭
    {
        Debug.Log(inputValue);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
