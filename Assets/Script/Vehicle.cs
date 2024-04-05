using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;
    bool isDead = false;

    Vector2 controllVector = Vector2.zero;//컨트롤 이동 벡터
    float hoverPower = 5;//상하 이동 추력
    float strafePower = 10;//좌우 이동 추력
    float horizontalRestorationPower = 2;//복원력

    //public Vector2 screenAimPoint = Vector2.zero;//에임 위치(스크린 기준)
    Vector2 aimPosition = Vector2.zero;//에임 위치(절대 좌표)
    Vector2 aimDirection = Vector2.zero;//에임 방향(상대 좌표)

    bool fireTrigger = false;

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
        SetTurretParentVelocity(rigidbody2D.velocity);        

        aimDirection = aimPosition - (Vector2)transform.position;//장비 중심 기준 로컬 벡터로 변환
    }

    private void FixedUpdate()
    {
        AltitudeHoldPropulsion();//고도유지 추진
        ControllPropulsion();//상하좌우 추진
        HorizontalRestoration();//수평복원 회전
    }
    void ControllPropulsion()//상하좌우 추진
    {
        rigidbody2D.AddForce(controllVector * mass, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//고도유지 추진
    {
        rigidbody2D.AddForce(this.transform.up * 9.8f * mass, ForceMode2D.Force);
    }
    void HorizontalRestoration()//수평 복원
    {
        rigidbody2D.AddTorque(this.transform.up.x * horizontalRestorationPower * mass * 360, ForceMode2D.Force);
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
    void SetTurretParentVelocity(Vector2 velocity)//터렛에 모함의 속도 전달
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetParentVelocity(velocity);
        }
    }


    public void SetControllVector(Vector2 controllVector)//조종 인풋
    {
        this.controllVector = new Vector2(controllVector.x * strafePower, controllVector.y * hoverPower);
    }
    public void SetAimPosition(Vector2 aimPosition)//조준 인풋
    {
        this.aimPosition = aimPosition;
    }
    public void SetTrigger(bool value)//발사 트리거 인풋
    {
        fireTrigger = value;
        SetTurretTrigger(fireTrigger);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
