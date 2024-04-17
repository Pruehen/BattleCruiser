using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;//기본 질량
    float totalMass = 0;//무기를 포함한 총 질량. mass보다 큼
    public bool isDead { get; private set; }
    bool isSplashed = false;
    float hp;//현재 체력
    float maxHp;//최대 체력
    bool isEnemy = false;
    bool isInit = false;
    public float maxEffectiveRange { get; private set; }//최대 유효 사거리
    public float minEffectiveRange { get; private set; }//최소 유효 사거리

    float HpRatio() { return 100 * hp / maxHp; }//hp비율. 0~100의 값을 가짐.
    float armor = 50;//방어력. 0~100의 값을 가짐.
    public float maxWeaponVelocity { get; private set; }//현재 장비된 무기 중 가장 탄속이 높은 무기의 탄속

    bool calledAt75 = false;
    bool calledAt60 = false;
    bool calledAt40 = false;
    bool calledAt30 = false;
    bool calledAt20 = false;

    Vector2 controllVector = Vector2.zero;//컨트롤 이동 벡터
    float hoverPower = 5;//상하 이동 가속력
    float strafePower = 10;//좌우 이동 가속력
    float horizontalRestorationPower = 2;//복원력
    float altPropulsionGain = 1;//고도에 따른 추진력 감소

    //public Vector2 screenAimPoint = Vector2.zero;//에임 위치(스크린 기준)
    Vector2 aimPosition = Vector2.zero;//에임 위치(절대 좌표)
    Vector2 aimDirection = Vector2.zero;//에임 방향(상대 좌표)

    bool fireTrigger = false;
    Vehicle target;//락온한 타겟.

    public Transform weaponsTrf;
    List<Weapon> childWeaponList;

    public Transform spriteTrf;
    List<Transform> spriteTrfs;

    public Transform mainEngineTrf;
    public Transform subEngineTrf;
    List<Transform> mainEngineTrfs;
    List<Transform> subEngineTrfs;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();//자기 자신의 리지드바디        

        childWeaponList = new List<Weapon>();//자기 자신의 무기 리스트
        spriteTrfs = new List<Transform>();//자기 자신의 스프라이트 구성체 리스트       
        mainEngineTrfs = new List<Transform>();//자기 자신의 엔진 리스트
        subEngineTrfs = new List<Transform>();
    }

    public void Init(bool isEnemy, ShipData shipData)
    {
        if (shipData != null)
        {
            this.isEnemy = isEnemy;
            this.maxHp = shipData.maxHp;
            hp = maxHp;
            this.mass = shipData.mass;
            totalMass = mass;
            rigidbody2D.mass = totalMass;
            this.armor = shipData.armor;
            this.hoverPower = shipData.hoverPower;
            this.strafePower = shipData.strafePower;
            this.horizontalRestorationPower = shipData.horizontalRestorationPower;

            isInit = true;            
        }

        for (int i = 0; i < spriteTrf.childCount; i++)
        {
            spriteTrfs.Add(spriteTrf.GetChild(i));//스프라이트 저장
        }
        for (int i = 0; i < mainEngineTrf.childCount; i++)
        {
            mainEngineTrfs.Add(mainEngineTrf.GetChild(i));//메인 엔진 위치 저장
            EffectManager.Instance.GenerateEngineEffect(mainEngineTrfs[i]);            
        }
        for (int i = 0; i < subEngineTrf.childCount; i++)
        {
            subEngineTrfs.Add(subEngineTrf.GetChild(i));//서브 엔진 위치 저장
            EffectManager.Instance.GenerateEngineEffect(subEngineTrfs[i]);
            subEngineTrfs[i].localScale *= 0.5f;
        }

        isDead = false;
    }
    public void WeaponInit(List<int> weaponSpriteIndex, List<WeaponData> weaponDatas)
    {
        maxWeaponVelocity = 0;
        maxEffectiveRange = 0;
        minEffectiveRange = float.MaxValue;

        if (weaponDatas.Count == weaponsTrf.childCount)//전달받은 무기 수량과 웨폰 트랜스폼의 자식 수량이 같을 경우
        {
            for (int i = 0; i < weaponDatas.Count; i++)
            {
                if (weaponSpriteIndex[i] == -1)//무장이 없을 경우 해당 루프 건너뜀
                    continue;

                Weapon weapon = Instantiate(PrefabManager.Instance.weapons[weaponSpriteIndex[i]], weaponsTrf.GetChild(i)).GetComponent<Weapon>();//무장 생성
                totalMass += weaponDatas[i].mass;//총질량에 무장의 질량 추가

                childWeaponList.Add(weapon);
                weapon.Init(isEnemy, weaponDatas[i], weapon.gameObject.transform.localPosition, this);

                if (weaponDatas[i].projectiledVelocity > maxWeaponVelocity)//최대 탄속 설정
                {
                    maxWeaponVelocity = weaponDatas[i].projectiledVelocity;
                }
                if (weapon.effectiveRange > maxEffectiveRange)//최대 유효 사거리 설정
                {
                    maxEffectiveRange = weapon.effectiveRange;
                }
                if (weapon.effectiveRange < minEffectiveRange)//최소 유효 사거리 설정
                {
                    minEffectiveRange = weapon.effectiveRange;
                }
            }            
            rigidbody2D.mass = totalMass;
        }
        else
        {
            Debug.LogError("무장의 수량이 다릅니다");
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

            if(target != null && target.isDead)
            {
                SetTarget(null);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            float airPressure = Mathf.Clamp(200 / (this.transform.position.y + 50), 0, 1);
            altPropulsionGain = airPressure;
            rigidbody2D.drag = airPressure;

            AltitudeHoldPropulsion();//고도유지 추진
            ControllPropulsion();//상하좌우 추진
            HorizontalRestoration();//수평복원 회전
        }
    }
    void ControllPropulsion()//상하좌우 추진
    {
        rigidbody2D.AddForce(controllVector * mass * altPropulsionGain, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//고도유지 추진
    {
        rigidbody2D.AddForce(this.transform.up * 9.8f * totalMass * altPropulsionGain, ForceMode2D.Force);
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
        foreach (Transform t in subEngineTrfs)
        {
            if(controllVector == Vector2.zero || isDead)
            {
                t.gameObject.SetActive(false);
            }
            else if(!isDead)
            {
                t.gameObject.SetActive(true);
                t.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(controllVector.y, controllVector.x) * Mathf.Rad2Deg - 90);
            }            
        }
    }
    public void SetAimPosition(Vector2 aimPosition)//조준 인풋
    {
        this.aimPosition = aimPosition;
    }
    public void SetTrigger(bool value)//발사 트리거 인풋
    {
        if (isSplashed == false)
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
            
            if(isEnemy)
            {
                BattleSceneManager.Instance.KineticDmgUp(apDmg);
                BattleSceneManager.Instance.ChemicalDmgUp(heDmg);
            }

            float hpRatio = HpRatio();
            //Debug.Log($"체력 비율 : {hpRatio}");

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
        rigidbody2D.drag = 0.05f;
        rigidbody2D.AddTorque(Random.Range(-90f * mass, 90f * mass), ForceMode2D.Impulse);//임의 회전 부여

        foreach (Transform t in mainEngineTrfs)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in subEngineTrfs)
        {
            t.gameObject.SetActive(false);
        }

        StartCoroutine(OnDestructEffect(0));
        StartCoroutine(OnDestructEffect(0.5f));
        StartCoroutine(OnDestructEffect(1));
        StartCoroutine(OnDestructEffect(1.5f));
        StartCoroutine(OnDestructEffect(2));
    }

    IEnumerator OnDestructEffect(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);        
        EffectManager.Instance.GenerateDemageEffect(this.transform, (Vector2)this.transform.position + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)), 2);
    }

    public void SetTarget(Vehicle target)
    {
        this.target = target;
    }
    public Vehicle GetTarget()
    {
        return this.target;
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
