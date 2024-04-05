using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;//질량 (Rigidbody2D의 정보를 받아옴)
    bool isDead = false;
    float hp;//현재 체력
    float maxHp = 100000;//최대 체력
    float HpRatio() { return 100 * hp / maxHp; }//hp비율. 0~100의 값을 가짐.
    float armor = 40;//방어력. 0~100의 값을 가짐.

    bool calledAt75 = false;
    bool calledAt50 = false;
    bool calledAt25 = false;
    bool calledAt12 = false;
    bool calledAt6 = false;

    Vector2 controllVector = Vector2.zero;//컨트롤 이동 벡터
    float hoverPower = 5;//상하 이동 가속력
    float strafePower = 10;//좌우 이동 가속력
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

    private void Start()
    {
        hp = maxHp;        
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

    public void Demage(float apDmg, float heDmg)
    {
        apDmg = Mathf.Clamp(apDmg - (apDmg * armor * 0.01f) - armor, 1, apDmg);//방어력에 따른 물리 데미지 경감
        Debug.Log($"물리 데미지 : {apDmg}");
        Debug.Log($"폭발 데미지 : {heDmg}");
        hp -= apDmg + heDmg;
        float hpRatio = HpRatio();
        Debug.Log($"체력 비율 : {hpRatio}");

        if(!calledAt75 && hpRatio < 75)
        {
            calledAt75 = true;
        }
        else if (!calledAt50 && hpRatio < 50)
        {
            calledAt50 = true;
        }
        else if (!calledAt25 && hpRatio < 50)
        {
            calledAt25 = true;
        }
        else if (!calledAt12 && hpRatio < 50)
        {
            calledAt12 = true;
        }
        else if (!calledAt6 && hpRatio < 50)
        {
            calledAt6 = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
