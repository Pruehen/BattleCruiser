using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;//질량 (Rigidbody2D의 정보를 받아옴)    
    bool isDead = false;
    bool isSplashed = false;
    float hp;//현재 체력
    float maxHp;//최대 체력
    bool isEnemy = false;
    bool isInit = false;
    float HpRatio() { return 100 * hp / maxHp; }//hp비율. 0~100의 값을 가짐.
    float armor = 50;//방어력. 0~100의 값을 가짐.

    bool calledAt75 = false;
    bool calledAt60 = false;
    bool calledAt40 = false;
    bool calledAt30 = false;
    bool calledAt20 = false;

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

    public Transform spriteTrf;
    List<Transform> spriteTrfs;   
   

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        mass = rigidbody2D.mass;

        childWeaponList = new List<Weapon>();
        for (int i = 0; i < weaponsTrf.childCount; i++)
        {
            childWeaponList.Add(weaponsTrf.GetChild(i).gameObject.GetComponent<Weapon>());
        }

        spriteTrfs = new List<Transform>();
        for (int i = 0; i < spriteTrf.childCount; i++)
        {
            spriteTrfs.Add(spriteTrf.GetChild(i));
        }        
    }
    public void Init(bool isEnemy, float maxHp, float mass, float armor, float hoverPower, float strafePower, float horizontalRestorationPower)
    {
        this.isEnemy = isEnemy;
        this.maxHp = maxHp;
        hp = maxHp;
        this.mass = mass;
        rigidbody2D.mass = mass;
        this.armor = armor;
        this.hoverPower = hoverPower;
        this.strafePower = strafePower;
        this.horizontalRestorationPower = horizontalRestorationPower;

        isInit = true;
    }
    public void WeaponInit(List<WeaponData> weaponDatas)
    {
        if(weaponDatas.Count == childWeaponList.Count)//두 리스트의 수가 같을 경우
        {
            for (int i = 0; i < weaponDatas.Count; i++)
            {
                childWeaponList[i].Init(isEnemy, weaponDatas[i], childWeaponList[i].gameObject.transform.localPosition);
            }
        }
        else
        {
            Debug.LogError("무장의 수량이 다릅니다.");
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        if(isInit == false)
        {
            Debug.LogWarning("초기화되지 않은 함선 사용");
        }

        if (!isDead)
        {
            SetTurretTargetPos(aimPosition);
            SetTurretParentVelocity(rigidbody2D.velocity);

            aimDirection = aimPosition - (Vector2)transform.position;//장비 중심 기준 로컬 벡터로 변환
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            AltitudeHoldPropulsion();//고도유지 추진
            ControllPropulsion();//상하좌우 추진
            HorizontalRestoration();//수평복원 회전
        }
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
        if (isDead == false)
        {
            fireTrigger = value;
        }
        else
        {
            fireTrigger = false;
        }
        SetTurretTrigger(fireTrigger);
    }

    public void Demage(float apDmg, float heDmg)
    {
        if (!isDead)
        {
            apDmg = Mathf.Clamp(apDmg - (apDmg * armor * 0.01f) - armor, 1, apDmg);//방어력에 따른 물리 데미지 경감
            //Debug.Log($"물리 데미지 : {apDmg}");
            //Debug.Log($"폭발 데미지 : {heDmg}");
            hp -= apDmg + heDmg;
            float hpRatio = HpRatio();
            Debug.Log($"체력 비율 : {hpRatio}");

            DemageEffectGenerate(hpRatio);

            if (hpRatio <= 0 && !isDead)
            {
                Dead();
            }
        }
    }

    void DemageEffectGenerate(float hpRatio)
    {
        if (!calledAt75 && hpRatio < 75)
        {
            calledAt75 = true;
            int randomIndex = Random.Range(0, spriteTrfs.Count);
            EffectManager.Instance.GenerateDemageEffect(spriteTrfs[randomIndex], spriteTrfs[randomIndex].position, 0);
            spriteTrfs.RemoveAt(randomIndex);
        }
        if (!calledAt60 && hpRatio < 60)
        {
            calledAt60 = true;
            int randomIndex = Random.Range(0, spriteTrfs.Count);
            EffectManager.Instance.GenerateDemageEffect(spriteTrfs[randomIndex], spriteTrfs[randomIndex].position, 0);
            spriteTrfs.RemoveAt(randomIndex);
        }
        if (!calledAt40 && hpRatio < 40)
        {
            calledAt40 = true;
            int randomIndex = Random.Range(0, spriteTrfs.Count);
            EffectManager.Instance.GenerateDemageEffect(spriteTrfs[randomIndex], spriteTrfs[randomIndex].position, 1);
            spriteTrfs.RemoveAt(randomIndex);
        }
        if (!calledAt30 && hpRatio < 30)
        {
            calledAt30 = true;
            int randomIndex = Random.Range(0, spriteTrfs.Count);
            EffectManager.Instance.GenerateDemageEffect(spriteTrfs[randomIndex], spriteTrfs[randomIndex].position, 1);
            spriteTrfs.RemoveAt(randomIndex);
        }
        if (!calledAt20 && hpRatio < 20)
        {
            calledAt20 = true;
            int randomIndex = Random.Range(0, spriteTrfs.Count);
            EffectManager.Instance.GenerateDemageEffect(spriteTrfs[randomIndex], spriteTrfs[randomIndex].position, 1);
            spriteTrfs.RemoveAt(randomIndex);
        }
    }

    void Dead()
    {
        isDead = true;
        rigidbody2D.angularDrag = 0;
        rigidbody2D.AddTorque(Random.Range(-90f * mass, 90f * mass), ForceMode2D.Impulse);//임의 회전 부여

        StartCoroutine(OnDestructEffect(0));
        StartCoroutine(OnDestructEffect(0.5f));
        StartCoroutine(OnDestructEffect(1));
    }

    IEnumerator OnDestructEffect(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);        
        EffectManager.Instance.GenerateDemageEffect(this.transform, (Vector2)this.transform.position + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)), 2);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead && !isSplashed && collision.gameObject.CompareTag("Ground"))
        {
            isSplashed = true;
            SetTurretTrigger(false);
            EffectManager.Instance.GenerateDemageEffect(this.transform, collision.contacts[0].point, 3);
        }
    }
}
