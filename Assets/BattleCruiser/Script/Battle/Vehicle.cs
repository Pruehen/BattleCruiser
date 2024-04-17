using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;//�⺻ ����
    float totalMass = 0;//���⸦ ������ �� ����. mass���� ŭ
    public bool isDead { get; private set; }
    bool isSplashed = false;
    float hp;//���� ü��
    float maxHp;//�ִ� ü��
    bool isEnemy = false;
    bool isInit = false;
    public float maxEffectiveRange { get; private set; }//�ִ� ��ȿ ��Ÿ�
    public float minEffectiveRange { get; private set; }//�ּ� ��ȿ ��Ÿ�

    float HpRatio() { return 100 * hp / maxHp; }//hp����. 0~100�� ���� ����.
    float armor = 50;//����. 0~100�� ���� ����.
    public float maxWeaponVelocity { get; private set; }//���� ���� ���� �� ���� ź���� ���� ������ ź��

    bool calledAt75 = false;
    bool calledAt60 = false;
    bool calledAt40 = false;
    bool calledAt30 = false;
    bool calledAt20 = false;

    Vector2 controllVector = Vector2.zero;//��Ʈ�� �̵� ����
    float hoverPower = 5;//���� �̵� ���ӷ�
    float strafePower = 10;//�¿� �̵� ���ӷ�
    float horizontalRestorationPower = 2;//������
    float altPropulsionGain = 1;//���� ���� ������ ����

    //public Vector2 screenAimPoint = Vector2.zero;//���� ��ġ(��ũ�� ����)
    Vector2 aimPosition = Vector2.zero;//���� ��ġ(���� ��ǥ)
    Vector2 aimDirection = Vector2.zero;//���� ����(��� ��ǥ)

    bool fireTrigger = false;
    Vehicle target;//������ Ÿ��.

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
        rigidbody2D = GetComponent<Rigidbody2D>();//�ڱ� �ڽ��� ������ٵ�        

        childWeaponList = new List<Weapon>();//�ڱ� �ڽ��� ���� ����Ʈ
        spriteTrfs = new List<Transform>();//�ڱ� �ڽ��� ��������Ʈ ����ü ����Ʈ       
        mainEngineTrfs = new List<Transform>();//�ڱ� �ڽ��� ���� ����Ʈ
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
            spriteTrfs.Add(spriteTrf.GetChild(i));//��������Ʈ ����
        }
        for (int i = 0; i < mainEngineTrf.childCount; i++)
        {
            mainEngineTrfs.Add(mainEngineTrf.GetChild(i));//���� ���� ��ġ ����
            EffectManager.Instance.GenerateEngineEffect(mainEngineTrfs[i]);            
        }
        for (int i = 0; i < subEngineTrf.childCount; i++)
        {
            subEngineTrfs.Add(subEngineTrf.GetChild(i));//���� ���� ��ġ ����
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

        if (weaponDatas.Count == weaponsTrf.childCount)//���޹��� ���� ������ ���� Ʈ�������� �ڽ� ������ ���� ���
        {
            for (int i = 0; i < weaponDatas.Count; i++)
            {
                if (weaponSpriteIndex[i] == -1)//������ ���� ��� �ش� ���� �ǳʶ�
                    continue;

                Weapon weapon = Instantiate(PrefabManager.Instance.weapons[weaponSpriteIndex[i]], weaponsTrf.GetChild(i)).GetComponent<Weapon>();//���� ����
                totalMass += weaponDatas[i].mass;//�������� ������ ���� �߰�

                childWeaponList.Add(weapon);
                weapon.Init(isEnemy, weaponDatas[i], weapon.gameObject.transform.localPosition, this);

                if (weaponDatas[i].projectiledVelocity > maxWeaponVelocity)//�ִ� ź�� ����
                {
                    maxWeaponVelocity = weaponDatas[i].projectiledVelocity;
                }
                if (weapon.effectiveRange > maxEffectiveRange)//�ִ� ��ȿ ��Ÿ� ����
                {
                    maxEffectiveRange = weapon.effectiveRange;
                }
                if (weapon.effectiveRange < minEffectiveRange)//�ּ� ��ȿ ��Ÿ� ����
                {
                    minEffectiveRange = weapon.effectiveRange;
                }
            }            
            rigidbody2D.mass = totalMass;
        }
        else
        {
            Debug.LogError("������ ������ �ٸ��ϴ�");
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        if(isInit == false)
        {
            Debug.LogWarning("�ʱ�ȭ���� ���� �Լ� ���");
        }

        if (!isDead)
        {
            SetTurretTargetPos(aimPosition);
            SetTurretParentVelocity(rigidbody2D.velocity);

            aimDirection = aimPosition - (Vector2)transform.position;//��� �߽� ���� ���� ���ͷ� ��ȯ

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

            AltitudeHoldPropulsion();//������ ����
            ControllPropulsion();//�����¿� ����
            HorizontalRestoration();//���򺹿� ȸ��
        }
    }
    void ControllPropulsion()//�����¿� ����
    {
        rigidbody2D.AddForce(controllVector * mass * altPropulsionGain, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//������ ����
    {
        rigidbody2D.AddForce(this.transform.up * 9.8f * totalMass * altPropulsionGain, ForceMode2D.Force);
    }
    void HorizontalRestoration()//���� ����
    {
        rigidbody2D.AddTorque(this.transform.up.x * horizontalRestorationPower * mass * 360, ForceMode2D.Force);
    }
    void SetTurretTargetPos(Vector2 aimPoint)//�ͷ� ��ǥ ������ ȸ��
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetTargetPoint(aimPoint);
        }
    }
    void SetTurretTrigger(bool value)//�ͷ� Ʈ���� ����
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetTrigger(value);
        }
    }
    void SetTurretParentVelocity(Vector2 velocity)//�ͷ��� ������ �ӵ� ����
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetParentVelocity(velocity);
        }
    }


    public void SetControllVector(Vector2 controllVector)//���� ��ǲ
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
    public void SetAimPosition(Vector2 aimPosition)//���� ��ǲ
    {
        this.aimPosition = aimPosition;
    }
    public void SetTrigger(bool value)//�߻� Ʈ���� ��ǲ
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
            apDmg = Mathf.Clamp(apDmg - (apDmg * armor * 0.01f) - armor, 1, apDmg);//���¿� ���� ���� ������ �氨
            //Debug.Log($"���� ������ : {apDmg}");
            //Debug.Log($"���� ������ : {heDmg}");
            hp -= apDmg + heDmg;
            
            if(isEnemy)
            {
                BattleSceneManager.Instance.KineticDmgUp(apDmg);
                BattleSceneManager.Instance.ChemicalDmgUp(heDmg);
            }

            float hpRatio = HpRatio();
            //Debug.Log($"ü�� ���� : {hpRatio}");

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
        rigidbody2D.AddTorque(Random.Range(-90f * mass, 90f * mass), ForceMode2D.Impulse);//���� ȸ�� �ο�

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
