using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;//���� (Rigidbody2D�� ������ �޾ƿ�)
    bool isDead = false;
    float hp;//���� ü��
    float maxHp = 100000;//�ִ� ü��
    float HpRatio() { return 100 * hp / maxHp; }//hp����. 0~100�� ���� ����.
    float armor = 40;//����. 0~100�� ���� ����.

    bool calledAt75 = false;
    bool calledAt50 = false;
    bool calledAt25 = false;
    bool calledAt12 = false;
    bool calledAt6 = false;

    Vector2 controllVector = Vector2.zero;//��Ʈ�� �̵� ����
    float hoverPower = 5;//���� �̵� ���ӷ�
    float strafePower = 10;//�¿� �̵� ���ӷ�
    float horizontalRestorationPower = 2;//������

    //public Vector2 screenAimPoint = Vector2.zero;//���� ��ġ(��ũ�� ����)
    Vector2 aimPosition = Vector2.zero;//���� ��ġ(���� ��ǥ)
    Vector2 aimDirection = Vector2.zero;//���� ����(��� ��ǥ)

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

        aimDirection = aimPosition - (Vector2)transform.position;//��� �߽� ���� ���� ���ͷ� ��ȯ
    }

    private void FixedUpdate()
    {
        AltitudeHoldPropulsion();//������ ����
        ControllPropulsion();//�����¿� ����
        HorizontalRestoration();//���򺹿� ȸ��
    }
    void ControllPropulsion()//�����¿� ����
    {
        rigidbody2D.AddForce(controllVector * mass, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//������ ����
    {
        rigidbody2D.AddForce(this.transform.up * 9.8f * mass, ForceMode2D.Force);
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
    }
    public void SetAimPosition(Vector2 aimPosition)//���� ��ǲ
    {
        this.aimPosition = aimPosition;
    }
    public void SetTrigger(bool value)//�߻� Ʈ���� ��ǲ
    {
        fireTrigger = value;
        SetTurretTrigger(fireTrigger);
    }

    public void Demage(float apDmg, float heDmg)
    {
        apDmg = Mathf.Clamp(apDmg - (apDmg * armor * 0.01f) - armor, 1, apDmg);//���¿� ���� ���� ������ �氨
        Debug.Log($"���� ������ : {apDmg}");
        Debug.Log($"���� ������ : {heDmg}");
        hp -= apDmg + heDmg;
        float hpRatio = HpRatio();
        Debug.Log($"ü�� ���� : {hpRatio}");

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
